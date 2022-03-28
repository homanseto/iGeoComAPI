using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class ChinaMobileGrabber
    {
        private PuppeteerConnection _puppeteerConnection;
        private IOptions<ChinaMobileOptions> _options;
        private ILogger<ChinaMobileGrabber> _logger;
        private string idCode = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll('.toggle-list-content '));
                                 const hkList = Array.from(selectors[0].querySelectorAll('.map-icon'));
                                 const klnList = Array.from(selectors[1].querySelectorAll('.map-icon'));
                                 const ntList = Array.from(selectors[2].querySelectorAll('.map-icon'));
                                 const hkArray = hkList.map(v =>{return {Region: 'HK', Id: v.querySelector('a').getAttribute('href')}});
                                 const klnArray = hkList.map(v =>{return {Region: 'KLN', Id: v.querySelector('a').getAttribute('href')}});
                                 const ntArray = ntList.map(v =>{return {Region: 'NT', Id: v.querySelector('a').getAttribute('href')}});
                                 return hkArray.concat(klnArray, ntArray);
                                 }";
        private string infoCode = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll('script'));
                                 return selectors[1].textContent.trim();
                                 }";
        private string waitSelectorId = ".toggle-list-content";
        private string waitSelectorInfo = "head";
        ChinaMobileModel chinaMobileModel = new ChinaMobileModel();

        public ChinaMobileGrabber(PuppeteerConnection puppeteerConnection, IOptions<ChinaMobileOptions> options, ILogger<ChinaMobileGrabber> logger)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _logger = logger;
        }

        public async Task GetWebSiteItems()
        {
            var idLink = await _puppeteerConnection.PuppeteerGrabber<ChinaMobileModel[]>(_options.Value.EnUrl, idCode, waitSelectorId);
            await grabResultByID(idLink);
        }

        public async Task grabResultByID(ChinaMobileModel[] idResult)
        {
            var _linkRgx = Regexs.ExtractInfo(chinaMobileModel.ExtractLink);
            List<ChinaMobileModel> ChinaMobileList = new List<ChinaMobileModel>();
            foreach (ChinaMobileModel id in idResult)
            {
                var extractLink = _linkRgx.Match(id.Id!).Groups[1].Value;
                var link = $"{_options.Value.BaseUrl}{extractLink}";
                var shopScript = await _puppeteerConnection.PuppeteerGrabber<string>(link, infoCode, waitSelectorInfo);
                var trimed = Regexs.TrimAllAndAdjustSpace(shopScript).Replace("\n", "").Replace("\t", "");
                Console.WriteLine(trimed);

            }

        }
    }
}
