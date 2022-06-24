using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class VangoGrabber :AbstractGrabber
    {
        private ConnectClient _httpClient;
        private JsonFunction _json;
        private IOptions<VangoOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<VangoGrabber> _logger;
        private readonly IDataAccess dataAccess;

        public VangoGrabber(ConnectClient httpClient, JsonFunction json, IOptions<VangoOptions> options, IMemoryCache memoryCache, ILogger<VangoGrabber> logger, IOptions<NorthEastOptions> absOptions, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _httpClient = httpClient;
            _json = json;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }


        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            _logger.LogInformation("start grabbing Vango rowdata");
            var hkQuery = new Dictionary<string, string>()
            {
                ["regionID"] = _options.Value.HKRegionID.ToString()
            };
            var klnQuery = new Dictionary<string, string>()
            {
                ["regionID"] = _options.Value.KLNRegionID.ToString()
            };
            var ntQuery = new Dictionary<string, string>()
            {
                ["regionID"] = _options.Value.NTRegionID.ToString()
            };
            var hkConnectHttp = await _httpClient.GetAsync(_options.Value.Url, hkQuery);
            var klnConnectHttp = await _httpClient.GetAsync(_options.Value.Url, klnQuery);
            var ntConnectHttp = await _httpClient.GetAsync(_options.Value.Url, ntQuery);
            var vangoHkResult =  _json.Dserialize<List<VangoModel>>(hkConnectHttp);
            var vangoKlnResult = _json.Dserialize<List<VangoModel>>(klnConnectHttp);
            var vangoNtResult =  _json.Dserialize<List<VangoModel>>(ntConnectHttp);
            var hkResult =  Parsing(vangoHkResult, "hk");
            var klnResult = Parsing(vangoKlnResult, "kln");
            var ntResult = Parsing(vangoNtResult, "nt");
            List<IGeoComGrabModel> mergeResult = hkResult.Concat(klnResult).Concat(ntResult).ToList();
            var result = await this.GetShopInfo(mergeResult);
            return result;
            // _memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
        }

        public List<IGeoComGrabModel> Parsing(List<VangoModel>? grabResult, string region)
        {
            List<IGeoComGrabModel> VangoIGeoComList = new List<IGeoComGrabModel>();
            if (grabResult != null)
            {
                foreach (var shop in grabResult)
                {
                    IGeoComGrabModel VangoIGeoCom = new IGeoComGrabModel();
                    VangoIGeoCom.ChineseName = $"{shop.store_number}-{shop.storename}";
                    VangoIGeoCom.EnglishName = $"{shop.store_number}-{shop.storename}";
                    VangoIGeoCom.C_Address = shop.address_description.Replace(" ", "");
                    VangoIGeoCom.Latitude = Convert.ToDouble(shop.address_geo_lat);
                    VangoIGeoCom.Longitude = Convert.ToDouble(shop.address_geo_lng);
                    VangoIGeoCom.Class = "CMF";
                    VangoIGeoCom.Type = "CVS";
                    VangoIGeoCom.Source = "27";
                    VangoIGeoCom.Shop = 3;
                    VangoIGeoCom.Web_Site = _options.Value.BaseUrl;
                    VangoIGeoCom.GrabId = $"{shop.store_number}_{shop.storename}{shop.address_geo_lat}";
                    VangoIGeoCom.Tel_No = $"{shop.telephone} {shop.telephone2} {shop.telephone3}";
                    VangoIGeoComList.Add(VangoIGeoCom);
                }
            }
            return VangoIGeoComList;
        }
    }
}
