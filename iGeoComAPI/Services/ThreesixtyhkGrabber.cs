using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace iGeoComAPI.Services
{
    public class ThreesixtyhkGrabber : AbstractGrabber
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        private readonly IOptions<ThreesixtyhkOptions> _options;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ThreesixtyhkGrabber> _logger;
        private readonly IDataAccess dataAccess;

        private readonly string infoCode = @"() =>{" +
            @"const latlngList = Array.from(document.querySelectorAll('.contactus_content > div > iframe'));" +
            @"const info = Array.from(document.querySelectorAll('.contactus_content > p'));" +
            @"return latlngList.map((v,i) => {return {latlng: v.getAttribute('src').trim(), name: document.querySelectorAll('strong')[i].textContent.trim()," +
            @"address: info[i*7].textContent, number: info[1 + i*7].textContent" +
            @"}});}";
        private string waitSelector = "#main-content";

        static Regex ExtractInfo(string input)
        {
            Regex reg = new Regex(input);
            return reg;
        }

        public ThreesixtyhkGrabber(PuppeteerConnection puppeteerConnection, IOptions<ThreesixtyhkOptions> options, IMemoryCache memoryCache, ILogger<ThreesixtyhkGrabber> logger,
    IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enResult = await _puppeteerConnection.PuppeteerGrabber<List<ThreesixtyhkModel>>(_options.Value.EnUrl, infoCode, waitSelector);
            var zhResult = await _puppeteerConnection.PuppeteerGrabber<List<ThreesixtyhkModel>>(_options.Value.ZhUrl, infoCode, waitSelector);
            var mergeResult = MergeEnAndZh(enResult, zhResult);
            var result = await this.GetShopInfo(mergeResult);
            //_memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
            return result;

        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<ThreesixtyhkModel> enResult, List<ThreesixtyhkModel> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge Threesixtyhk En and Zh");
                var rgxLatLng = ExtractInfo(ThreesixtyhkModel.extraLatLng);
                var rgxFax = ExtractInfo(ThreesixtyhkModel.extraFax);
                var rgxTel = ExtractInfo(ThreesixtyhkModel.extraTel);
                var rgxZhFax = ExtractInfo(ThreesixtyhkModel.extraZhFax);
                var rgxZhTel = ExtractInfo(ThreesixtyhkModel.extraZhTel);
                var rgxSplitlatLng = ExtractInfo(ThreesixtyhkModel.splitLatLng);
                List<IGeoComGrabModel> ThreesixtyhkIGeoComList = new List<IGeoComGrabModel>();
                foreach (var item in enResult.Select((value, i) => new { i, value }))
                {
                    var shopEn = item.value;
                    var index = item.i;
                    IGeoComGrabModel ThreesixtyhkIGeoCom = new IGeoComGrabModel();
                    ThreesixtyhkIGeoCom.E_Address = shopEn.address!;
                    ThreesixtyhkIGeoCom.EnglishName = $"Threesixtyhk {shopEn.name}";
                    var extraLatLng = rgxLatLng.Match(shopEn.latlng);
                    if (extraLatLng.Success)
                    {
                        if (!String.IsNullOrEmpty(extraLatLng.Groups["latlng"].Value))
                        {
                            string latlng = extraLatLng.Groups["latlng"].Value;
                            var matchesLatLng = rgxSplitlatLng.Matches(latlng);
                            if (matchesLatLng.Count > 0 && matchesLatLng != null)
                            {
                                ThreesixtyhkIGeoCom.Latitude = Convert.ToDouble(matchesLatLng[0].Value);
                                ThreesixtyhkIGeoCom.Longitude = Convert.ToDouble(matchesLatLng[3].Value);
                            }
                        }
                    }
                    var extraTel = rgxTel.Match(shopEn.number);
                    if (extraTel.Success)
                    {
                        if (!String.IsNullOrEmpty(extraTel.Groups["tel"].Value))
                        {
                            string tel = extraTel.Groups["tel"].Value;
                            ThreesixtyhkIGeoCom.Tel_No = tel;
                        }
                    }
                    var extraFax = rgxFax.Match(shopEn.number);
                    if (extraFax.Success)
                    {
                        if (!String.IsNullOrEmpty(extraFax.Groups["fax"].Value))
                        {
                            string fax = extraTel.Groups["fax"].Value;
                            ThreesixtyhkIGeoCom.Fax_No = fax;
                        }
                    }
                    ThreesixtyhkIGeoCom.Web_Site = _options.Value.BaseUrl!;
                    ThreesixtyhkIGeoCom.Class = "CMF";
                    ThreesixtyhkIGeoCom.Type = "SMK";
                    ThreesixtyhkIGeoCom.GrabId = $"Threesixtyhk_{ThreesixtyhkIGeoCom.Tel_No}".Replace("-","");
                    foreach (var item2 in zhResult.Select((value2, i2) => new { i2, value2 }))
                    {
                        var shopZh = item2.value2;
                        var index2 = item2.i2;
                        var extraTel2 = rgxZhTel.Match(shopZh.number);
                        string telZh = "";
                        if (extraTel2.Success)
                        {
                            if (!String.IsNullOrEmpty(extraTel2.Groups["tel"].Value))
                            {
                                telZh = extraTel2.Groups["tel"].Value;
                                if (ThreesixtyhkIGeoCom.Tel_No.Replace("-","").Replace(" ","") == telZh.Replace("-", "").Replace(" ", ""))
                                {
                                    ThreesixtyhkIGeoCom.C_Address = shopZh.address!.Replace(" ", "");
                                    ThreesixtyhkIGeoCom.ChineseName = $"超級市場-{shopZh.name}";
                                    continue;
                                }
                            }
                        }
                    }
                    ThreesixtyhkIGeoComList.Add(ThreesixtyhkIGeoCom);
                }
                return ThreesixtyhkIGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge Threesixtyhk En and Zh");
                throw;
            }
        }

    }

}
