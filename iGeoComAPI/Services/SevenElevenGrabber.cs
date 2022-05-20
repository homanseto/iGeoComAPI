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
                var mergeResult = await MergeEnAndZh(enSerializedResult, zhSerializedResult);
                var result =await this.GetNorthEastAndMapInfo(mergeResult);
                // _memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public override List<IGeoComGrabModel> MergeResult(List<SevenElevenModel> enInput, List<SevenElevenModel> zhInput)
        {
            foreach(SevenElevenModel result in enInput)
            {
                Console.WriteLine(result.Address);
            }
            List<IGeoComGrabModel> resultList = new List<IGeoComGrabModel>();
            return resultList;
        }

        public async Task<List<IGeoComGrabModel>> MergeEnAndZh(List<SevenElevenModel>? enResult, List<SevenElevenModel>? zhResult)
        {
            var _rgx = Regexs.ExtractInfo(SevenElevenModel.RegLatLngRegex);
            List<IGeoComGrabModel> SevenElevenIGeoComList = new List<IGeoComGrabModel>();
            try
            {
                _logger.LogMergeEngAndZh(nameof(SevenElevenModel));
                if (enResult != null && zhResult != null)
                {
                    //Parallel.ForEach(enResult, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, (shopEn) =>
                    foreach (SevenElevenModel shopEn in enResult)
                    {
                        IGeoComGrabModel sevenElevenIGeoCom = new IGeoComGrabModel();
                        sevenElevenIGeoCom.E_Address = shopEn.Address;
                        var matchesEn = _rgx.Matches(shopEn.LatLng!);
                        sevenElevenIGeoCom.Latitude = Convert.ToDouble(matchesEn[0].Value);
                        sevenElevenIGeoCom.Longitude = Convert.ToDouble(matchesEn[2].Value);
                        sevenElevenIGeoCom.Class = "CMF";
                        sevenElevenIGeoCom.Type = "CVS";
                        if (shopEn.Opening_24 == "1")
                        {
                            sevenElevenIGeoCom.Subcat = " ";
                        }
                        else
                        {
                            sevenElevenIGeoCom.Subcat = "NON24R";
                        }
                        sevenElevenIGeoCom.GrabId = $"seveneleven_{shopEn.Address?.Replace(" ", "").Replace("/", "").Replace(",", "").Replace(".", "")}";
                        sevenElevenIGeoCom.Web_Site = _options.Value.BaseUrl;

                        foreach (SevenElevenModel shopZh in zhResult)
                        {
                            var matchesZh = _rgx.Matches(shopZh.LatLng!);
                            if (matchesZh.Count > 0 && matchesZh != null)
                            {
                                if (matchesEn[0].Value == matchesZh[0].Value && matchesEn[2].Value == matchesZh[2].Value)
                                {
                                    sevenElevenIGeoCom.C_Address = shopZh.Address.Replace(" ", "");
                                    var cFloor = Regexs.ExtractC_Floor().Matches(sevenElevenIGeoCom.C_Address);
                                    if (cFloor.Count > 0 && cFloor != null)
                                    {
                                        sevenElevenIGeoCom.C_floor = cFloor[0].Value; 
                                    }
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


        //public List<IGeoComGrabModel> FindAdded(List<IGeoComGrabModel> newData, List<IGeoComRepository> previousData)
        //{
        //    int newDataLength = newData.Count;
        //    int previousDataLength = previousData.Count;
        //    List<IGeoComGrabModel> AddedSevenElevenIGeoComList = new List<IGeoComGrabModel>();

        //    for (int i = 0; i < newDataLength; i++)
        //    {
        //        int j;
        //        for (j = 0; j < previousDataLength; j++)
        //            if (newData[i].E_Address?.Replace(",", "").Replace(" ", "") == previousData[j].E_Address?.Replace(",", "").Replace(" ", "") |
        //               newData[i].C_Address?.Replace(",", "").Replace(" ", "") == previousData[j].C_Address?.Replace(",", "").Replace(" ", "")
        //                )
        //                break;
        //        if (j == previousDataLength)
        //        {
        //            AddedSevenElevenIGeoComList.Add(newData[i]);
        //        }
        //    }
        //    return AddedSevenElevenIGeoComList;
        //}
    }
}
