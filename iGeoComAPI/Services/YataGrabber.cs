using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;

namespace iGeoComAPI.Services
{

    public class YataGrabber : AbstractGrabber
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        private IOptions<YataOptions> _options;
        private ILogger<YataGrabber> _logger;
        private readonly IMemoryCache _memoryCache;

        private readonly string infoCode1 = @"() =>{" +
            @"const selectors = Array.from(document.querySelectorAll('.storedetailsection > .row > .gallery_product '));" +
            @"return selectors.map(v =>{return {name: v.querySelector('.storename').textContent, href: v.querySelector('a').getAttribute('href')," +
            @"type: v.querySelector('.storecat').textContent" +
            @"}})" +
            @"}";
        private string waitSelector1 = ".storedetailsection";

        private readonly string infoCode2 = @"() =>{" +
            @"return {latlng: document.querySelector('.accesssection > .googlemaptext > a').getAttribute('href'), address: document.querySelector('.accesssection > .accesstext').textContent," +
            @"number: document.querySelectorAll('.accesssection ')[2].querySelector('.accesstext').textContent}" +
            @"}";
        private string waitSelector2 = ".storedetailscontentsection";

        static Regex ExtractInfo(string input)
        {
            Regex reg = new Regex(input);

            return reg;
        }

        public YataGrabber(PuppeteerConnection puppeteerConnection, IOptions<YataOptions> options, IMemoryCache memoryCache, ILogger<YataGrabber> logger,
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
            var grabEnResult = await _puppeteerConnection.PuppeteerGrabber<List<YataModel>>(_options.Value.EnUrl, infoCode1, waitSelector1);
            var enFilterResult = grabEnResult.Where(v => !v.type.Contains("KONBINI", comp)).ToList();
            var enResult = new List<YataModel>();
            var zhResult = new List<YataModel>();
            foreach (YataModel en in enFilterResult)
            {
                var shop = en;
                var shopinf = await _puppeteerConnection.PuppeteerGrabber<YataModel>($"{_options.Value.Url}{en.href}", infoCode2, waitSelector2);
                if (shopinf != null)
                {
                    shop.address = shopinf.address;
                    shop.latlng = shopinf.latlng;
                    shop.number = shopinf.number;
                }
                enResult.Add(shop);
            }
            var grabZhResult = await _puppeteerConnection.PuppeteerGrabber<List<YataModel>>(_options.Value.ZhUrl, infoCode1, waitSelector1);
            var zhFilterResult = grabZhResult.Where(v => !v.type.Contains("KONBINI", comp)).ToList();
            foreach (YataModel zh in zhFilterResult)
            {
                var shop = zh;
                var shopinf = await _puppeteerConnection.PuppeteerGrabber<YataModel>($"{_options.Value.Url}{zh.href}", infoCode2, waitSelector2);
                if (shopinf != null)
                {
                    shop.address = shopinf.address;
                    shop.latlng = shopinf.latlng;
                    shop.number = shopinf.number;
                }
                zhResult.Add(shop);
            }
            var mergeResult = CreateIGeoCom(enResult, zhResult);
            var result =await this.GetShopInfo(mergeResult);
            return result;
        }

        public List<IGeoComGrabModel> CreateIGeoCom(List<YataModel> enResult, List<YataModel> zhResult)
        {
            List<IGeoComGrabModel> YataIGeoComList = new List<IGeoComGrabModel>();
            foreach (var shopEn in enResult)
            {
                if (shopEn.type.Contains("supermarket", comp))
                {
                    var supermaketShop = MergeEnAndZh(shopEn, zhResult, "supermarket");
                    YataIGeoComList.Add(supermaketShop);
                }
                else
                {

                    var supermaketShop = MergeEnAndZh(shopEn, zhResult, "supermarket");
                    var departmentStore = MergeEnAndZh(shopEn, zhResult, "");
                    YataIGeoComList.Add(supermaketShop);
                    YataIGeoComList.Add(departmentStore);
                }
            }
            return YataIGeoComList;
        }

        public IGeoComGrabModel MergeEnAndZh(YataModel shopEn, List<YataModel> zhResult, string type)
        {
            _logger.LogInformation("Merge Yata En and Zh");
            var _rgx = ExtractInfo(YataModel.RegLatLng);
            var extraLatLng = ExtractInfo(YataModel.ExtraLatLng);
            IGeoComGrabModel YataIGeoCom = new IGeoComGrabModel();
            if (type == "supermarket")
            {
                if(!shopEn.type.Contains("supermarket", comp) && type == "supermarket")
                {
                    YataIGeoCom.EnglishName = $"supermarket {shopEn.name}";
                }
                else
                {
                    YataIGeoCom.EnglishName = $"{shopEn.type} {shopEn.name}";
                }
                YataIGeoCom.Class = "CMF";
                YataIGeoCom.Type = "SMK";
            }
            else
            {
                YataIGeoCom.EnglishName = shopEn.name;
                YataIGeoCom.Class = "CMF";
                YataIGeoCom.Type = "ROI";
            }
            YataIGeoCom.E_Address = shopEn.address;
            YataIGeoCom.Tel_No = shopEn.number;
            YataIGeoCom.Web_Site = _options.Value.BaseUrl;
            string latlng = "";
            var regexLatLng = extraLatLng.Match(shopEn.latlng);
            if (regexLatLng.Success)
            {
                if (!String.IsNullOrEmpty(regexLatLng.Groups["value1"].Value))
                {
                    latlng = regexLatLng.Groups["value1"].Value;
                }
                else if(!String.IsNullOrEmpty(regexLatLng.Groups["value2"].Value))
                {
                    latlng = regexLatLng.Groups["value2"].Value;
                }
            }
            var matchesEn = _rgx.Matches(latlng);
            if (matchesEn.Count > 0 && matchesEn != null)
            {
                YataIGeoCom.Latitude = Convert.ToDouble(matchesEn[0].Value);
                YataIGeoCom.Longitude = Convert.ToDouble(matchesEn[2].Value);
            }

            foreach (var shopZh in zhResult)
            {
                if (shopEn.number.Replace(" ", "") == shopZh.number.Replace(" ", ""))
                {
                    if (!shopEn.type.Contains("supermarket", comp) && type == "supermarket")
                    {
                        YataIGeoCom.EnglishName = $"超級市場 {shopZh.name}";
                    }
                    else
                    {
                        YataIGeoCom.ChineseName = $"{shopZh.type} {shopZh.name}";
                    }
                    YataIGeoCom.C_Address = shopZh.address;
                }
            }
            return YataIGeoCom;
        }
    }

}
