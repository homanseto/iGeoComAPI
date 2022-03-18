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
        public async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var rawDataEng = await _puppeteerConnection.PuppeteerGrabber<string>(_options.Value.EnUrl, infoCode, waitSelector);
            var rawDataZh = await _puppeteerConnection.PuppeteerGrabber<string>(_options.Value.ZhUrl, infoCode, waitSelector);
            var _rgxRegionList = Regexs.ExtractInfo(_regionListRegex);
            var _rgxStoreList = Regexs.ExtractInfo(_storeListRegex);
            string regionStringEng = _rgxRegionList.Match(rawDataEng).Groups[1].Value;
            string storeStringEng = _rgxStoreList.Match(rawDataEng).Groups[1].Value;
            string regionStringZh = _rgxRegionList.Match(rawDataZh).Groups[1].Value;
            string storeStringZh = _rgxStoreList.Match(rawDataZh).Groups[1].Value;
            var enSerializedRegionResult = _json.DserializeSingle<CircleKRegionFilter>(regionStringEng);
            var zhSerializedRegionResult = _json.DserializeSingle<CircleKRegionFilter>(regionStringZh);
            var enSerializedDistrictResult = _json.DserializeSingle<CircleKDistrictFilter>(storeStringEng);
            
            var result = Parsing(enSerializedDistrictResult);

            return result;
        }


        public List<IGeoComGrabModel> Parsing(CircleKDistrictFilter? input)
        {
            List<IGeoComGrabModel> CircleKIGeoComList = new List<IGeoComGrabModel>();
            if(input != null)
            {
                List<List<CircleKModel>> collection = new List<List<CircleKModel>>((IEnumerable<List<CircleKModel>>)input);
                var collection1 = ((IEnumerable)input).Cast<List<CircleKModel>>().ToList();

                foreach (List<CircleKModel> prop in collection)
                {
                    foreach (CircleKModel v in prop)
                    {
                        IGeoComGrabModel circleKIGeoCom = new IGeoComGrabModel();
                        circleKIGeoCom.EnglishName = $"CircleK_{v.store_no}";
                        circleKIGeoCom.E_Address = v.address;
                        circleKIGeoCom.E_District = v.location;
                        if (v.zone == "Hong Kong Island")
                        {
                            circleKIGeoCom.E_Region = "HK";
                        }
                        else if (v.zone == "Kowloon")
                        {
                            circleKIGeoCom.E_Region = "KLN";
                        }
                        else
                        {
                            circleKIGeoCom.E_Region = "NT";
                        }
                        circleKIGeoCom.Latitude = v.latitude;
                        circleKIGeoCom.Longitude = v.longitude;
                        CircleKIGeoComList.Add(circleKIGeoCom);

                    }
                }
            }
            
           return CircleKIGeoComList;
        }


        public List<IGeoComGrabModel> MergeEnAndZh(List<CircleKModel> enResult, List<CircleKModel> zhResult)
        {
            throw new NotImplementedException();
        }
    }


    public class CircleKRegionFilter
    {
        public List<string>? 香港 { get; set; }
        public List<string>? 九龍 { get; set; }
        public List<string>? 新界 { get; set; }

    }

    public class CircleKDistrictFilter
    {   
        public List<CircleKModel>? Wanchai { get; set; }

        public List<CircleKModel>? Aberdeen { get; set; }

    }
}
