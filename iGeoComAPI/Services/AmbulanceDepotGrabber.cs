using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class AmbulanceDepotGrabber
    {
        private PuppeteerConnection _puppeteerConnection;
        private IOptions<AmbulanceDepotOptions> _options;
        private ILogger<AmbulanceDepotGrabber> _logger;
        private LatLngFunction _function;
        private string infoCode = @"()=>{
                                 const selectors = Array.from(document.querySelectorAll('.navigation > .content > .table > .content > .row'));
                                 return selectors.map(v1=>{ 
                                 const info = Array.from(v1.querySelectorAll('div'));
                                 return {Name: info[1].textContent.trim(), Address: info[2].textContent.trim(),Email: info[2].querySelector('a').textContent.trim(),
                                 Phone: info[3].textContent.trim(), Fax: info[4].textContent.trim()};
                                 })
                                 }";
        private string infoCode2 = @"()=>{
                                 const selectors = Array.from(document.querySelectorAll('.navigation > .content > .table'));
                                 return array = selectors.map(v1=>{ let shopInfo = Array.from(v1.querySelectorAll('.content > .row'));
                                 return shopInfo.map(v2 =>{return {Name: v2.querySelector('.name').textContent.trim(), Address: Array.from(v2.querySelectorAll('.col-md-8 > div'))[1].textContent.trim(), 
                                 Phone: Array.from(v2.querySelectorAll('div'))[1].textContent.trim(), Fax: Array.from(v2.querySelectorAll('div'))[2].textContent.trim(),
                                 Region: v1.querySelector('.col-md-12').textContent.trim(), Id: v1.getAttribute('.id')}});
                                 })
                                 }";
        private string waitSelector = ".navigation";

        //public AmbulanceDepotGrabber(PuppeteerConnection puppeteerConnection, IOptions<AmbulanceDepotOptions> options, ILogger<AmbulanceDepotGrabber> logger, LatLngFunction function,
        //    IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json) : base(httpClient, absOptions, json)
        //{
        //    _puppeteerConnection = puppeteerConnection;
        //    _options = options;
        //    _logger = logger;
        //    _function = function;
        //}
        public AmbulanceDepotGrabber(PuppeteerConnection puppeteerConnection, IOptions<AmbulanceDepotOptions> options, ILogger<AmbulanceDepotGrabber> logger, LatLngFunction function,
    IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _logger = logger;
            _function = function;
        }

        public async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enResult = await _puppeteerConnection.PuppeteerGrabber<AmbulanceDepotModel[]>(_options.Value.EnUrl, infoCode, waitSelector);
            var zhResult = await _puppeteerConnection.PuppeteerGrabber<AmbulanceDepotModel[]>(_options.Value.ZhUrl, infoCode, waitSelector);
            var enResultList = enResult.ToList();
            var zhResultList = zhResult.ToList();
            var mergeResult = await MergeEnAndZh(enResultList, zhResultList);
            return mergeResult;
        }

       

        public async Task<List<IGeoComGrabModel>> MergeEnAndZh(List<AmbulanceDepotModel> enResult, List<AmbulanceDepotModel> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge AmbulanceDepot En and Zh");
                List<IGeoComGrabModel> AmbulanceDepotIGeoComList = new List<IGeoComGrabModel>();
                foreach (var item in enResult.Take(50).Select((value, i) => new { i, value }))
                {
                    var shopEn = item.value;
                    var index = item.i;
                    IGeoComGrabModel AmbulanceDepotIGeoCom = new IGeoComGrabModel();
                    AmbulanceDepotIGeoCom.EnglishName = shopEn.Name;
                    AmbulanceDepotIGeoCom.E_Address = shopEn.Address.Replace(shopEn.Email,"");
                    AmbulanceDepotIGeoCom.Tel_No = shopEn.Phone;
                    AmbulanceDepotIGeoCom.Fax_No = shopEn.Fax;
                    AmbulanceDepotIGeoCom.GrabId = $"ambulanceDepot_{shopEn.Fax}{index}".Replace(" ","");
                    foreach (var shopZh in zhResult)
                    {
                        if (shopEn.Phone == shopZh.Phone && shopEn.Fax == shopZh.Fax)
                        {
                            AmbulanceDepotIGeoCom.ChineseName = shopZh.Name;
                            AmbulanceDepotIGeoCom.C_Address = shopZh.Address.Replace(shopZh.Email, "").Replace(" ", "");
                            var cFloor = Regexs.ExtractC_Floor().Matches(AmbulanceDepotIGeoCom.C_Address);
                            if (cFloor.Count > 0 && cFloor != null)
                            {
                                AmbulanceDepotIGeoCom.C_floor = cFloor[0].Value;
                            }
                            var latlng =  await _function.FindLatLngByAddress($"消防局{AmbulanceDepotIGeoCom.C_Address}");
                            AmbulanceDepotIGeoCom.Latitude = latlng.Latitude;
                            AmbulanceDepotIGeoCom.Longitude = latlng.Longtitude;
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
