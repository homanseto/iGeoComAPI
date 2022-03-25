using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class CSLGrabber
    {
        private PuppeteerConnection _puppeteerConnection;
        private IOptions<CSLOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<CSLGrabber> _logger;
        private string infoCode = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll(' .bg_clr_w > .table-border'));
                                 const hkList = Array.from(selectors[0].querySelectorAll('tr'));
                                 const klnList = Array.from(selectors[1].querySelectorAll('tr'));
                                 const ntList = Array.from(selectors[2].querySelectorAll('tr'));
                                 let hkArray = [];
                                 let klnArray = [];
                                 let ntArray = [];
                                 for(i=2; i< hkList.length; i++){
                                 const tdList = Array.from(hkList[i].querySelectorAll('td')); 
                                 const phone = Array.from(hkList[2].querySelectorAll('td'))[2].textContent.trim();
                                 let hkObj = {Region: 'HK', Address: tdList[1].textContent.trim(), District: tdList[0].textContent.trim(), Phone: phone }
                                 hkArray.push(hkObj);
                                 }
                                 for(i=2; i< klnList.length; i++){
                                 const tdList = Array.from(klnList[i].querySelectorAll('td')); 
                                 const phone = Array.from(klnList[2].querySelectorAll('td'))[2].textContent.trim();
                                 let klnObj = {Region: 'KLN', Address: tdList[1].textContent.trim(), District: tdList[0].textContent.trim(), Phone: phone }
                                 hkArray.push(klnObj);
                                 }
                                 for(i=2; i< ntList.length; i++){
                                 const tdList = Array.from(ntList[i].querySelectorAll('td')); 
                                 const phone = Array.from(ntList[2].querySelectorAll('td'))[2].textContent.trim();
                                 let ntObj = {Region: 'NT', Address: tdList[1].textContent.trim(), District: tdList[0].textContent.trim(), Phone: phone }
                                 hkArray.push(ntObj);
                                 }
                                 return hkArray.concat(klnArray,ntArray);
                                 }";

        /*
                                          const hkResult = hkList.map((v,i)=>{ const tdList = Array.from(v.querySelectorAll('td'));
                                 return{Region: 'HK', Address: tdList[1].textContent.trim(), District: tdList[0].textContent.trim(), Phone: tdList[2].textContent.trim() }})
                                 return hkResult;
         */
        private string waitSelector = ".bg_clr_w";

        public CSLGrabber(PuppeteerConnection puppeteerConnection, IOptions<CSLOptions> options, IMemoryCache memoryCache, ILogger<CSLGrabber> logger)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task GetWebSiteItems()
        {
            var enScript = await _puppeteerConnection.PuppeteerGrabber<CSLModel[]>(_options.Value.EnUrl, infoCode, waitSelector);
            var zhScript = await _puppeteerConnection.PuppeteerGrabber<CSLModel[]>(_options.Value.ZhUrl, infoCode, waitSelector);
            var enResultList = enScript.ToList();
            var zhResultList = zhScript.ToList();
        }

        /*
        public List<IGeoComGrabModel> MergeEnAndZh(List<CSLModel>? enResult, List<CSLModel>? zhResult)
        {
   
            List<IGeoComGrabModel> CSLIGeoComList = new List<IGeoComGrabModel>();
            try
            {
                foreach (var item in enResult.Select((value, i) => new { i, value }))
                {
                    var shopEn = item.value;
                    var index = item.i;
                    IGeoComGrabModel CSLIGeoCom = new IGeoComGrabModel();
                    shopEn.ID = index;
                    CSLIGeoCom.E_Address = shopEn.Address;
                    CSLIGeoCom.E_District;

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        */
    }
}
