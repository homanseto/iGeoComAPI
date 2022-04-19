using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Reflection;

namespace iGeoComAPI.Services
{
    public class CircleKGrabber 
    {
        private PuppeteerConnection _puppeteerConnection;
        private JsonFunction _json;
        private IOptions<CircleKOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<CircleKGrabber> _logger;
        private string infoCode = @"() =>{
                                 const selector = document.querySelector('td.ff_parent_table > script').textContent;
                                 return selector;
                                 }";
        private string waitSelector = "td.ff_parent_table";
        private string _regionListRegex = @"\s* region_list = jQuery\.parseJSON\('(.*)'\)";
        private string _storeListRegex = @"\s* store_list = jQuery\.parseJSON\('(.*)'\);";


        public CircleKGrabber(PuppeteerConnection puppeteerConnection, JsonFunction json, IOptions<CircleKOptions> options, IMemoryCache memoryCache, ILogger<CircleKGrabber> logger)
        {
            _puppeteerConnection = puppeteerConnection;
            _json = json;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        public async Task GetWebSiteItems()
        {
            var rawDataEng = await _puppeteerConnection.PuppeteerGrabber<string>(_options.Value.EnUrl, infoCode, waitSelector);
            var rawDataZh = await _puppeteerConnection.PuppeteerGrabber<string>(_options.Value.ZhUrl, infoCode, waitSelector);
            var _rgxRegionList = Regexs.ExtractInfo(_regionListRegex);
            var _rgxStoreList = Regexs.ExtractInfo(_storeListRegex);
            string regionStringEng = _rgxRegionList.Match(rawDataEng).Groups[1].Value;
            string storeStringEng = _rgxStoreList.Match(rawDataEng).Groups[1].Value;
            string regionStringZh = _rgxRegionList.Match(rawDataZh).Groups[1].Value;
            string storeStringZh = _rgxStoreList.Match(rawDataZh).Groups[1].Value;
            var enSerializedRegionResult = _json.Dserialize<Dictionary<string, List<string>>>(regionStringEng);
            var zhSerializedRegionResult = _json.Dserialize<Dictionary<string, List<string>>>(regionStringZh);
            var enSerializedDistrictResult = _json.Dserialize<Dictionary<string, List<CircleKModel>>>(storeStringEng);
            var zhSerializedDistrictResult = _json.Dserialize<Dictionary<string, List<CircleKModel>>>(storeStringZh);
            MergeEnAndZh(enSerializedDistrictResult, zhSerializedDistrictResult);

        }

        //public async Task Testing(CircleKRegionFilter input)
        //{
        //    IEnumerable<CircleKRegionFilter> en = input;
        //    foreach(var item in en)
        //    {
        //        Console.WriteLine(item);
        //    }
        //}

        public void MergeEnAndZh(Dictionary<string, List<CircleKModel>> emResult, Dictionary<string, List<CircleKModel>> zhResult)
        {
            List<IGeoComGrabModel> CircleKList = new List<IGeoComGrabModel>();
            foreach (KeyValuePair<string, List<CircleKModel>> enEntry in emResult)
            {
                foreach (var en in enEntry.Value)
                {
                    IGeoComGrabModel circleK = new IGeoComGrabModel();
                    circleK.E_District = en.location;
                    circleK.Latitude = Convert.ToDouble(en.latitude);
                    circleK.Longitude = Convert.ToDouble(en.longitude);
                    circleK.Grab_ID = $"circleK_{en.store_no}";
                    circleK.E_Address = en.address;
                    foreach (KeyValuePair<string, List<CircleKModel>> zhEntry in zhResult)
                    {
                        foreach (var zh in zhEntry.Value)
                        {
                            if (en.longitude == zh.longitude && en.latitude == zh.latitude)
                            {
                                circleK.C_District = zh.location;
                                circleK.C_Address = zh.address;
                               
                            }
                        }
                    }
                    CircleKList.Add(circleK);
                }
            }
        }


    }



}
