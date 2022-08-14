using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;

namespace iGeoComAPI.Services
{
    public class ParknShopGrabber : AbstractGrabber
    {
        private ConnectClient _httpClient;
        private JsonFunction _json;
        private IOptions<ParknShopOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<ParknShopGrabber> _logger;
        private readonly IDataAccess dataAccess;

        public ParknShopGrabber(ConnectClient httpClient, JsonFunction json, IOptions<ParknShopOptions> options, IMemoryCache memoryCache, ILogger<ParknShopGrabber> logger, IOptions<NorthEastOptions> absOptions, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _httpClient = httpClient;
            _json = json;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        StringComparison comp = StringComparison.OrdinalIgnoreCase;
        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            _logger.LogInformation("start grabbing ParknShop rowdata");
            string num = "0";
            int totalPage = 0;
            var totalConnectHttp = await _httpClient.GetAsync(String.Format(_options.Value.EnUrl, num));
            var totalSerializedResult = _json.Dserialize<ParknShopModel>(totalConnectHttp);
            if (totalSerializedResult != null)
            {
                totalPage = totalSerializedResult.pagination.totalPages;
            }
            var enResult = await Parsing(_options.Value.EnUrl, totalPage);
            var zhResult = await Parsing(_options.Value.ZhUrl, totalPage);
            var mergeResult = MergeEnAndZh(enResult, zhResult);
            // _memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
            return mergeResult;
        }

        public async Task<List<IGeoComGrabModel>> Parsing(string url, int totalPage)
        {
            try
            {
                List<IGeoComGrabModel> parknshopIGeoComList = new List<IGeoComGrabModel>();
                for (int i = 0; i < totalPage; i++)
                {
                    var connectHttp = await _httpClient.GetAsync(String.Format(url, i));
                    var serializedResult = _json.Dserialize<ParknShopModel>(connectHttp);
                    if (serializedResult != null)
                    {
                        var stores = serializedResult.stores;
                        foreach (var store in stores)
                        {
                            if (store != null)
                            {
                                if (store.address.country.isocode == "MO")
                                {
                                    continue;
                                }
                                else
                                {
                                    var ParknShopIGeoCom = new IGeoComGrabModel();
                                    if (url.Contains("lang=en_HK", comp))
                                    {
                                        ParknShopIGeoCom.E_Address = store.address.displayAddress1;
                                        ParknShopIGeoCom.EnglishName = $"{store.storeContent} {store.displayName}";
                                    }
                                    else
                                    {
                                        ParknShopIGeoCom.C_Address = store.address.displayAddress1;
                                        ParknShopIGeoCom.ChineseName = $"{store.storeContent} {store.displayName}";
                                    }
                                    ParknShopIGeoCom.ShopId = "smk1";
                                    ParknShopIGeoCom.GrabId = $"parknshop_{store.elabStoreNumber}";
                                    ParknShopIGeoCom.Latitude = store.geoPoint.latitude;
                                    ParknShopIGeoCom.Longitude = store.geoPoint.longitude;
                                    ParknShopIGeoCom.Tel_No = store.address.phone;

                                    parknshopIGeoComList.Add(ParknShopIGeoCom);
                                }
                            }
                        }
                    }
                }
                return parknshopIGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public List<IGeoComGrabModel> MergeEnAndZh(List<IGeoComGrabModel> enResult, List<IGeoComGrabModel> zhResult)
        {
            var mergedList = enResult;
            if (enResult != null && zhResult != null)
            {
                foreach (IGeoComGrabModel en in mergedList)
                {
                    foreach (IGeoComGrabModel zh in zhResult)
                    {
                        if (en.GrabId == zh.GrabId)
                        {
                            en.ChineseName = zh.ChineseName;
                            en.C_Address = zh.C_Address;
                            break;
                        }
                    }
                }
            }
            return mergedList;
        }
    }
}
