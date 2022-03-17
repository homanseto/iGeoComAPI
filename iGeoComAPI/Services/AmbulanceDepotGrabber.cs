using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class AmbulanceDepotGrabber : IGrabberAPI<AmbulanceDepotModel>
    {
        private PuppeteerConnection _puppeteerConnection;
        private IOptions<AmbulanceDepotOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<AmbulanceDepotGrabber> _logger;
        private string infoCode = @"()=>{
                                 const selectors = Array.from(document.querySelectorAll('.navigation > .content > .table > .content > .row'));
                                 return selectors.map(v1=>{ 
                                 const info = Array.from(v1.querySelectorAll('div'));
                                 return {Name: info[1].textContent.trim(), Address: info[2].textContent.trim(),
                                 Phone: info[3].textContent.trim(), Fax: info[4].textContent.trim()};
                                 })
                                 }";
        private string infoCode2 = @"()=>{
                                 const selectors = Array.from(document.querySelectorAll('.navigation > .content > .table'));
                                 const array = selectors.map(v1=>{ let shopInfo = Array.from(v1.querySelectorAll('.content > .row'));
                                 return shopInfo.map(v2 =>{return {Name: v2.querySelector('.name').textContent.trim(), Address: Array.from(v2.querySelectorAll('.col-md-8 > div'))[1].textContent.trim(), 
                                 Phone: Array.from(v2.querySelectorAll('div'))[1].textContent.trim(), Fax: Array.from(v2.querySelectorAll('div'))[2].textContent.trim(),
                                 Region: v1.querySelector('.col-md-12').textContent.trim(), Id: v1.getAttribute('.id')}});
                                 return JsonConvert.DeserializeObject<List<AmbulanceDepotModel>>(array);
                                 })
                                 }";
        private string waitSelector = ".navigation";

        public AmbulanceDepotGrabber(PuppeteerConnection puppeteerConnection, IOptions<AmbulanceDepotOptions> options, IMemoryCache memoryCache, ILogger<AmbulanceDepotGrabber> logger)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enResult = await _puppeteerConnection.PuppeteerGrabber<AmbulanceDepotModel>(_options.Value.EnUrl, infoCode, waitSelector);
            var zhResult = await _puppeteerConnection.PuppeteerGrabber<AmbulanceDepotModel>(_options.Value.ZhUrl, infoCode, waitSelector);
            var enResultList = enResult.ToList();
            var zhResultList = zhResult.ToList();
            var mergeResult = MergeEnAndZh(enResultList, zhResultList);
            return mergeResult;
        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<AmbulanceDepotModel> enResult, List<AmbulanceDepotModel> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge AmbulanceDepot En and Zh");
                List<IGeoComGrabModel> AmbulanceDepotIGeoComList = new List<IGeoComGrabModel>();
                foreach (var shopEn in enResult)
                {
                    IGeoComGrabModel AmbulanceDepotIGeoCom = new IGeoComGrabModel();
                    AmbulanceDepotIGeoCom.EnglishName = shopEn.Name;
                    AmbulanceDepotIGeoCom.E_Address = shopEn.Address;
                    AmbulanceDepotIGeoCom.Tel_No = shopEn.Phone;
                    AmbulanceDepotIGeoCom.Fax_No = shopEn.Fax;
                    AmbulanceDepotIGeoCom.Grab_ID = $"ambulanceDepot_{shopEn.Fax}".Replace(" ","");
                    foreach (var shopZh in zhResult)
                    {
                        if (shopEn.Phone == shopZh.Phone && shopEn.Fax == shopZh.Fax)
                        {
                            AmbulanceDepotIGeoCom.ChineseName = shopZh.Name;
                            AmbulanceDepotIGeoCom.C_Address = shopZh.Address;
                        }
                    }
                    AmbulanceDepotIGeoComList.Add(AmbulanceDepotIGeoCom);
                }
                return AmbulanceDepotIGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge Wellcome En and Zh");
                throw;
            }
        }
    }
}
