using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;

namespace iGeoComAPI.Services
{
    public class HkMarketGrabber : AbstractGrabber
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        private IOptions<HkMarketOptions> _options;
        private ILogger<HkMarketGrabber> _logger;
        private readonly IMemoryCache _memoryCache;

        private readonly string infoCode = @"() =>{" +
            @"const selectors = Array.from(document.querySelectorAll('.market-grid-loop > .row > .col-sm-3 > .market-grid-detail > a'));" +
            @"return selectors.map(v =>{return {href: v.getAttribute('href')}})" +
            @"}";
        private string waitSelector = ".fusion-text";

        private readonly string infoCode2 = @"() =>{" +
            @"return {address: document.querySelector('.market-address').textContent.trim(), name: document.querySelector('.entry-title').textContent.trim()," +
            @"latitude: document.querySelector('.wpv-addon-maps-marker').getAttribute('data-markerlat'), longitude: document.querySelector('.wpv-addon-maps-marker').getAttribute('data-markerlon')}" +
            @"}";
        private string waitSelector2 = ".top-section-container";


        public HkMarketGrabber(PuppeteerConnection puppeteerConnection, IOptions<HkMarketOptions> options, IMemoryCache memoryCache, ILogger<HkMarketGrabber> logger,
            IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enResult = await _puppeteerConnection.PuppeteerGrabber<List<HkMarketModel>>(_options.Value.EnUrl, infoCode, waitSelector);
            foreach(var en in enResult)
            {
                if (!String.IsNullOrEmpty(en.href))
                {
                    var enInfo = await _puppeteerConnection.PuppeteerGrabber<HkMarketModel>(en.href, infoCode2, waitSelector2);
                    en.latitude = enInfo.latitude;
                    en.longitude = enInfo.longitude;
                    en.name = enInfo.name;
                    en.address = enInfo.address;
                }
            }
            var zhResult = await _puppeteerConnection.PuppeteerGrabber<List<HkMarketModel>>(_options.Value.ZhUrl, infoCode, waitSelector);
            foreach (var zh in zhResult)
            {
                if (!String.IsNullOrEmpty(zh.href))
                {
                    var zhInfo = await _puppeteerConnection.PuppeteerGrabber<HkMarketModel>(zh.href, infoCode2, waitSelector2);
                    zh.latitude = zhInfo.latitude;
                    zh.longitude = zhInfo.longitude;
                    zh.name = zhInfo.name;
                    zh.address = zhInfo.address;
                }
            }
            var mergeResult = MergeEnAndZh(enResult, zhResult);
            var result = await this.GetShopInfo(mergeResult);
            return result;
        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<HkMarketModel> enResult, List<HkMarketModel> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge HkMarket En and Zh");
                List<IGeoComGrabModel> HkMarketIGeoComList = new List<IGeoComGrabModel>();
                foreach (var item in enResult.Select((value, i) => new { i, value }))
                {
                    var shopEn = item.value;
                    var index = item.i;
                    IGeoComGrabModel HkMarketIGeoCom = new IGeoComGrabModel();
                    HkMarketIGeoCom.E_Address = shopEn.address!;
                    HkMarketIGeoCom.EnglishName = $"HkMarket {shopEn.name}";
                    HkMarketIGeoCom.Latitude = shopEn.latitude;
                    HkMarketIGeoCom.Longitude = shopEn.longitude;
                    HkMarketIGeoCom.Web_Site = _options.Value.BaseUrl!;
                    HkMarketIGeoCom.Type = "SMK";
                    HkMarketIGeoCom.Class = "CMF";
                    HkMarketIGeoCom.GrabId = $"HkMarket_";
                    foreach (var item2 in zhResult.Select((value2, i2) => new { i2, value2 }))
                    {
                        var shopZh = item2.value2;
                        var index2 = item2.i2;
                        if (shopEn.latitude == shopZh.latitude && shopEn.longitude == shopZh.longitude)
                        {
                            HkMarketIGeoCom.C_Address = shopZh.address!.Replace(" ", "");
                            HkMarketIGeoCom.ChineseName = $"香港街市-{shopZh.name}";
                            continue;
                        }

                    }
                    HkMarketIGeoComList.Add(HkMarketIGeoCom);
                }
                return HkMarketIGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge HkMarket En and Zh");
                throw;
            }
        }
    }
}
