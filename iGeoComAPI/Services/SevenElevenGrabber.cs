using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;

namespace iGeoComAPI.Services
{
    public partial class SevenElevenGrabber
    {
        //private readonly HttpClient _httpcClient;
        //private readonly IOptions<SevenElevenOptions> _options;
        private readonly ConnectClient _httpClient;
        private readonly SerializeFunction _serializeFunction;   
        private readonly IOptions<SevenElevenOptions> _options;
        private readonly IMemoryCache _memoryCache;

        /*
        public SevenElevenGrabber(HttpClient client, IOptions<SevenElevenOptions> options)
        {
            _client = client;
            _options = options;
        }
        */

        public SevenElevenGrabber(ConnectClient httpClient, SerializeFunction serializeFunction, IOptions<SevenElevenOptions> options, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _serializeFunction = serializeFunction;
            _options = options;
            _memoryCache = memoryCache;
        }
        
        //HttpClient _HttpClient = new HttpClient();
        public async Task<List<IGeoComModel>?> GetWebSiteItems()
        {
            try
            {
                var enConnectHttp = await _httpClient.SendAsync(_options.Value.EnUrl);
                var enSerializedResult = await _serializeFunction.Diserialize<SevenElevenModel>(enConnectHttp);
                var zhConnectHttp = await _httpClient.SendAsync(_options.Value.ZhUrl);
                var zhSerializedResult = await _serializeFunction.Diserialize<SevenElevenModel>(zhConnectHttp);
                var mergeResult = MergeEnAndZh(enSerializedResult, zhSerializedResult);
               // _memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
                return mergeResult;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }

        }

        public List<IGeoComModel> MergeEnAndZh(List<SevenElevenModel> enResult, List<SevenElevenModel> zhResult)
        {
            var _rgx = Regexs.ExtractLagLong();
            List<IGeoComModel> SevenElevenIGeoComList = new List<IGeoComModel>();
            if (enResult != null && zhResult != null)
            {
                foreach (SevenElevenModel shopEn in enResult)
                {
                    IGeoComModel sevenElevenIGeoCom = new IGeoComModel();
                    sevenElevenIGeoCom.E_Address = shopEn.Address?.Replace(",","");
                    sevenElevenIGeoCom.E_Region = shopEn.Region;
                    sevenElevenIGeoCom.E_District = shopEn.District;
                    var matchesEn = _rgx.Matches(shopEn.LatLng!);
                    sevenElevenIGeoCom.Latitude = matchesEn[0].Value;
                    sevenElevenIGeoCom.Longitude = matchesEn[2].Value;
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
                    sevenElevenIGeoCom.Grab_ID = $"seveneleven_{shopEn.Address?.Replace(" ", "").Replace("/", "").Replace(",", "").Replace(".", "")}";
                    sevenElevenIGeoCom.Web_Site = _options.Value.BaseUrl;

                    foreach (SevenElevenModel shopZh in zhResult)
                    {
                        var matchesZh = _rgx.Matches(shopZh.LatLng!);
                        if (matchesZh.Count > 0 && matchesZh != null)
                        {
                            if (sevenElevenIGeoCom.Latitude == matchesZh[0].Value && sevenElevenIGeoCom.Longitude == matchesZh[2].Value)
                            {
                                sevenElevenIGeoCom.C_Address = shopZh.Address?.Replace(",", "");
                                if(shopZh.Region == "Kowloon")
                                {
                                    sevenElevenIGeoCom.C_Region = "九龍";
                                }else if(shopZh.Region == "New Territories")
                                {
                                    sevenElevenIGeoCom.C_Region = "新界";
                                }
                                else
                                {
                                    sevenElevenIGeoCom.C_Region = "香港";
                                }
                                sevenElevenIGeoCom.C_District = shopZh.District;
                                continue;
                            }
                        }
                    }
                    SevenElevenIGeoComList.Add(sevenElevenIGeoCom);

                }
            }
            return SevenElevenIGeoComList.Where(shop => shop.E_Region != "Macau").ToList();
        }
    }
}
