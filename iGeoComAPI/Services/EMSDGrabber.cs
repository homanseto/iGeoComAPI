using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace iGeoComAPI.Services
{
    public class EMSDGrabber : AbstractGrabber
    {
        private ConnectClient _httpClient;
        private JsonFunction _json;
        private IOptions<EMSDOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<EMSDGrabber> _logger;
        private readonly IDataAccess dataAccess;

        public EMSDGrabber(ConnectClient httpClient, JsonFunction json, IOptions<EMSDOptions> options, IMemoryCache memoryCache, ILogger<EMSDGrabber> logger, IOptions<NorthEastOptions> absOptions, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _httpClient = httpClient;
            _json = json;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enHttpRecords = await _httpClient.GetAsync(_options.Value.EnUrl);
            var enResult = discodeInfo(enHttpRecords);
            var zhHttpRecords = await _httpClient.GetAsync(_options.Value.ZhUrl);
            var zhResult = discodeInfo(zhHttpRecords);
            var mergeResult = MergeEnAndZh(enResult, zhResult);
            var result = await this.GetShopInfo(mergeResult);
            return result;
        }

        public List<EMSDModel> discodeInfo(string httpRecords)
        {
            var rgx = Regexs.ExtractInfo(EMSDModel.records);
            string records = rgx.Match(httpRecords).Groups[1].Value;
            List<string> extractRecords = records.Split("\"]").ToList();
            extractRecords.RemoveAt(extractRecords.Count - 1);
            List<EMSDModel> EMSDGrab = new List<EMSDModel>();
            foreach (var record in extractRecords)
            {
                EMSDModel eMSDModel = new EMSDModel();
                string recordString = record.ToString();
                recordString = recordString.Substring(1);
                recordString += "\",";
                List<string> aList = recordString.Split("\",").ToList();
                eMSDModel.id = aList[1].Replace("\"", "");
                eMSDModel.Brand = aList[2].Replace("\"", "");
                eMSDModel.Address = aList[3].Replace("\"", "");
                eMSDModel.Number = aList[4].Replace("\"", "");
                eMSDModel.Latitude = Convert.ToDouble(aList[6].Replace("\"", ""));
                eMSDModel.Longitude = Convert.ToDouble(aList[7].Replace("\"", ""));
                EMSDGrab.Add(eMSDModel);
            }
            return EMSDGrab;
        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<EMSDModel> enResult, List<EMSDModel> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge EMSD En and Zh");
                List<IGeoComGrabModel> EMSDIGeoComList = new List<IGeoComGrabModel>();
                foreach (var item in enResult.Select((value, i) => new { i, value }))
                {
                    var shopEn = item.value;
                    var index = item.i;
                    IGeoComGrabModel EMSDIGeoCom = new IGeoComGrabModel();
                    EMSDIGeoCom.E_Address = shopEn.Address!;
                    EMSDIGeoCom.EnglishName = $"{shopEn.Brand} Liquefied Petroleum Gas Filling Station";
                    EMSDIGeoCom.Latitude = shopEn.Latitude;
                    EMSDIGeoCom.Longitude = shopEn.Longitude;
                    EMSDIGeoCom.Tel_No = shopEn.Number;
                    EMSDIGeoCom.Web_Site = _options.Value.BaseUrl!;
                    EMSDIGeoCom.Class = "UTI";
                    EMSDIGeoCom.Type = "LPG";
                    EMSDIGeoCom.GrabId = $"EMSD {shopEn.id}";
                    foreach (var item2 in zhResult.Select((value2, i2) => new { i2, value2 }))
                    {
                        var shopZh = item2.value2;
                        var index2 = item2.i2;
                        if (shopEn.id == shopZh.id)
                        {
                            EMSDIGeoCom.C_Address = shopZh.Address!.Replace(" ", "");
                            EMSDIGeoCom.ChineseName = $"{shopZh.Brand} 加氣站";
                            continue;
                        }

                    }
                    EMSDIGeoComList.Add(EMSDIGeoCom);
                }
                return EMSDIGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge EMSD En and Zh");
                throw;
            }
        }
    }
}
