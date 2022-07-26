using HtmlAgilityPack;
using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PuppeteerSharp;
using System.Text.RegularExpressions;

namespace iGeoComAPI.Services
{
    public class PeoplesPlaceGrabber : AbstractGrabber
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        private readonly IOptions<PeoplesPlaceOptions> _options;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<PeoplesPlaceGrabber> _logger;
        private readonly IDataAccess dataAccess;

        private readonly string infoCodeEng = @"() =>{" +
            @"const selectors = Array.from(document.querySelector(' .elementor-element-5eba2b4 > .elementor-column-gap-default>.elementor-row > .elementor-col-66 > .elementor-column-wrap" +
            @"> .elementor-widget-wrap').querySelectorAll('.elementor-text-editor > p > a'));" +
            @"return selectors.map(v => {return {href: v.getAttribute('href')" +
            @"}});}";
        private readonly string waitSelectorEng = ".elementor-element-5eba2b4";

        private readonly string infoCodeZh = @"() =>{" +
            @"const list1 ='.elementor-element > .elementor-column-gap-default > .elementor-row'; const list2 = '.elementor-element > .elementor-column-wrap > .elementor-widget-wrap > .elementor-element > .elementor-widget-container > .elementor-text-editor > p > a'; " +
            @"const selectors1 = Array.from(document.querySelectorAll(list1)[11].querySelectorAll(list2));" +
            @"const selectors2 = Array.from(document.querySelectorAll(list1)[12].querySelectorAll(list2));" +
            @"const merge = [...selectors1,...selectors2];"+
            @"return merge.map(v => {return {href: v.getAttribute('href')}});" +
            @"}";
        private readonly string waitSelectorZh = ".elementor-element";

        private readonly string iframeInfoCode1 = @".elementor-custom-embed > iframe ";
        private readonly string iframeWaitSelector = @".elementor-custom-embed > iframe";
        private readonly string iframeInfoCode2 = @"()=>{return document.querySelector('.google-maps-link > a')? document.querySelector('.google-maps-link > a').getAttribute('href'):'' }";

        private readonly string shopInfoCode = @"() =>{ return {address: document.querySelectorAll('.elementor-widget-container > .elementor-text-editor > p')[2].textContent," +
            @"number: document.querySelectorAll('.elementor-widget-container > .elementor-text-editor > p')[7].textContent, name:document.querySelector('.elementor-widget-container > h2').textContent, " +
            @"parkingInfo: document.querySelectorAll('.elementor-text-editor > .p1')[1]? document.querySelectorAll('.p1')[1].textContent: '', " +
            @"marketInfo1: document.querySelectorAll(' .elementor-widget-container > .elementor-heading-title')[2]? document.querySelectorAll(' .elementor-widget-container > .elementor-heading-title')[2].textContent: ''," +
            @"marketInfo2: document.querySelectorAll(' .elementor-widget-container > .elementor-heading-title')[3]? document.querySelectorAll(' .elementor-widget-container > .elementor-heading-title')[3].textContent: ''," +
            @"marketAddress1: document.querySelectorAll('.elementor-text-editor > p > .s1')[3]? document.querySelectorAll('.elementor-text-editor > p > .s1')[3].textContent:'', " +
            @"marketAddress2: document.querySelectorAll('.elementor-widget-wrap > .elementor-element > .elementor-widget-container > .elementor-clearfix > p')[9]? document.querySelectorAll('.elementor-widget-wrap > .elementor-element > .elementor-widget-container > .elementor-clearfix > p')[9].textContent:'' " +
            @" }}";
        private readonly string waitSelectorInfo = ".elementor-widget-container";

        static Regex ExtractInfo(string input)
        {
            Regex reg = new Regex(input);
            return reg;
        }
        public PeoplesPlaceGrabber(PuppeteerConnection puppeteerConnection, IOptions<PeoplesPlaceOptions> options, IMemoryCache memoryCache, ILogger<PeoplesPlaceGrabber> logger,
    IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        StringComparison comp = StringComparison.OrdinalIgnoreCase;

        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var zhHrefList = await _puppeteerConnection.PuppeteerGrabber<List<PeoplesPlaceModel>>(_options.Value.ZhUrl, infoCodeZh, waitSelectorZh);
            var enHrefList = await _puppeteerConnection.PuppeteerGrabber<List<PeoplesPlaceModel>>(_options.Value.EnUrl, infoCodeEng, waitSelectorEng);
            var enResult = new List<PeoplesPlaceModel>();
            var zhResult = new List<PeoplesPlaceModel>();
            foreach (var zhHref in zhHrefList.GetRange(10, 11))
            {
                if (!String.IsNullOrEmpty(zhHref.href))
                {
                    var shopInfo = zhHref;
                    var iframe = await _puppeteerConnection.GetIframaContent(zhHref.href, iframeInfoCode1, iframeWaitSelector, iframeInfoCode2);
                    var info = await _puppeteerConnection.PuppeteerGrabber<PeoplesPlaceModel>(zhHref.href, shopInfoCode, waitSelectorInfo);
                    shopInfo.latlng = iframe;
                    shopInfo.address = info.address;
                    shopInfo.name = info.name;
                    shopInfo.number = info.number;
                    shopInfo.parkingInfo = info.parkingInfo;
                    shopInfo.marketInfo1 = info.marketInfo1;
                    shopInfo.marketInfo2 = info.marketInfo2;
                    shopInfo.marketAddress1 = info.marketAddress1;
                    shopInfo.marketAddress2 = info.marketAddress2;
                    zhResult.Add(shopInfo);
                }
            }
            foreach (var enHref in enHrefList.GetRange(10, 11))
            {
                var shopInfo = enHref;
                var info = await _puppeteerConnection.PuppeteerGrabber<PeoplesPlaceModel>(enHref.href, shopInfoCode, waitSelectorInfo);
                shopInfo.address = info.address;
                shopInfo.name = info.name;
                shopInfo.number = info.number;
                enResult.Add(shopInfo);
            }
            var mergeResult = CreateIGeoCom(zhResult, enResult);
            var result = await this.GetShopInfo(mergeResult);
            //var mergeResult = MergeEnAndZh(enResult, zhResult);
            //var result = await this.GetShopInfo(testList);
            //_memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
            return result;

        }

        public List<IGeoComGrabModel> CreateIGeoCom(List<PeoplesPlaceModel> zhResult, List<PeoplesPlaceModel> enResult)
        {
            List<IGeoComGrabModel> PeoplesPlaceIGeoComList = new List<IGeoComGrabModel>();
            foreach (var shopZh in zhResult)
            {
                if (shopZh != null)
                {
                    var shoppingMall = MergeEnAndZh(shopZh, enResult, "");
                    PeoplesPlaceIGeoComList.Add(shoppingMall);
                }

                if (shopZh != null && !shopZh.parkingInfo.Contains("不提供時租泊車服務及泊車優惠", comp))
                {
                    var parking = MergeEnAndZh(shopZh, enResult, "parking");
                    PeoplesPlaceIGeoComList.Add(parking);
                }
                if (shopZh != null && (shopZh.marketInfo1.Contains("街市地點", comp) || shopZh.marketInfo2.Contains("街市地點", comp)))
                {

                    var marketShop = MergeEnAndZh(shopZh, zhResult, "market");
                    PeoplesPlaceIGeoComList.Add(marketShop);
                }
            }
            return PeoplesPlaceIGeoComList;
        }

        public IGeoComGrabModel MergeEnAndZh(PeoplesPlaceModel shopZh, List<PeoplesPlaceModel> enResult, string type)
        {
            try
            {
                _logger.LogInformation("Merge PeoplesPlace En and Zh");
                IGeoComGrabModel PeoplesPlaceIGeoCom = new IGeoComGrabModel();
                PeoplesPlaceIGeoCom.ChineseName = $"PeoplesPlace Supermarket-{shopZh.name}";
                var rgxLat = ExtractInfo(PeoplesPlaceModel.ExtractLat);
                var rgxLng = ExtractInfo(PeoplesPlaceModel.ExtractLng);

                var extractLat = rgxLat.Match(shopZh.latlng);
                if (extractLat.Success)
                {
                    if (!String.IsNullOrEmpty(extractLat.Groups["lat"].Value))
                    {
                        string lat = extractLat.Groups["lat"].Value;
                        PeoplesPlaceIGeoCom.Latitude = Convert.ToDouble(lat);
                    }
                }
                var extractLng = rgxLng.Match(shopZh.latlng);
                if (extractLng.Success)
                {
                    if (!String.IsNullOrEmpty(extractLng.Groups["lng"].Value))
                    {
                        string lng = extractLng.Groups["lng"].Value;
                        PeoplesPlaceIGeoCom.Longitude = Convert.ToDouble(lng);
                    }
                }
                if(type == "parking")
                {
                    PeoplesPlaceIGeoCom.Type = "CPO";
                    PeoplesPlaceIGeoCom.Class = "TRS";
                    PeoplesPlaceIGeoCom.C_Address = shopZh.address.Replace(":", "").Replace(" ", "")!;
                }
                else if(type == "market")
                {
                    PeoplesPlaceIGeoCom.Type = "MKT";
                    PeoplesPlaceIGeoCom.Class = "CMF";
                    if (String.IsNullOrEmpty(shopZh.marketAddress1))
                    {
                        PeoplesPlaceIGeoCom.C_Address = shopZh.marketAddress2;
                    }
                    else
                    {
                        PeoplesPlaceIGeoCom.C_Address = shopZh.marketAddress1;
                    }
                }
                else
                {
                    PeoplesPlaceIGeoCom.Class = "CMF";
                    PeoplesPlaceIGeoCom.Type = "MAL";
                    PeoplesPlaceIGeoCom.C_Address = shopZh.address.Replace(":", "").Replace(" ", "")!;
                }
                PeoplesPlaceIGeoCom.Tel_No = shopZh.number.Replace(":", "").Replace(" ", "");
                PeoplesPlaceIGeoCom.Web_Site = _options.Value.BaseUrl!;
                PeoplesPlaceIGeoCom.GrabId = $"PeoplesPlace_{PeoplesPlaceIGeoCom.Tel_No}";
                foreach (var item2 in enResult.Select((value2, i2) => new { i2, value2 }))
                {
                    var shopEn = item2.value2;
                    var index2 = item2.i2;

                    if (shopZh.number == shopEn.number)
                    {
                        PeoplesPlaceIGeoCom.E_Address = shopEn.address!.Replace(":", "");
                        PeoplesPlaceIGeoCom.EnglishName = $"-{shopEn.name}";
                        continue;
                    }

                }


                return PeoplesPlaceIGeoCom;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge PeoplesPlace En and Zh");
                throw;
            }
        }

    }

}
