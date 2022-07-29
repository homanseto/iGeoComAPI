using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;


namespace iGeoComAPI.Services
{
    public class CheungKongGrabber : AbstractGrabber
    {
        private PuppeteerConnection _puppeteerConnection;
        private IOptions<CheungKongOptions> _options;
        private ILogger<CheungKongGrabber> _logger;
        private readonly IMemoryCache _memoryCache;
        private string idCode = @"() =>{"+
            @"const selectors = Array.from(document.querySelectorAll('#footer-container > .footer > .bar > .colume'));"+
            @"const arrayList1 = Array.from(selectors[0].querySelectorAll('a'));"+
            @"const idList1 = arrayList1.map(v =>{ return { href: v.getAttribute('href'), name: v.textContent}});"+
            @"const arrayList2 = Array.from(selectors[1].querySelectorAll('a'));"+
            @"const idList2 = arrayList2.map(v =>{return {href: v.getAttribute('href'), name: v.textContent}});" +
            @"return idList1.concat(idList2);"+
            @"}";
        private string infoCode = @"() =>{"+
            @"const selectors = Array.from(document.querySelectorAll('head > script'));"+
            @"return selectors[6].textContent.trim();"+
            @"}";
        private string waitSelectorId = "#footer-container";
        private string waitSelectorInfo = "head";
        private string idLinkAbout = "about";
        private string replaceAboutToLocation = "location";

        public CheungKongGrabber(PuppeteerConnection puppeteerConnection, IOptions<CheungKongOptions> options, IMemoryCache memoryCache, ILogger<CheungKongGrabber> logger,
            IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enIdResult = await _puppeteerConnection.PuppeteerGrabber<List<CheungKongModel>>(_options.Value.EnUrl, idCode, waitSelectorId);
            var zhIdResult = await _puppeteerConnection.PuppeteerGrabber<List<CheungKongModel>>(_options.Value.ZhUrl, idCode, waitSelectorId);
            var enResult = await grabResultByID(enIdResult);
            var zhResult = await grabResultByID(zhIdResult);
            var mergeResult = MergeEnAndZh(enResult, zhResult);
            var result = await this.GetShopInfo(mergeResult);
            return result;
        }

        public async Task<List<CheungKongModel>?> grabResultByID(List<CheungKongModel> idResult)
        {
            var _nameRgx = Regexs.ExtractInfo(CheungKongModel.ExtractName);
            var _latLngRgx = Regexs.ExtractInfo(CheungKongModel.ExtractLatLng);
            var _addressRgx = Regexs.ExtractInfo(CheungKongModel.ExtractAddress);
            var _idRgx = Regexs.ExtractInfo(CheungKongModel.ExtractId);
            var _separateLatLng = Regexs.ExtractInfo(CheungKongModel.RegLagLngRegex);
            List<CheungKongModel> CheungKongModelList = new List<CheungKongModel>();
            foreach(var shop in idResult){
                CheungKongModel CheungKong = new CheungKongModel();
                string infoLink = shop.href.Replace(idLinkAbout, replaceAboutToLocation);
                var infoString = await _puppeteerConnection.PuppeteerGrabber<string>(infoLink, infoCode, waitSelectorInfo);
                var trimed = Regexs.TrimAllAndAdjustSpace(infoString).Replace("\n","").Replace("\t","");
                if(trimed.Length > 0)
                {
                    CheungKong.Name = shop.Name;
                    if (_idRgx.Match(shop.href).Success)
                    {
                        CheungKong.Id = _idRgx.Match(shop.href).Groups["id"].Value;
                    }
                    if (_addressRgx.Match(trimed).Success)
                    {
                        CheungKong.Address = _addressRgx.Match(trimed).Groups["address"].Value.Replace("<span style=\\\"font-size: medium;\\\">", "").Replace("</span>", "").Replace("\" < spanstyle =\\\"font-size:medium;\\\">","").Replace("<span>","");
                    }
                    if (_latLngRgx.Match(trimed).Success)
                    {
                        var latlng = _latLngRgx.Match(trimed).Groups["latlng"].Value;
                        var seperateGroup = _separateLatLng.Matches(latlng);
                        if (seperateGroup.Count > 0)
                        {
                            CheungKong.Latitude = Convert.ToDouble(seperateGroup[0].Value);
                            CheungKong.Longitude = Convert.ToDouble(seperateGroup[3].Value);
                        }
                    }
                    CheungKongModelList.Add(CheungKong);
                }
                
            }
            return CheungKongModelList;
        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<CheungKongModel>? enResult, List<CheungKongModel>? zhResult)
        {
            List<IGeoComGrabModel> CheungKongIGeoComList = new List<IGeoComGrabModel>();
            try
            {
                _logger.LogInformation("Start merging CheungKong eng and Zh");
                if (enResult != null && zhResult != null)
                {
                    foreach (var en in enResult)
                    {
                        IGeoComGrabModel CheungKongIGeoCom = new IGeoComGrabModel();
                        CheungKongIGeoCom.GrabId = $"CheungKong_{en.Id}";
                        CheungKongIGeoCom.EnglishName = $"CheungKong-{en.Name!.Trim()}";
                        CheungKongIGeoCom.E_Address = en.Address;
                        CheungKongIGeoCom.Latitude = en.Latitude;
                        CheungKongIGeoCom.Longitude = en.Longitude;
                        CheungKongIGeoCom.Web_Site = _options.Value.BaseUrl;
                        CheungKongIGeoCom.Class = "CMF";
                        CheungKongIGeoCom.Type = "MAL";
                        foreach (var zh in zhResult)
                        {
                            if (en.Id == zh.Id)
                            {
                                CheungKongIGeoCom.ChineseName = $"長江-{zh.Name!.Trim()}";
                                CheungKongIGeoCom.C_Address = zh.Address;

                                continue;
                            }
                        }
                        CheungKongIGeoComList.Add(CheungKongIGeoCom);
                    }
                }
                return CheungKongIGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge Catlex Eng and Zh RawData");
                throw;
            }
        }
    }
}
