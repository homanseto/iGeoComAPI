using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class WellcomeGrabber : AbstractGrabber
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        private readonly IOptions<WellcomeOptions> _options;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<WellcomeGrabber> _logger;
        private readonly IDataAccess dataAccess;

        private readonly string infoCode = @"() =>{" +
            @"const selectors = Array.from(document.querySelectorAll('.table-responsive > .table-striped > tbody > tr'));" +
            @"return selectors.map(v => {return {Address: v.querySelector('.views-field-field-address').textContent.trim(), Name: v.querySelector(" +
            @"'.views-field-title > .store-title',).textContent, LatLng: v.querySelector('.views-field-title > .store-title').getAttribute('data-latlng')," +
            @"Phone: v.querySelector('.views-field-field-store-telephone').textContent.trim()" +
            @"}});}";
        private string waitSelector = ".table-responsive";
        private string _regLagLngRegex = "([^|]*)";

        public WellcomeGrabber(PuppeteerConnection puppeteerConnection, IOptions<WellcomeOptions> options, IMemoryCache memoryCache, ILogger<WellcomeGrabber> logger,
    IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        
        public static void TestA(IGeoComModel m)
        {
            var v = m as IGeoComGrabModel;

        }

        public static void TestB()
        {
            IGeoComGrabModel b = new IGeoComGrabModel();
            TestA(b);
        }


        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enResult = await _puppeteerConnection.PuppeteerGrabber<List<WellcomeModel>>(_options.Value.EnUrl, infoCode, waitSelector);
            var zhResult = await _puppeteerConnection.PuppeteerGrabber<List<WellcomeModel>>(_options.Value.ZhUrl, infoCode, waitSelector);
            var mergeResult = MergeEnAndZh(enResult, zhResult);
            var result = await this.GetShopInfo(mergeResult);
            //_memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
            return result;
        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<WellcomeModel> enResult, List<WellcomeModel> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge {Name} En and Zh", this.GetType().Name.Replace("Grabber", ""));
                var _rgx = Regexs.ExtractInfo(_regLagLngRegex);
                List<IGeoComGrabModel> WellcomeIGeoComList = new List<IGeoComGrabModel>();
                foreach (var item in enResult.Select((value, i) => new { i, value }))
                {
                    var shopEn = item.value;
                    var index = item.i;
                    IGeoComGrabModel WellcomeIGeoCom = new IGeoComGrabModel();
                    WellcomeIGeoCom.E_Address = shopEn.Address!;
                    WellcomeIGeoCom.EnglishName = $"Wellcome Supermarket-{shopEn.Name}";
                    var matchesEn = _rgx.Matches(shopEn.LatLng!);
                    WellcomeIGeoCom.Latitude = Convert.ToDouble(matchesEn[0].Value);
                    WellcomeIGeoCom.Longitude = Convert.ToDouble(matchesEn[2].Value);
                    WellcomeIGeoCom.Tel_No = shopEn.Phone!;
                    WellcomeIGeoCom.Web_Site = _options.Value.BaseUrl!;
                    WellcomeIGeoCom.Class = "CMF";
                    WellcomeIGeoCom.Type = "SMK";
                    WellcomeIGeoCom.GrabId = $"wellcome_{shopEn.LatLng}{shopEn.Phone}_{index}".Replace(" ", "").Replace("|", "").Replace(".", "");
                    foreach (var item2 in zhResult.Select((value2, i2) => new { i2, value2 }))
                    {
                        var shopZh = item2.value2;
                        var index2 = item2.i2;
                        var matchesZh = _rgx.Matches(shopZh.LatLng!);
                        if (matchesZh.Count > 0 && matchesZh != null)
                        {
                            if (matchesEn[0].Value == matchesZh[0].Value && matchesEn[2].Value == matchesZh[2].Value && shopEn.Phone == shopZh.Phone && index == index2)
                            {
                                WellcomeIGeoCom.C_Address = shopZh.Address!.Replace(" ", "");
                                WellcomeIGeoCom.ChineseName = $"惠康超級市場-{shopZh.Name}";
                                continue;
                            }      
                        }
                    }
                    WellcomeIGeoComList.Add(WellcomeIGeoCom);
                }
                return WellcomeIGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge Wellcome En and Zh", "Wellcome");
                throw;
            }
        }

    }

}
