using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class CatholicOrgGrabber
    {
        private PuppeteerConnection _puppeteerConnection;
        private IOptions<CatholicOrgOptions> _options;
        private ILogger<CatholicOrgGrabber> _logger;
        private string infoCode = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll('.su-table-alternate > .su-row > .su-column-size-1-3'));
                                 const hkList = Array.from(selectors[0].querySelectorAll('.menu-churches-hki-container >.mip-drop-nav > option'));
                                 const klnList = Array.from(selectors[1].querySelectorAll('.menu-churches-kol-container >.mip-drop-nav > option'));
                                 const ntList = Array.from(selectors[2].querySelectorsAll('.menu-churches-nt-container>.mip-drop-nav > option'));
                                 const hkListResult = hkList.map(v =>{return {Region:'HK', Id: v.getAttribute('value ')}});
                                 const klnListResult = hkList.map(v =>{return {Region:'HK', Id: v.getAttribute('value ')}});
                                 const ntListResult = hkList.map(v =>{return {Region:'HK', Id: v.getAttribute('value ')}});
                                 return hkListResult.concat(klnListResult,ntListResult);
                                 }";
        private string waitSelector = ".rich_editor_text";

        public CatholicOrgGrabber(PuppeteerConnection puppeteerConnection, IOptions<CatholicOrgOptions> options, ILogger<CatholicOrgGrabber> logger)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _logger = logger;
        }

        public async Task GetWebSiteItems()
        {
            var enResult = await _puppeteerConnection.PuppeteerGrabber<CatholicOrgModel[]>(_options.Value.EnUrl, infoCode, waitSelector);
            foreach(var en in enResult)
            {
                Console.WriteLine(en.Id);
            }

        }
    }
}
