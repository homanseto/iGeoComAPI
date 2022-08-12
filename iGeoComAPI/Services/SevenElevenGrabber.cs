using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using iGeoComAPI.Repository;

namespace iGeoComAPI.Services
{
    public class SevenElevenGrabber : AbstractGrabber
    {
        //private readonly HttpClient _httpcClient;
        //private readonly IOptions<SevenElevenOptions> _options;
        private readonly ConnectClient _httpClient;
        private readonly JsonFunction _json;
        private readonly IOptions<SevenElevenOptions> _options;
        private readonly MyLogger _logger;
        private readonly IGeoComGrabRepository _iGeoComGrabRepository;
        private readonly IDataAccess dataAccess;

        /*
        public SevenElevenGrabber(HttpClient client, IOptions<SevenElevenOptions> options)
        {
            _client = client;
            _options = options;
        }
        */

        public SevenElevenGrabber(ConnectClient httpClient, JsonFunction json, IOptions<SevenElevenOptions> options, MyLogger logger, IOptions<NorthEastOptions> absOptions, IGeoComGrabRepository iGeoComGrabRepository, IDataAccess dataAccess) : base (httpClient, absOptions, json, dataAccess)
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
                _logger.LogStartGrabbing(nameof(SevenElevenGrabber));
                var enConnectHttp = await _httpClient.GetAsync(_options.Value.EnUrl);
                var enSerializedResult =  _json.Dserialize<List<SevenElevenModel>>(enConnectHttp);
                var zhConnectHttp = await _httpClient.GetAsync(_options.Value.ZhUrl);
                var zhSerializedResult = _json.Dserialize<List<SevenElevenModel>>(zhConnectHttp);
                var mergeResult = MergeEnAndZh(enSerializedResult, zhSerializedResult);
                var result =await this.GetShopInfo(mergeResult);
                // _memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<SevenElevenModel>? enResult, List<SevenElevenModel>? zhResult)
        {
            var _rgx = Regexs.ExtractInfo(SevenElevenModel.RegLatLngRegex);
            List<IGeoComGrabModel> SevenElevenIGeoComList = new List<IGeoComGrabModel>();
            try
            {
                _logger.LogMergeEngAndZh(nameof(SevenElevenModel));
                if (enResult != null && zhResult != null)
                {
                    //Parallel.ForEach(enResult, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, (shopEn) =>
                    foreach (var item in enResult.Select((value, i) => new { i, value }))
                    {
                        var shopEn = item.value;
                        var index = item.i;
                        IGeoComGrabModel sevenElevenIGeoCom = new IGeoComGrabModel();
                        sevenElevenIGeoCom.E_Address = shopEn.Address;
                        var matchesEn = _rgx.Matches(shopEn.LatLng!);
                        sevenElevenIGeoCom.Latitude = Convert.ToDouble(matchesEn[0].Value);
                        sevenElevenIGeoCom.Longitude = Convert.ToDouble(matchesEn[2].Value);
                        sevenElevenIGeoCom.Type = "CVS";
                        sevenElevenIGeoCom.Class = "CMF";
                        sevenElevenIGeoCom.E_District = shopEn.District;
                        sevenElevenIGeoCom.ShopId = _options.Value.ShopId;
                        sevenElevenIGeoCom.ChineseName = "7-11便利店";
                        sevenElevenIGeoCom.EnglishName = "7-Eleven";
                        if (shopEn.Opening_24 == "1")
                        {
                            sevenElevenIGeoCom.Subcat = " ";
                        }
                        else
                        {
                            sevenElevenIGeoCom.Subcat = "NON24R";
                        }

                        sevenElevenIGeoCom.Web_Site = _options.Value.BaseUrl;
                        sevenElevenIGeoCom.GrabId = $"{this.GetType().Name.Replace("Grabber", "").ToLower()}{sevenElevenIGeoCom.Latitude}{sevenElevenIGeoCom.Longitude}{shopEn.Opening_Weekday}{shopEn.Daily_Cafe}".Replace("-","").Replace(" ","");
                        foreach (SevenElevenModel shopZh in zhResult)
                        {
                            var matchesZh = _rgx.Matches(shopZh.LatLng!);
                            if (matchesZh.Count > 0 && matchesZh != null)
                            {
                                if (matchesEn[0].Value == matchesZh[0].Value && matchesEn[2].Value == matchesZh[2].Value && shopEn.Opening_Weekday == shopZh.Opening_Weekday && shopEn.Daily_Cafe == shopZh.Daily_Cafe)
                                {
                                    sevenElevenIGeoCom.C_Address = shopZh.Address.Replace(" ", "");
                                    
                                    continue;
                                }
                            }
                        }
                        SevenElevenIGeoComList.Add(sevenElevenIGeoCom);

                    }
                }
               return SevenElevenIGeoComList.Where(shop => shop.E_District.ToLower() != "macau").ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
