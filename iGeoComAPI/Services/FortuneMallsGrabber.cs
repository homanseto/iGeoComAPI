using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;
using System.Data;
using System.Reflection;
using Newtonsoft.Json;

namespace iGeoComAPI.Services
{
    public class FortuneMallsGrabber : AbstractGrabber
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        private IOptions<FortuneMallsOptions> _options;
        private ILogger<FortuneMallsGrabber> _logger;
        private readonly IMemoryCache _memoryCache;

        private readonly string infoCode1 = @"() =>{" +
            @"const selectors = Array.from(document.querySelectorAll('.common-menu-list > li'));" +
            @"return selectors.map(v =>{return {href: v.getAttribute('onClick'), name:v.querySelector('span').textContent }})" +
            @"}";
        private string waitSelector1 = ".common-menu-list";

        private readonly string iframeInfoCode1 = @"#parking-content > div > iframe";
        private readonly string iframeWaitSelector = @".google-maps-link ";
        private readonly string iframeInfoCode2 = @"()=>{return document.querySelector('.google-maps-link > a')? document.querySelector('.google-maps-link > a').getAttribute('href'):'' }";
        private readonly string iframeChick = @"#parking";

        private readonly string infoCode2 = @"() =>{" +
            @"const selectors = Array.from(document.querySelectorAll('#parking-content > div > p > span'));" +
            @"const infoArray = [selectors[4]? selectors[4].textContent:'', selectors[5]? selectors[5].textContent:'' ,selectors[6]? selectors[6].textContent:''," +
            @" selectors[8]? selectors[8].textContent:'', selectors[9]? selectors[9].textContent:'', selectors[10]? selectors[10].textContent:''," +
            @"selectors[11]? selectors[11].textContent:'', selectors[12]? selectors[12].textContent:'', selectors[15]? selectors[15].textContent:''];" +
            @"return {infoList: infoArray}" +
            @"}";
        private string waitSelector2 = "#parking-content";
        StringComparison comp = StringComparison.OrdinalIgnoreCase;
        static Regex ExtractInfo(string input)
        {
            Regex reg = new Regex(input);
            return reg;
        }
        Regex rgxId = ExtractInfo(FortuneMallsModel.extractId);
        Regex rgxLatLng = ExtractInfo(FortuneMallsModel.extractLatLng);

        public FortuneMallsGrabber(PuppeteerConnection puppeteerConnection, IOptions<FortuneMallsOptions> options, IMemoryCache memoryCache, ILogger<FortuneMallsGrabber> logger,
            IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var hrefEnResult = await _puppeteerConnection.PuppeteerGrabber<List<FortuneMallsModel>>(_options.Value.EnUrl, infoCode1, waitSelector1);
            var hrefZhResult = await _puppeteerConnection.PuppeteerGrabber<List<FortuneMallsModel>>(_options.Value.ZhUrl, infoCode1, waitSelector1);
            var enResult = await GetFortuneMallsShopInfo(hrefEnResult, "eng");
            var zhResult = await GetFortuneMallsShopInfo(hrefZhResult, "zh");
            var mergeResult = MergeEnAndZh(enResult, zhResult);
            var result = await this.GetShopInfo(mergeResult);
            return result;
        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<FortuneMallsModel> enResult, List<FortuneMallsModel> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge FortuneMalls En and Zh");
                List<IGeoComGrabModel> FortuneMallsGeoComList = new List<IGeoComGrabModel>();
                foreach (var item in enResult.Select((value, i) => new { i, value }))
                {
                    var shopEn = item.value;
                    var index = item.i;
                    IGeoComGrabModel FortuneMallsGeoCom = new IGeoComGrabModel();
                    FortuneMallsGeoCom.EnglishName = shopEn.name;
                    FortuneMallsGeoCom.E_Address = shopEn.address;
                    FortuneMallsGeoCom.Latitude = shopEn.latitude;
                    FortuneMallsGeoCom.Longitude = shopEn.longitude;
                    FortuneMallsGeoCom.Tel_No = shopEn.number;
                    FortuneMallsGeoCom.GrabId = $"FortuneMalls_{shopEn.id}";
                    foreach (var item2 in zhResult.Select((value2, i2) => new { i2, value2 }))
                    {
                        var shopZh = item2.value2;
                        var index2 = item2.i2;
                        if(shopEn.id == shopZh.id)
                        {
                            FortuneMallsGeoCom.ChineseName = shopZh.name;
                            FortuneMallsGeoCom.C_Address = shopZh.address;
                        }

                    }
                    FortuneMallsGeoComList.Add(FortuneMallsGeoCom);
                }
                return FortuneMallsGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge FortuneMalls En and Zh");
                throw;
            }
        }

        public async Task<List<FortuneMallsModel>> GetFortuneMallsShopInfo(List<FortuneMallsModel> hrefResult, string type)
        {
            var result = new List<FortuneMallsModel>();
            foreach (var shop in hrefResult)
            {
                var shopInfo = new FortuneMallsModel();
                var extractId = rgxId.Match(shop.href);
                if (extractId.Success)
                {
                    if (!String.IsNullOrEmpty(extractId.Groups["id"].Value))
                    {
                        shop.id = extractId.Groups["id"].Value;
                    }
                }
                if (type == "eng")
                {
                    shopInfo = await _puppeteerConnection.PuppeteerGrabber<FortuneMallsModel>($"{_options.Value.EnUrl}{shop.id}", infoCode2, waitSelector2);
                }
                else
                {
                    shopInfo = await _puppeteerConnection.PuppeteerGrabber<FortuneMallsModel>($"{_options.Value.ZhUrl}{shop.id}", infoCode2, waitSelector2);
                }
                
                GetAddressAndNumber(shopInfo, shop);
                if(type == "eng")
                {
                    var iframe = await _puppeteerConnection.GetIframaContent($"{_options.Value.EnUrl}{shop.id}", iframeInfoCode1, iframeWaitSelector, iframeInfoCode2, iframeChick);
                    if (String.IsNullOrEmpty(iframe))
                    {
                        result.Add(shop);
                        continue;
                    }
                    var extractLatLng = rgxLatLng.Match(iframe);
                    if (extractLatLng.Success)
                    {
                        if (!String.IsNullOrEmpty(extractLatLng.Groups["latlng"].Value) && extractLatLng.Groups["latlng"].Value.Contains(','))
                        {
                            List<string> latlngList = extractLatLng.Groups["latlng"].Value.Split(',').ToList();
                            shop.latitude = Convert.ToDouble(latlngList[0]);
                            shop.longitude = Convert.ToDouble(latlngList[1]);
                        }
                    }
                }
            }
            return hrefResult;
        }

        public void GetAddressAndNumber(FortuneMallsModel shopInfo, FortuneMallsModel shop)
        {
            foreach (var info in shopInfo.infoList)
            {
                if (info.Contains("Address", comp) || info.Contains("地址", comp))
                {
                    shop.address = info.Replace("Address:", "").Replace("地址：", "");
                    continue;
                }
                if (info.Contains("電話", comp) || info.Contains("Tel", comp))
                {
                    shop.number = info.Replace("電話:", "").Replace("Tel:", "");
                    continue;
                }

            }
        }
    }
}
