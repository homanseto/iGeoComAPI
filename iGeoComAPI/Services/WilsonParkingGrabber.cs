using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using iGeoComAPI.Repository;

namespace iGeoComAPI.Services
{
    public class WilsonParkingGrabber : AbstractGrabber
    {
        //private readonly HttpClient _httpcClient;
        //private readonly IOptions<WilsonParkingOptions> _options;
        private readonly ConnectClient _httpClient;
        private readonly JsonFunction _json;
        private readonly IOptions<WilsonParkingOptions> _options;
        private readonly MyLogger _logger;
        private readonly IGeoComGrabRepository _iGeoComGrabRepository;
        private readonly IDataAccess dataAccess;

        /*
        public WilsonParkingGrabber(HttpClient client, IOptions<WilsonParkingOptions> options)
        {
            _client = client;
            _options = options;
        }
        */

        public WilsonParkingGrabber(ConnectClient httpClient, JsonFunction json, IOptions<WilsonParkingOptions> options, MyLogger logger, IOptions<NorthEastOptions> absOptions, IGeoComGrabRepository iGeoComGrabRepository, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _httpClient = httpClient;
            _json = json;
            _options = options;
            _logger = logger;
            _iGeoComGrabRepository = iGeoComGrabRepository;
        }

        //HttpClient _HttpClient = new HttpClient();
        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            try
            {
                _logger.LogStartGrabbing(nameof(WilsonParkingGrabber));
                var headers = new Dictionary<string, string>()
                {
                    [_options.Value.HeaderKey] = _options.Value.XRequestedWith
                };
                var config = new List<KeyValuePair<string, Dictionary<string, string>>>();
                config.Add(new KeyValuePair<string, Dictionary<string, string>>("headers", headers));
                var connectHttp = await _httpClient.GetResult(_options.Value.Url, config);
                var grabResult = _json.Dserialize<List<WilsonParkingModel>>(connectHttp);
                var filterResult = new List<WilsonParkingModel>();
                if (grabResult != null && grabResult.Count > 0)
                {
                    // 14: hourlyperHour, 18: MaxPark in serviceTypeIds
                    var filter = grabResult.Where(x => x.ServiceTypeIds.Contains("14") || x.ServiceTypeIds.Contains("18"));
                    filterResult = filter.ToList();
                }
                var parsingResult = Parsing(filterResult);
                return parsingResult;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public List<IGeoComGrabModel> Parsing(List<WilsonParkingModel>? grabResult)
        {
            List<IGeoComGrabModel> WilsonParkingIGeoComList = new List<IGeoComGrabModel>();
            if (grabResult != null)
            {
                foreach (var shop in grabResult)
                {
                    IGeoComGrabModel WilsonParkingIGeoCom = new IGeoComGrabModel();
                    if(shop.nameTranslation != null && shop.nameTranslation.Count > 0)
                    {
                        WilsonParkingIGeoCom.EnglishName = shop.nameTranslation[0].content;
                        WilsonParkingIGeoCom.ChineseName = shop.nameTranslation[1].content;
                    }
                    if(shop.addressTranslation != null && shop.addressTranslation.Count > 0)
                    {
                        WilsonParkingIGeoCom.E_Address = shop.addressTranslation[0].content;
                        WilsonParkingIGeoCom.C_Address = shop.addressTranslation[1].content;
                        
                    }
                    if(shop.carPark != null)
                    {
                        WilsonParkingIGeoCom.Latitude = shop.carPark.latitude;
                        WilsonParkingIGeoCom.Longitude = shop.carPark.longitude;
                        WilsonParkingIGeoCom.GrabId = $"wilison{shop.carPark.id}";
                    }
                    WilsonParkingIGeoCom.Type = "CPO";
                    WilsonParkingIGeoCom.Class = "TRS";
                    WilsonParkingIGeoCom.Source = "27";
                    WilsonParkingIGeoCom.Web_Site = _options.Value.BaseUrl;
                    WilsonParkingIGeoComList.Add(WilsonParkingIGeoCom);
                }
            }
            return WilsonParkingIGeoComList;
        }
    }
}
