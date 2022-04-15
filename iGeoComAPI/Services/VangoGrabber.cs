using iGeoComAPI.Models;
using iGeoComAPI.Options;
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

        public VangoGrabber(ConnectClient httpClient, JsonFunction json, IOptions<VangoOptions> options, IMemoryCache memoryCache, ILogger<VangoGrabber> logger, IOptions<NorthEastOptions> absOptions) :base(httpClient, absOptions, json)
        {
            _httpClient = httpClient;
            _json = json;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
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
            var hkResult = await Parsing(vangoHkResult, "hk");
            var klnResult = await Parsing(vangoKlnResult, "kln");
            var ntResult = await Parsing(vangoNtResult, "nt");
            List<IGeoComGrabModel> VangoResult = hkResult.Concat(klnResult).Concat(ntResult).ToList();
            return VangoResult;
            // _memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
        }

        public async Task<List<IGeoComGrabModel>> Parsing(List<VangoModel>? grabResult, string region)
        {
            List<IGeoComGrabModel> VangoIGeoComList = new List<IGeoComGrabModel>();
            if (grabResult != null)
            {
                foreach (var shop in grabResult)
                {
                    IGeoComGrabModel VangoIGeoCom = new IGeoComGrabModel();
                    VangoIGeoCom.ChineseName = $"{shop.store_number}-{shop.storename}";
                    VangoIGeoCom.EnglishName = $"{shop.store_number}-{shop.storename}";
                    VangoIGeoCom.C_Address = shop.address_description;
                    VangoIGeoCom.Latitude = Convert.ToDouble(shop.address_geo_lat);
                    VangoIGeoCom.Longitude = Convert.ToDouble(shop.address_geo_lng);
                    NorthEastModel eastNorth = await this.getNorthEastNorth(VangoIGeoCom.Latitude, VangoIGeoCom.Longitude);
                    if(eastNorth != null)
                    {
                        VangoIGeoCom.Northing = eastNorth.hkN;
                        VangoIGeoCom.Easting = eastNorth.hkE;
                    }
                    VangoIGeoCom.Class = "CMF";
                    VangoIGeoCom.Type = "CVS";
                    VangoIGeoCom.Source = "27";
                    VangoIGeoCom.Web_Site = _options.Value.BaseUrl;
                    VangoIGeoCom.Grab_ID = $"{shop.store_number}_{shop.storename}{shop.address_geo_lat}";
                    if (region != null & region == "hk")
                    {
                        VangoIGeoCom.C_Region = "香港";
                        VangoIGeoCom.E_Region = "HK";
                    }
                    else if (region != null & region == "kln")
                    {
                        VangoIGeoCom.C_Region = "九龍";
                        VangoIGeoCom.E_Region = "KLN";
                    }
                    else
                    {
                        VangoIGeoCom.C_Region = "新界";
                        VangoIGeoCom.E_Region = "NT";
                    }
                    VangoIGeoCom.Tel_No = $"{shop.telephone} {shop.telephone2} {shop.telephone3}";
                    VangoIGeoComList.Add(VangoIGeoCom);
                }
            }
            return VangoIGeoComList;
        }
    }
}
