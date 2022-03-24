namespace iGeoComAPI.Services
{
    public class AbstractGrabber
    {
        /*
        private ConnectClient _httpClient;
        private JsonFunction _json;
        private IOptions<SevenElevenOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<SevenElevenGrabber> _logger;
        private string _regLagLngRegex = "([^|]*)";

        public SevenElevenGrabber(ConnectClient httpClient, JsonFunction json, IOptions<SevenElevenOptions> options, IMemoryCache memoryCache, ILogger<SevenElevenGrabber> logger)
        {
            _httpClient = httpClient;
            _json = json;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            try
            {
                _logger.LogInformation("start grabbing 7-11 rowdata");
                var enConnectHttp = await _httpClient.GetAsync(_options.Value.EnUrl);
                var enSerializedResult = _json.Dserialize<SevenElevenModel>(enConnectHttp);
                var zhConnectHttp = await _httpClient.GetAsync(_options.Value.ZhUrl);
                var zhSerializedResult = _json.Dserialize<SevenElevenModel>(zhConnectHttp);
                var mergeResult = MergeEnAndZh(enSerializedResult, zhSerializedResult);
                // _memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
                return mergeResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to grab  7-11");
                throw;
            }

        }
        */
    }
}
