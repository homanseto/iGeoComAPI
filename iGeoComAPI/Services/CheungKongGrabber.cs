using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class CheungKongGrabber
    {
        private PuppeteerConnection _puppeteerConnection;
        private IOptions<CheungKongOptions> _options;
        private ILogger<CheungKongGrabber> _logger;
        private string idCode = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll('#footer-container > .footer > .bar > .colume'));
                                 const arrayList1 = Array.from(selectors[0].querySelectorAll('a'));
                                 const idList1 = arrayList1.map(v =>{return {Id: v.getAttribute('href')}});
                                 const arrayList2 = Array.from(selectors[1].querySelectorAll('a'));
                                 const idList2 = arrayList2.map(v =>{return {Id: v.getAttribute('href')}});
                                 return idList1.concat(idList2);
                                 }";
        private string infoCode = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll('head > script'));
                                 return selectors[6].textContent.trim();
                                 }";
        private string waitSelectorId = "#footer-container";
        private string waitSelectorInfo = "head";
        private string idLinkAbout = "about";
        private string replaceAboutToLocation = "location";

        public CheungKongGrabber(PuppeteerConnection puppeteerConnection, IOptions<CheungKongOptions> options, ILogger<CheungKongGrabber> logger,
            IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _logger = logger;
        }

        public async Task<List<CheungKongModel>?> GetWebSiteItems()
        {
            var idResult = await _puppeteerConnection.PuppeteerGrabber<CheungKongModel[]>(_options.Value.EnUrl, idCode, waitSelectorId);
            var result = await grabResultByID(idResult);
            return result;
        }

        public async Task<List<CheungKongModel>?> grabResultByID(CheungKongModel[] idResult)
        {
            var _nameRgx = Regexs.ExtractInfo(CheungKongModel.ExtractName);
            var _latLngRgx = Regexs.ExtractInfo(CheungKongModel.ExtractLatLng);
            var _addressRgx = Regexs.ExtractInfo(CheungKongModel.ExtractAdrress);
            var _idRgx = Regexs.ExtractInfo(CheungKongModel.ExtractId);
            var _separateLatLng = Regexs.ExtractInfo(CheungKongModel.RegLagLngRegex);
            List<CheungKongModel> CheungKongModelList = new List<CheungKongModel>();
            foreach(CheungKongModel id in idResult){
                CheungKongModel CheungKong = new CheungKongModel();
                string infoLink = id.Id!.Replace(idLinkAbout, replaceAboutToLocation);
                var infoString = await _puppeteerConnection.PuppeteerGrabber<string>(infoLink, infoCode, waitSelectorInfo);
                var trimed = Regexs.TrimAllAndAdjustSpace(infoString).Replace("\n","").Replace("\t","");
                CheungKong.Name = _nameRgx.Match(trimed).Groups[1].Value;
                CheungKong.Address = _addressRgx.Match(trimed).Groups[1].Value;
                CheungKong.LatLng = _latLngRgx.Match(trimed).Groups[1].Value;
                CheungKong.Id = _idRgx.Match(id.Id).Groups[1].Value;
                CheungKongModelList.Add(CheungKong);
            }
            return CheungKongModelList;
        }
    }
}
