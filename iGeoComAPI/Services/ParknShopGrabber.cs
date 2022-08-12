using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class ParknShopGrabber : AbstractGrabber
    {
        private PuppeteerConnection _puppeteerConnection;
        private IOptions<ParknShopOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<ParknShopGrabber> _logger;
        private readonly IDataAccess dataAccess;

        private string infoCode = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll('.result-list > .shop-list > .iscroll-object > .scroll > .location-info'));
                                 return selectors.map(v => {return {BrandName: v.getAttribute('data-brandname').trim(), Region: v.getAttribute('data-region').trim(),
                                 District: v.getAttribute('data-district').trim(), Latitude: v.getAttribute('data-latitude').trim(), Longitude: v.getAttribute('data-longitude').trim(),
                                 Phone: v.querySelector('.detail > .info > .phone').textContent.trim(), Address: v.querySelector('.detail > .info > .address').textContent.trim(),
                                 Name: v.querySelector('.detail > .info > .shop-name').textContent.trim()
                                 }});
                                 }";
        private string waitSelector = ".result-list";

        public ParknShopGrabber(PuppeteerConnection puppeteerConnection, IOptions<ParknShopOptions> options, IMemoryCache memoryCache, ILogger<ParknShopGrabber> logger,
            IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enResult = await _puppeteerConnection.PuppeteerGrabber<ParknShopModel[]>(_options.Value.EnUrl, infoCode, waitSelector);
            var zhResult = await _puppeteerConnection.PuppeteerGrabber<ParknShopModel[]>(_options.Value.ZhUrl, infoCode, waitSelector);
            var enResultList = enResult.ToList();
            var zhResultList = zhResult.ToList();
            var mergeResult = MergeEnAndZh(enResultList, zhResultList);
            var result = await this.GetShopInfo(mergeResult);
            //_memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
            return result;
        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<ParknShopModel> enResult, List<ParknShopModel> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge ParknShop En and Zh");
                List<IGeoComGrabModel> ParknShopIGeoComList = new List<IGeoComGrabModel>();
                foreach (var item in enResult.Select((value, i) => new { i, value }))
                {
                    var shopEn = item.value;
                    var index = item.i;
                    IGeoComGrabModel ParknShopIGeoCom = new IGeoComGrabModel();
                    ParknShopIGeoCom.E_Address = shopEn.Address;
                    ParknShopIGeoCom.EnglishName = $"{shopEn.BrandName}-{shopEn.Name}";
                    ParknShopIGeoCom.E_District = shopEn.District;
                    ParknShopIGeoCom.Latitude = Convert.ToDouble(shopEn.Latitude);
                    ParknShopIGeoCom.Longitude = Convert.ToDouble(shopEn.Longitude);
                    ParknShopIGeoCom.Tel_No = shopEn.Phone;
                    ParknShopIGeoCom.Type = "SMK";
                    ParknShopIGeoCom.Class = "CMF";
                    ParknShopIGeoCom.Web_Site = _options.Value.BaseUrl;
                    ParknShopIGeoCom.GrabId = $"parknshop{shopEn.BrandName}{index}";
                    foreach (var shopZh in zhResult)
                    {
                        if (shopEn.Latitude == shopZh.Latitude && shopEn.Longitude == shopZh.Longitude && shopEn.Phone == shopZh.Phone && shopEn.BrandName == shopZh.BrandName)
                        {
                            ParknShopIGeoCom.ChineseName = $"{shopZh.BrandName}-{shopZh.Name}";
                            ParknShopIGeoCom.C_Address = shopZh.Address.Replace(" ", "");
                            break;
                        }
                    }
                    ParknShopIGeoComList.Add(ParknShopIGeoCom);
                }
                List<IGeoComGrabModel> ReplacedDuplicated = ParknShopIGeoComList.GroupBy(x => x.E_Address).Select(x => x.First()).GroupBy(x => x.C_Address).Select(x => x.First()).ToList();
                return ReplacedDuplicated.Where(shop => shop.E_District.ToLower() != "macau").ToList();
            }catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge ParknShop En and Zh");
                throw;
            }
            
        }
    }
}
