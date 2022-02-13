using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;

namespace iGeoComAPI.Services
{
    public partial class SevenElevenGrabber
    {
        //private readonly HttpClient _httpcClient;
        //private readonly IOptions<SevenElevenOptions> _options;
        private readonly ConnectClient _httpClient;
        private readonly SerializeFunction _serializeFunction;   
        private readonly Regexs _regexs;
        private readonly IOptions<SevenElevenOptions> _options;

        /*
        public SevenElevenGrabber(HttpClient client, IOptions<SevenElevenOptions> options)
        {
            _client = client;
            _options = options;
        }
        */
        
        public SevenElevenGrabber(ConnectClient httpClient, SerializeFunction serializeFunction, Regexs regexs, IOptions<SevenElevenOptions> options)
        {
            _httpClient = httpClient;
            _serializeFunction = serializeFunction;
            _regexs = regexs;
            _options = options;
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
                var result = MergeEnAndZh(enSerializedResult, zhSerializedResult);

               // _dataAccess.SaveGrabbedData("INSERT INTO igeocomtable VALUES (@GEONAMEID,@ENGLISHNAME,@CHINESENAME,@ClASS,@TYPE, @SUBCAT,@EASTING,@NORTHING,@SOURCE,@E_FLOOR,@C_FLOOR,@E_SITENAME,@C_SITENAME,@E_AREA,@C_AREA,@E_DISTRICT,@C_DISTRICT,@E_REGION,@C_REGION,@E_ADDRESS,@C_ADDRESS,@TEL_NO,@FAX_NO,@WEB_SITE,@REV_DATE,@GRAB_ID,@Latitude,@Longitude)", result, "Server=127.0.0.1;Port=3306;database=igeocom; user id=root; password=YouMeK100");

                return result;

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }

        }

        public List<IGeoComModel> MergeEnAndZh(List<SevenElevenModel> enResult, List<SevenElevenModel> zhResult)
        {
            var _rgx = _regexs.ExtractLagLong();
            List<IGeoComModel> SevenElevenIGeoComList = new List<IGeoComModel>();
            if (enResult != null && zhResult != null)
            {
                foreach (SevenElevenModel shopEn in enResult)
                {
                    IGeoComModel sevenElevenIGeoCom = new IGeoComModel();
                    sevenElevenIGeoCom.E_Address = shopEn.Address;
                    sevenElevenIGeoCom.E_Region = shopEn.Region;
                    sevenElevenIGeoCom.E_District = shopEn.District;
                    var matchesEn = _rgx.Matches(shopEn.LatLng!);
                    sevenElevenIGeoCom.Latitude = matchesEn[0].Value;
                    sevenElevenIGeoCom.Longitude = matchesEn[2].Value;
                    if (shopEn.Opening_24 == "1")
                    {
                        sevenElevenIGeoCom.Subcat = "true";
                    }
                    else
                    {
                        sevenElevenIGeoCom.Subcat = "false";
                    }
                    sevenElevenIGeoCom.Grab_ID = $"sevenEleven{shopEn.LatLng}";
                    sevenElevenIGeoCom.Web_Site = _options.Value.BaseUrl;

                    foreach (SevenElevenModel shopZh in zhResult)
                    {
                        var matchesZh = _rgx.Matches(shopZh.LatLng!);
                        if (matchesZh.Count > 0 && matchesZh != null)
                        {
                            if (sevenElevenIGeoCom.Latitude == matchesZh[0].Value && sevenElevenIGeoCom.Longitude == matchesZh[2].Value)
                            {
                                sevenElevenIGeoCom.C_Address = shopZh.Address;
                                sevenElevenIGeoCom.C_Region = shopZh.Region;
                                sevenElevenIGeoCom.C_District = shopZh.District;
                                continue;
                            }
                        }
                    }
                    SevenElevenIGeoComList.Add(sevenElevenIGeoCom);

                }
            }
            return SevenElevenIGeoComList;
        }
    }
}
