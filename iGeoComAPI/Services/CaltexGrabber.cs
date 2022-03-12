using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;

namespace iGeoComAPI.Services
{
    public class CaltexGrabber : IGrabberAPI<CaltexModel>
    {
        private ConnectClient _httpClient;
        private JSON _json;
        private IOptions<CaltexOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<CaltexGrabber> _logger;


        public CaltexGrabber(ConnectClient httpClient, JSON json, IOptions<CaltexOptions> options, IMemoryCache memoryCache, ILogger<CaltexGrabber> logger)
        {
            _httpClient = httpClient;
            _json = json;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        public async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            _logger.LogInformation("start grabbing 7-11 rowdata");
            var enConnectHttp = await _httpClient.GetAsync(_options.Value.EnUrl);
            var enSerializedResult = await _json.Diserialize<CaltexModel>(enConnectHttp);
            var zhConnectHttp = await _httpClient.GetAsync(_options.Value.ZhUrl);
            var zhSerializedResult = await _json.Diserialize<CaltexModel>(zhConnectHttp);
            var mergeResult = MergeEnAndZh(enSerializedResult, zhSerializedResult);
            // _memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
            return mergeResult;
        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<CaltexModel>? enResult, List<CaltexModel>? zhResult)
        {
            List<IGeoComGrabModel> CaltexIGeoComList = new List<IGeoComGrabModel>();
            try
            {
                _logger.LogInformation("Start merging Caltex eng and Zh");
                if (enResult != null && zhResult != null)
                {
                    foreach (var en in enResult)
                    {
                        IGeoComGrabModel CaltexIGeoCom = new IGeoComGrabModel();
                        CaltexIGeoCom.Grab_ID = $"caltex_{en.Id}";
                        CaltexIGeoCom.EnglishName = $"Caltex-{en.Name!.Trim()}";
                        CaltexIGeoCom.E_Address = en.Street!.Trim();
                        CaltexIGeoCom.Tel_No = en.PhoneNumber!.Replace(" ", "");
                        CaltexIGeoCom.Latitude = en.Latitude!.Trim();
                        CaltexIGeoCom.Longitude = en.Longitude!.Trim();
                        CaltexIGeoCom.Web_Site = _options.Value.BaseUrl;
                        CaltexIGeoCom.Class = "UTI";
                        CaltexIGeoCom.Type = "PFS";
                        CaltexIGeoCom.Source = "27";
                        foreach (var zh in zhResult)
                        {
                            if (en.Id == zh.Id)
                            {
                                CaltexIGeoCom.ChineseName = $"加德士-{zh.Name!.Trim()}";
                                CaltexIGeoCom.C_Address = zh.Street!.Trim().Replace(" ", "");
                                continue;
                            }
                        }
                        CaltexIGeoComList.Add(CaltexIGeoCom);
                    }
                }
                return CaltexIGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge Catlex Eng and Zh RawData");
                throw;
            }
        }
    }
}
