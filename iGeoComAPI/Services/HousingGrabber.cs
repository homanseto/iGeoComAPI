using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;

namespace iGeoComAPI.Services
{
    public class HousingGrabber : AbstractGrabber
    {
        private ConnectClient _httpClient;
        private JsonFunction _json;
        private IOptions<HousingOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<HousingGrabber> _logger;
        private readonly IDataAccess dataAccess;

        public HousingGrabber(ConnectClient httpClient, JsonFunction json, IOptions<HousingOptions> options, IMemoryCache memoryCache, ILogger<HousingGrabber> logger, IOptions<NorthEastOptions> absOptions, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _httpClient = httpClient;
            _json = json;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }


        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            _logger.LogInformation("start grabbing Housing rowdata");
            var enConnectHttp = await _httpClient.GetAsync(_options.Value.Url);
            var enSerializedResult = _json.Dserialize<HousingModel>(enConnectHttp);
            // _memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
            var paringResult = Parsing(enSerializedResult);
            var result = await this.GetShopInfo(paringResult);
            return result;
        }

        public List<IGeoComGrabModel> Parsing(HousingModel grabResult)
        {
            try
            {
                List<IGeoComGrabModel> HousingIGeoComList = new List<IGeoComGrabModel>();
                if (grabResult != null)
                {
                    if (grabResult.regionArray != null && grabResult.regionArray.Count > 0)
                    {
                        foreach (var districtList in grabResult.regionArray)
                        {
                            if (districtList.district != null && districtList.district.Count > 0)
                            {
                                foreach (var estate in districtList.district)
                                {
                                    if (estate.estates != null && estate.estates.Count > 0)
                                    {
                                        foreach (var shop in estate.estates)
                                        {

                                            IGeoComGrabModel HousingIGeoCom = new IGeoComGrabModel();
                                            if (shop.name != null && districtList.area != null && estate.name != null && shop.center != null)
                                            {
                                                HousingIGeoCom.ChineseName = shop.name.ZhHant;
                                                HousingIGeoCom.EnglishName = shop.name.en;
                                                HousingIGeoCom.C_Address = $"{shop.name.ZhHant}{districtList.area.ZhHant}{estate.name.ZhHant}";
                                                HousingIGeoCom.E_Address = $"{shop.name.en}{districtList.area.en}{estate.name.en}";
                                                HousingIGeoCom.GrabId = $"Housing_${shop.id}";
                                                HousingIGeoCom.Latitude = shop.center[0];
                                                HousingIGeoCom.Longitude = shop.center[1];
                                                HousingIGeoComList.Add(HousingIGeoCom);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                return HousingIGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }
    }
}
