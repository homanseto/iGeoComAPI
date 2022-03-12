using iGeoComAPI.Models;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class USelectGrabber
    {
        private ConnectClient _httpClient;
        private JSON _json;
        private IOptions<USelectOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<USelectGrabber> _logger;

        public USelectGrabber(ConnectClient httpClient, JSON json, IOptions<USelectOptions> options, IMemoryCache memoryCache, ILogger<USelectGrabber> logger)
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
            var selectConnectHttp = await _httpClient.GetAsync(_options.Value.ShopAPI, $"regionID={_options.Value.select}");
            var selectFoodConnectHttp = await _httpClient.GetAsync(_options.Value.ShopAPI, $"regionID={_options.Value.selectFood}");
            var selectMiniConnectHttp = await _httpClient.GetAsync(_options.Value.ShopAPI, $"regionID={_options.Value.selectMini}");
            var selectResult = await _json.Diserialize<USelectModel>(selectConnectHttp);
            var selectFoodResult = await _json.Diserialize<USelectModel>(selectFoodConnectHttp);
            var selectMiniResult = await _json.Diserialize<USelectModel>(selectMiniConnectHttp);
            var hkResult = Parsing(selectResult);
            var klnResult = Parsing(selectFoodResult);
            var ntResult = Parsing(selectMiniResult);
            List<IGeoComGrabModel> VangoResult = hkResult.Concat(klnResult).Concat(ntResult).ToList();
            return VangoResult;
            // _memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
        }

        public List<IGeoComGrabModel> Parsing(List<USelectModel> grabResult)
        {
            List<IGeoComGrabModel> USelectIGeoComList = new List<IGeoComGrabModel>();
            if (grabResult != null)
            {
                foreach (var shop in grabResult)
                {
                    IGeoComGrabModel USelectIGeoCom = new IGeoComGrabModel();
                    USelectIGeoCom.ChineseName = $"{shop.store_number}-{shop.storename}";
                    USelectIGeoCom.EnglishName = $"{shop.store_number}-{shop.storename_en}";
                    USelectIGeoCom.C_Address = shop.address_description;
                    USelectIGeoCom.E_Address = shop.address_description_en;
                    USelectIGeoCom.Latitude = shop.address_geo_lat;
                    USelectIGeoCom.Longitude = shop.address_geo_lng; ;
                    USelectIGeoCom.Class = "CMF";
                    USelectIGeoCom.Type = "SMK";
                    USelectIGeoCom.Grab_ID = $"{shop.store_number}_{shop.storename}{shop.address_geo_lat}";
                    USelectIGeoCom.Tel_No = $"{shop.telephone} {shop.telephone2} {shop.telephone3}";
                    USelectIGeoComList.Add(USelectIGeoCom);
                }
            }
            return USelectIGeoComList;
        }
    }
}
