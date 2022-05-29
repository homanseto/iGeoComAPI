using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class AmbulanceDepotGrabber: AbstractGrabber
    {
        private PuppeteerConnection _puppeteerConnection;
        private IOptions<AmbulanceDepotOptions> _options;
        private ILogger<AmbulanceDepotGrabber> _logger;
        private LatLngFunction _function;
        private readonly IDataAccess dataAccess;
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

        public AmbulanceDepotGrabber(PuppeteerConnection puppeteerConnection, IOptions<AmbulanceDepotOptions> options, ILogger<AmbulanceDepotGrabber> logger, LatLngFunction function,
            IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _logger = logger;
            _function = function;
        }

        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enResult = await _puppeteerConnection.PuppeteerGrabber<AmbulanceDepotModel[]>(_options.Value.EnUrl, infoCode, waitSelector);
            var zhResult = await _puppeteerConnection.PuppeteerGrabber<AmbulanceDepotModel[]>(_options.Value.ZhUrl, infoCode, waitSelector);
            var enResultList = enResult.ToList();
            var zhResultList = zhResult.ToList();
            var mergeResult = MergeEnAndZh(enResultList, zhResultList);
            var latlngResult = await FindLatLng(mergeResult);
            var result = await this.GetShopInfo(latlngResult);
            return result;
        }

       

        public List<IGeoComGrabModel> MergeEnAndZh(List<AmbulanceDepotModel> enResult, List<AmbulanceDepotModel> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge AmbulanceDepot En and Zh");
                List<IGeoComGrabModel> AmbulanceDepotIGeoComList = new List<IGeoComGrabModel>();
                foreach (var item in enResult.Select((value, i) => new { i, value }))
                {
                    var shopEn = item.value;
                    var index = item.i;
                    IGeoComGrabModel AmbulanceDepotIGeoCom = new IGeoComGrabModel();
                    AmbulanceDepotIGeoCom.EnglishName = shopEn.Name;
                    AmbulanceDepotIGeoCom.E_Address = shopEn.Address.Replace(shopEn.Email,"");
                    AmbulanceDepotIGeoCom.Tel_No = shopEn.Phone;
                    AmbulanceDepotIGeoCom.Fax_No = shopEn.Fax;
                    AmbulanceDepotIGeoCom.Class = "GOV";
                    AmbulanceDepotIGeoCom.Type = "FSN";
                    AmbulanceDepotIGeoCom.Shop = 9;
                    AmbulanceDepotIGeoCom.Source = "22";
                    AmbulanceDepotIGeoCom.Web_Site = _options.Value.BaseUrl;
                    AmbulanceDepotIGeoCom.GrabId = $"ambulanceDepot_{shopEn.Fax}{index}".Replace(" ","");
                    foreach (var shopZh in zhResult)
                    {
                        if (shopEn.Email == shopZh.Email)
                        {
                            AmbulanceDepotIGeoCom.ChineseName = shopZh.Name;
                            AmbulanceDepotIGeoCom.C_Address = shopZh.Address.Replace(shopZh.Email, "").Replace(" ", "");
                        }
                    }
                    AmbulanceDepotIGeoComList.Add(AmbulanceDepotIGeoCom);
                }
                var result = AmbulanceDepotIGeoComList.GroupBy(x => x.GrabId).Select(x => x.First()).ToList();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge Wellcome En and Zh");
                throw;
            }
        }
        public async Task<List<IGeoComGrabModel>> FindLatLng(List<IGeoComGrabModel> input)
        {
            foreach (var inputItem in input)
            {
                var latlng = await _function.FindLatLngByAddress($"消防局{inputItem.C_Address}");
                inputItem.Latitude = latlng.Latitude;
                inputItem.Longitude = latlng.Longtitude;
            }
            return input;
        }
    }
}
