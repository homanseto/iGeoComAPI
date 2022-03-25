using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class BmcpcGrabber
    {
        private PuppeteerConnection _puppeteerConnection;
        private IOptions<BmcpcOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<BmcpcGrabber> _logger;
        private string infoCode = @"()=>{
                                 const selectorName = Array.from(document.querySelectorAll('#mainContent > #content > h3'));
                                 const selectorInfo = Array.from(document.querySelectorAll('#mainContent > #content > p'));
                                 return selectorName.map((v,i)=>{ return {Info: selectorInfo[i+3].textContent.trim(), Name: v.textContent.trim() }});
                                 }";
        private string waitSelector = "#mainContent";
        BmcpcModel bmcpcModel = new BmcpcModel();
        

        public BmcpcGrabber(PuppeteerConnection puppeteerConnection, IOptions<BmcpcOptions> options, IMemoryCache memoryCache, ILogger<BmcpcGrabber> logger)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var zhResult = await _puppeteerConnection.PuppeteerGrabber<BmcpcModel[]>(_options.Value.ZhUrl, infoCode, waitSelector);
            var zhResultList = zhResult.ToList();
            return Parsing(zhResultList);
        } 

        
        public List<IGeoComGrabModel> Parsing(List<BmcpcModel> input)
        {
            var addressRgx = Regexs.ExtractInfo(bmcpcModel.AddressRegex);
            var phoneRgx = Regexs.ExtractInfo(bmcpcModel.PhoneRegex);
            List<IGeoComGrabModel> BmcpcIGeoComList = new List<IGeoComGrabModel>();
            foreach(var shop in input)
            {
                IGeoComGrabModel BmcpcIGeoCom = new IGeoComGrabModel();
                BmcpcIGeoCom.ChineseName = shop.Name;
                BmcpcIGeoCom.C_Address = addressRgx.Match(shop.Info!).Groups[1].Value;
                BmcpcIGeoCom.Tel_No = phoneRgx.Match(shop.Info!).Groups[1].Value;
                BmcpcIGeoCom.Class = "BGD";
                BmcpcIGeoCom.Type = "CEM";
                BmcpcIGeoCom.Web_Site = _options.Value.BaseUrl;
                BmcpcIGeoComList.Add(BmcpcIGeoCom);
            }
            return BmcpcIGeoComList;
        }
        

    }
}
