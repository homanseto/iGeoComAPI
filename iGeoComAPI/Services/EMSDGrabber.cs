using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace iGeoComAPI.Services
{
    public class EMSDGrabber: AbstractGrabber
    {
        private ConnectClient _httpClient;
        private JsonFunction _json;
        private IOptions<ShellOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<ShellGrabber> _logger;
        private readonly IDataAccess dataAccess;

        public EMSDGrabber(ConnectClient httpClient, JsonFunction json, IOptions<ShellOptions> options, IMemoryCache memoryCache, ILogger<ShellGrabber> logger, IOptions<NorthEastOptions> absOptions, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _httpClient = httpClient;
            _json = json;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {

            List<IGeoComGrabModel> temp = new List<IGeoComGrabModel>();
            var enConnectHttp = await _httpClient.GetAsync("https://www.emsd.gov.hk/filemanager/LPGFillingStation/en/list_LPGFillingStation_1343.js");
            var rgx = Regexs.ExtractInfo(EMSDModel.records);
            string records = rgx.Match(enConnectHttp).Groups[1].Value;
            List<string> extractRecords = records.Split("\"]").ToList();
            extractRecords.RemoveAt(extractRecords.Count - 1);
            foreach(var record in extractRecords)
            {
                string recordString = record.ToString();
                recordString = recordString.Substring(1);
                recordString += "\",";
                List<string> aList = recordString.Split("\",").ToList();
                Console.WriteLine(aList);
            }
            return temp;
        }
    }
}
