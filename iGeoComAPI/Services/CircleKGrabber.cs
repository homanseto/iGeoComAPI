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
    public class CircleKGrabber : AbstractGrabber
    {
        private PuppeteerConnection _puppeteerConnection;
        private JsonFunction _json;
        private IOptions<CircleKOptions> _options;
        private ILogger<CircleKGrabber> _logger;
        private readonly IDataAccess dataAccess;

        private string infoCode = @"() =>{
                                 const selector = document.querySelector('td.ff_parent_table > script').textContent;
                                 return selector;
                                 }";
        private string waitSelector = "td.ff_parent_table";



        public CircleKGrabber(PuppeteerConnection puppeteerConnection, IOptions<CircleKOptions> options, ILogger<CircleKGrabber> logger,
            IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _json = json;
            _options = options;
            _logger = logger;
        }

        public override async Task<List<IGeoComGrabModel>> GetWebSiteItems()
        {
            var rawDataEng = await _puppeteerConnection.PuppeteerGrabber<string>(_options.Value.EnUrl, infoCode, waitSelector);
            var rawDataZh = await _puppeteerConnection.PuppeteerGrabber<string>(_options.Value.ZhUrl, infoCode, waitSelector);
            //var _rgxRegionList = Regexs.ExtractInfo(_regionListRegex);
            var _rgxStoreList = Regexs.ExtractInfo(CircleKModel.storeListRegex);
            //string regionStringEng = _rgxRegionList.Match(rawDataEng).Groups[1].Value;
            string storeStringEng = _rgxStoreList.Match(rawDataEng).Groups[1].Value;
            //string regionStringZh = _rgxRegionList.Match(rawDataZh).Groups[1].Value;
            string storeStringZh = _rgxStoreList.Match(rawDataZh).Groups[1].Value;
            var enSerializedDistrictResult = _json.Dserialize<Dictionary<string, List<CircleKModel>>>(storeStringEng);
            var zhSerializedDistrictResult = _json.Dserialize<Dictionary<string, List<CircleKModel>>>(storeStringZh);
            var mergeResult = MergeEnAndZhAsync(enSerializedDistrictResult, zhSerializedDistrictResult);
            var result = await this.GetShopInfo(mergeResult);
            return result;
        }

        //public async Task Testing(CircleKRegionFilter input)
        //{
        //    IEnumerable<CircleKRegionFilter> en = input;
        //    foreach(var item in en)
        //    {
        //        Console.WriteLine(item);
        //    }
        //}

        public  List<IGeoComGrabModel> MergeEnAndZhAsync(Dictionary<string, List<CircleKModel>> emResult, Dictionary<string, List<CircleKModel>> zhResult)
        {
            List<IGeoComGrabModel> CircleKIGeoComList = new List<IGeoComGrabModel>();
            foreach (KeyValuePair<string, List<CircleKModel>> enEntry in emResult)
            {
                foreach (var en in enEntry.Value)
                {
                    IGeoComGrabModel circleKIGeoCom = new IGeoComGrabModel();
                    circleKIGeoCom.E_Region = en.zone;
                    circleKIGeoCom.Latitude = Convert.ToDouble(en.latitude);
                    circleKIGeoCom.Longitude = Convert.ToDouble(en.longitude);
                    circleKIGeoCom.Shop = 2;
                    circleKIGeoCom.Type = "CVS";
                    circleKIGeoCom.Class = "CMF";
                    circleKIGeoCom.GrabId = $"circleK_{en.store_no}";
                    circleKIGeoCom.E_Address = en.address;
                    if(en.operation_hour.ToLower() == "24 hours")
                    {
                        circleKIGeoCom.Subcat = " ";
                    }
                    else
                    {
                        circleKIGeoCom.Subcat = "NON24R";
                    }
                    circleKIGeoCom.Class = "CMF";
                    circleKIGeoCom.Type = "CVS";
                    foreach (KeyValuePair<string, List<CircleKModel>> zhEntry in zhResult)
                    {
                        foreach (var zh in zhEntry.Value)
                        {
                            if (en.store_no == zh.store_no)
                            {
                                circleKIGeoCom.C_Address = zh.address.Replace(" ", "");
                               
                            }
                        }
                    }
                    CircleKIGeoComList.Add(circleKIGeoCom);
                }
            }
             return CircleKIGeoComList.Where(shop => (shop.E_Region.ToLower() != "macau") && (shop.E_Region.ToLower() != "zhuhai")).ToList();
        }
    }



}
