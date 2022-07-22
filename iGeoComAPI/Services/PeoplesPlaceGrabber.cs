using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PuppeteerSharp;

namespace iGeoComAPI.Services
{
    public class PeoplesPlaceGrabber : AbstractGrabber
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        private readonly IOptions<PeoplesPlaceOptions> _options;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<PeoplesPlaceGrabber> _logger;
        private readonly IDataAccess dataAccess;

        private readonly string infoCode1 = @"() =>{" +
            @"const selectors = Array.from(document.querySelector(' .elementor-element-5eba2b4 > .elementor-column-gap-default>.elementor-row > .elementor-col-66 > .elementor-column-wrap" +
            @"> .elementor-widget-wrap').querySelectorAll('.elementor-text-editor > p > a'));" +
            @"return selectors.map(v => {return {href: v.getAttribute('href')" +
            @"}});}";
        private string waitSelector1 = ".elementor-element-5eba2b4";

        private readonly string infoCode2 = @"async () =>{" +
            @"const selectors = await document.querySelector('.elementor-custom-embed > iframe');" +
            @"return selectors;"+
            @"}";
        private string waitSelector2 = ".elementor-custom-embed";

        public PeoplesPlaceGrabber(PuppeteerConnection puppeteerConnection, IOptions<PeoplesPlaceOptions> options, IMemoryCache memoryCache, ILogger<PeoplesPlaceGrabber> logger,
    IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            //var enHrefList = await _puppeteerConnection.PuppeteerGrabber<List<PeoplesPlaceModel>>(_options.Value.EnUrl, infoCode1, waitSelector1);
            //var enResult = new List<PeoplesPlaceModel>();
            //foreach (var enHref in enHrefList)
            //{
            //    if (!String.IsNullOrEmpty(enHref.href))
            //    {
            //        var en = await _puppeteerConnection.PuppeteerGrabber<string>(enHref.href, infoCode2, waitSelector2);
            //    }       
            //}
            //var zhResult = await _puppeteerConnection.PuppeteerGrabber<List<PeoplesPlaceModel>>(_options.Value.ZhUrl, infoCode1, waitSelector1);
            var test = await _puppeteerConnection.PuppeteerGrabber<ElementHandle>("https://www.peoplesplace.com.hk/Information/%E9%9B%8D%E7%9B%9B%E5%95%86%E5%A0%B4%E2%80%8B/", infoCode2, waitSelector2);
            //var mergeResult = MergeEnAndZh(enResult, zhResult);
            var result = new List<IGeoComGrabModel>();
            //_memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
            return result;

        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<PeoplesPlaceModel> enResult, List<PeoplesPlaceModel> zhResult)
        {
            //try
            //{
            //    _logger.LogInformation("Merge PeoplesPlace En and Zh");
            //    var _rgx = Regexs.ExtractInfo(_regLagLngRegex);
            //    List<IGeoComGrabModel> PeoplesPlaceIGeoComList = new List<IGeoComGrabModel>();
            //    foreach (var item in enResult.Select((value, i) => new { i, value }))
            //    {
            //        var shopEn = item.value;
            //        var index = item.i;
            //        IGeoComGrabModel PeoplesPlaceIGeoCom = new IGeoComGrabModel();
            //        PeoplesPlaceIGeoCom.E_Address = shopEn.address!;
            //        PeoplesPlaceIGeoCom.EnglishName = $"PeoplesPlace Supermarket-{shopEn.name}";
            //        var matchesEn = _rgx.Matches(shopEn.latlng!);
            //        PeoplesPlaceIGeoCom.Latitude = Convert.ToDouble(matchesEn[0].Value);
            //        PeoplesPlaceIGeoCom.Longitude = Convert.ToDouble(matchesEn[2].Value);
            //        PeoplesPlaceIGeoCom.Tel_No = shopEn.number!;
            //        PeoplesPlaceIGeoCom.Web_Site = _options.Value.BaseUrl!;
            //        PeoplesPlaceIGeoCom.Class = "CMF";
            //        PeoplesPlaceIGeoCom.Type = "SMK";
            //        PeoplesPlaceIGeoCom.Shop = 4;
            //        PeoplesPlaceIGeoCom.GrabId = $"PeoplesPlace_".Replace(" ", "").Replace("|", "").Replace(".", "");
            //        foreach (var item2 in zhResult.Select((value2, i2) => new { i2, value2 }))
            //        {
            //            var shopZh = item2.value2;
            //            var index2 = item2.i2;
            //            var matchesZh = _rgx.Matches(shopZh.latlng!);
            //            if (matchesZh.Count > 0 && matchesZh != null)
            //            {
            //                if (matchesEn[0].Value == matchesZh[0].Value && matchesEn[2].Value == matchesZh[2].Value && shopEn.Phone == shopZh.Phone && index == index2)
            //                {
            //                    PeoplesPlaceIGeoCom.C_Address = shopZh.Address!.Replace(" ", "");
            //                    PeoplesPlaceIGeoCom.ChineseName = $"惠康超級市場-{shopZh.Name}";
            //                    continue;
            //                }
            //            }
            //        }
            //        PeoplesPlaceIGeoComList.Add(PeoplesPlaceIGeoCom);
            //    }
            //    return PeoplesPlaceIGeoComList;
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex.Message, "fail to merge PeoplesPlace En and Zh");
            //    throw;
            //}
            return new List<IGeoComGrabModel>();
        }

    }

}
