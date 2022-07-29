using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;

namespace iGeoComAPI.Services
{
    public class ShellGrabber : AbstractGrabber
    {
        private ConnectClient _httpClient;
        private JsonFunction _json;
        private IOptions<ShellOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<ShellGrabber> _logger;
        private readonly IDataAccess dataAccess;

        public ShellGrabber(ConnectClient httpClient, JsonFunction json, IOptions<ShellOptions> options, IMemoryCache memoryCache, ILogger<ShellGrabber> logger, IOptions<NorthEastOptions> absOptions, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _httpClient = httpClient;
            _json = json;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            _logger.LogInformation("start grabbing Shell rowdata");
            var query = new Dictionary<string, string>()
            {
                ["lat"] = _options.Value.lat.ToString(),
                ["lng"] = _options.Value.lng.ToString(),
                ["autoload"] = _options.Value.autoload.ToString(),
                ["travel_mode"] = _options.Value.travel_mode,
                ["avoid_tolls"] = _options.Value.avoid_tolls.ToString(),
                ["avoid_highways"] = _options.Value.avoid_highways.ToString(),
                ["avoid_ferries"] = _options.Value.avoid_ferries.ToString(),
                ["corridor_radius"] = _options.Value.corridor_radius.ToString(),
                ["driving_distance"] = _options.Value.driving_distance.ToString(),
                ["format"] = _options.Value.format.ToString(),
            };

            var enConnectHttp = await _httpClient.GetAsync(_options.Value.Url, query);
            var enSerializedResult = _json.Dserialize<List<ShellModel>>(enConnectHttp);
            var shellResult = new List<IGeoComGrabModel>();
            if (enSerializedResult != null && enSerializedResult.Count > 0)
            {
                var mergeResult = Parsing(enSerializedResult);
                var result = await this.GetShopInfo(mergeResult);
                shellResult = result;
            }
            return shellResult;
        }

        public List<IGeoComGrabModel> Parsing(List<ShellModel> grabResult)
        {
            List<IGeoComGrabModel> shellIGeoComList = new List<IGeoComGrabModel>();
            if(grabResult != null)
            {
                foreach(var item in grabResult)
                {
                    IGeoComGrabModel shellIGeoCom = new IGeoComGrabModel();
                    shellIGeoCom.GrabId = item.id;
                    shellIGeoCom.E_Address = item.address;
                    shellIGeoCom.Latitude = Convert.ToDouble(item.lat);
                    shellIGeoCom.Longitude = Convert.ToDouble(item.lng);
                    shellIGeoCom.Web_Site = _options.Value.BaseUrl;
                    shellIGeoCom.Tel_No = item.telephone;
                    shellIGeoCom.EnglishName = item.name;
                    shellIGeoCom.Class = "UTI";
                    shellIGeoCom.Type = "PFS";
                    shellIGeoComList.Add(shellIGeoCom);
                }
            }
            return shellIGeoComList;
        }
    }
}
