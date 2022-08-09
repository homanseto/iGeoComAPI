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
            @"return selectors.map(v => {return {href: v.getAttribute('href'), name: v.textContent" +
            @"}});}";
        private readonly string waitSelectorEng = ".elementor-element-5eba2b4";

        private readonly string infoCodeZh = @"() =>{" +
            @"const list1 ='.elementor-element > .elementor-column-gap-default > .elementor-row'; const list2 = '.elementor-element > .elementor-column-wrap > .elementor-widget-wrap > .elementor-element > .elementor-widget-container > .elementor-text-editor > p > a'; " +
            @"const selectors1 = Array.from(document.querySelectorAll(list1)[11].querySelectorAll(list2));" +
            @"const selectors2 = Array.from(document.querySelectorAll(list1)[12].querySelectorAll(list2));" +
            @"const merge = [...selectors1,...selectors2];" +
            @"return merge.map(v => {return {href: v.getAttribute('href'), name: v.textContent}});" +
            @"}";
        private readonly string waitSelectorZh = ".elementor-element";

        private readonly string iframeInfoCode1 = @".elementor-custom-embed > iframe ";
        private readonly string iframeWaitSelector = @".google-maps-link";
        private readonly string iframeInfoCode2 = @"()=>{return document.querySelector('.google-maps-link > a')? document.querySelector('.google-maps-link > a').getAttribute('href'):'' }";

        private readonly string shopInfoCode = @"() =>{ const marketInfo = Array.from(document.querySelectorAll('.elementor-widget-container')); " +
            @"const shopinfo = Array.from(document.querySelectorAll('.elementor-row'));" +
            @"const parkingArray =  Array.from(document.querySelectorAll('.elementor-widget-wrap'));" +
            @"const infoArray = [marketInfo[12]? marketInfo[12].textContent:'',marketInfo[13]? marketInfo[13].textContent:'',marketInfo[14]? marketInfo[14].textContent:'',marketInfo[15]? marketInfo[15].textContent:''," +
            @"shopinfo[3]? shopinfo[3].textContent:'',shopinfo[4]? shopinfo[4].textContent:'',shopinfo[5]? shopinfo[5].textContent:'',shopinfo[6]? shopinfo[6].textContent:'' ];" +
            @"return {infoList: infoArray, parkingInfo: parkingArray[10]? parkingArray[10].textContent: '' } " +
            @"}";
        private readonly string waitSelectorInfo = ".elementor-widget-container";

        static Regex ExtractInfo(string input)
        {
            Regex reg = new Regex(input);
            return reg;
        }
        Regex rgxNum = ExtractInfo(PeoplesPlaceModel.ExtractNum);
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
            var zhResult = await _puppeteerConnection.PuppeteerGrabber<List<PeoplesPlaceModel>>(_options.Value.ZhUrl, infoCodeZh, waitSelectorZh);
            var enResult = await _puppeteerConnection.PuppeteerGrabber<List<PeoplesPlaceModel>>(_options.Value.EnUrl, infoCodeEng, waitSelectorEng);
            var testZh = new List<PeoplesPlaceModel>();
            var testEn = new List<PeoplesPlaceModel>();
            foreach (var zhHref in zhResult)
            {
                if (!String.IsNullOrEmpty(zhHref.href))
                {
                    //var iframe = await _puppeteerConnection.GetIframaContent(zhHref.href, iframeInfoCode1, iframeWaitSelector, iframeInfoCode2);
                    var info = await _puppeteerConnection.PuppeteerGrabber<PeoplesPlaceModel>(zhHref.href, shopInfoCode, waitSelectorInfo);
                    zhHref.infoList = info.infoList;
                    zhHref.parkingInfo = info.parkingInfo;
                    testZh.Add(zhHref);
                    //zhHref.latlng = iframe;
                }
            }
            foreach (var enHref in enResult)
            {
                if (!String.IsNullOrEmpty(enHref.href))
                {
                    var info = await _puppeteerConnection.PuppeteerGrabber<PeoplesPlaceModel>(enHref.href, shopInfoCode, waitSelectorInfo);
                    enHref.infoList = info.infoList;
                    enHref.parkingInfo = info.parkingInfo;
                    testEn.Add(enHref);
                }
            }
            var mergeResult = CreateIGeoCom(testZh, testEn);
            var result = await this.GetShopInfo(mergeResult);
            return result;

        }

        public List<IGeoComGrabModel> CreateIGeoCom(List<PeoplesPlaceModel> zhResult, List<PeoplesPlaceModel> enResult)
        {
            List<IGeoComGrabModel> PeoplesPlaceIGeoComList = new List<IGeoComGrabModel>();
            foreach (var shopZh in zhResult)
            {
                if (shopZh != null)
                {
                    var mall = shopZh;
                    var market = shopZh;
                    for (int n = 0; n < shopZh.infoList.Count; n++)
                    {
                        if (shopZh.infoList[n].Contains("地址", comp))
                        {
                            string s1 = shopZh.infoList[n].Replace("\n", "").Replace("地址", "").Replace("開放時間", "");
                            var list = s1.Split(":").ToList();
                            mall.address = list[1];
                        }
                        if (String.IsNullOrEmpty(mall.number))
                        {
                            ExtractNumber(shopZh.infoList[n], mall);
                        }
                    }
                    var shoppingMall = MergeEnAndZh(mall, enResult, "");
                    PeoplesPlaceIGeoComList.Add(shoppingMall);
                    if (!shopZh.parkingInfo.Contains("不提供時租泊車服務及泊車優惠", comp))
                    {
                        var parking = MergeEnAndZh(mall, enResult, "parking");
                        PeoplesPlaceIGeoComList.Add(parking);
                    }
                    for (int n = 0; n < shopZh.infoList.Count; n++)
                    {

                        if (shopZh.infoList[n].Contains("街市地點", comp))
                        {
                            string s1 = shopZh.infoList[n + 1].Replace("\n", "");
                            market.address = s1;
                            if (String.IsNullOrEmpty(market.number))
                            {
                                ExtractNumber(shopZh.infoList[n], market);
                            }
                            var marketInfo = MergeEnAndZh(market, enResult, "market");
                            PeoplesPlaceIGeoComList.Add(marketInfo);
                            break;
                        }
                    }

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
                if (type == "parking")
                {
                    PeoplesPlaceIGeoCom.ChineseName = shopZh.name;
                    PeoplesPlaceIGeoCom.Type = "CPO";
                    PeoplesPlaceIGeoCom.Class = "TRS";
                    PeoplesPlaceIGeoCom.C_Address = $"停車場-{shopZh.address}";
                }
                else if (type == "market")
                {
                    PeoplesPlaceIGeoCom.Type = "MKT";
                    PeoplesPlaceIGeoCom.Class = "CMF";
                    PeoplesPlaceIGeoCom.ChineseName = $"市場-{shopZh.name}";
                    PeoplesPlaceIGeoCom.C_Address = shopZh.address;
                }
                else
                {
                    PeoplesPlaceIGeoCom.ChineseName = shopZh.name;
                    PeoplesPlaceIGeoCom.Class = "CMF";
                    PeoplesPlaceIGeoCom.Type = "MAL";
                    PeoplesPlaceIGeoCom.C_Address = shopZh.address;
                }
                PeoplesPlaceIGeoCom.Tel_No = String.Join(",", shopZh.number);
                PeoplesPlaceIGeoCom.Web_Site = _options.Value.BaseUrl!;
                PeoplesPlaceIGeoCom.GrabId = $"PeoplesPlace_{PeoplesPlaceIGeoCom.Tel_No}";
                foreach (var item2 in enResult.Select((value2, i2) => new { i2, value2 }))
                {
                    var shopEn = item2.value2;
                    var index2 = item2.i2;
                    for (int n = 0; n < shopEn.infoList.Count; n++)
                    {
                        if (String.IsNullOrEmpty(shopEn.number))
                        {
                            ExtractNumber(shopEn.infoList[n], shopEn);
                        }
                    }
                    if (type == "parking")
                    {
                        PeoplesPlaceIGeoCom.EnglishName = $"parking_{shopEn.name}";
                    }
                    else if (type == "market")
                    {
                        PeoplesPlaceIGeoCom.EnglishName = $"market_{shopEn.name}";
                    }
                    else
                    {
                        PeoplesPlaceIGeoCom.EnglishName = shopEn.name;
                    }
                    if (shopZh.number.Replace(" ", "").Replace(" ", "") == shopEn.number.Replace(" ", "").Replace(" ", ""))
                    {
                        if (type == "market")
                        {
                            for (int n = 0; n < shopEn.infoList.Count; n++)
                            {

                                if (shopEn.infoList[n].Contains("Wet Market", comp))
                                {
                                    string s1 = shopEn.infoList[n + 1].Replace("\n", "");
                                    PeoplesPlaceIGeoCom.E_Address = s1;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int n = 0; n < shopEn.infoList.Count; n++)
                            {

                                if (shopEn.infoList[n].Contains("Address", comp))
                                {
                                    string s1 = shopEn.infoList[n].Replace("\n", "").Replace("Address", "").Replace("Opening Hour", "");
                                    var list = s1.Split(":").ToList();
                                    PeoplesPlaceIGeoCom.E_Address = list[1];
                                    break;
                                }
                            }
                        }
                        break;
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

        public void ExtractNumber(string number, PeoplesPlaceModel shop)
        {
            if (number.Contains("管理處", comp) || number.Contains("Management", comp))
            {
                string s1 = number.Replace("\n", "").Replace("租務", "").Replace("管理處", "").Replace("Management Office", "").Replace("Leasing", "");
                var list = s1.Split(":").ToList();
                if (list.Count == 2)
                {
                    shop.number = list[1];
                }
                else if (list.Count == 3)
                {
                    shop.number = list[2];
                }
            }
        }
    }

}
