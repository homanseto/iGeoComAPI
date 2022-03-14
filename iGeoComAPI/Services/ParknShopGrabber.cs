using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class ParknShopGrabber : IGrabberAPI<ParknShopModel>
    {
        private PuppeteerConnection _puppeteerConnection;
        private IOptions<ParknShopOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<ParknShopGrabber> _logger;
        private string infoCode = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll('.result-list > .shop-list > .iscroll-object > .scroll > .location-info'));
                                 return selectors.map(v => {return {BrandName: v.getAttribute('data-brandname').trim(), Region: v.getAttribute('data-region').trim(),
                                 District: v.getAttribute('data-district').trim(), Latitude: v.getAttribute('data-latitude').trim(), Longitude: v.getAttribute('data-longitude').trim(),
                                 Phone: v.querySelector('.detail > .info > .phone').textContent.trim(), Address: v.querySelector('.detail > .info > .address').textContent.trim(),
                                 Name: v.querySelector('.detail > .info > .shop-name').textContent.trim()
                                 }});
                                 }";
        private string waitSelector = ".result-list";

        public ParknShopGrabber(PuppeteerConnection puppeteerConnection, IOptions<ParknShopOptions> options, IMemoryCache memoryCache, ILogger<ParknShopGrabber> logger)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        public async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enResult = await _puppeteerConnection.PuppeteerGrabber<ParknShopModel>(_options.Value.EnUrl, infoCode, waitSelector);
            var zhResult = await _puppeteerConnection.PuppeteerGrabber<ParknShopModel>(_options.Value.ZhUrl, infoCode, waitSelector);
            var enResultList = enResult.ToList();
            var zhResultList = zhResult.ToList();
            var mergeResult = MergeEnAndZh(enResultList, zhResultList);
            //_memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
            return mergeResult;
        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<ParknShopModel> enResult, List<ParknShopModel> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge ParknShop En and Zh");
                List<IGeoComGrabModel> ParknShopIGeoComList = new List<IGeoComGrabModel>();
                foreach (var shopEn in enResult)
                {
                    IGeoComGrabModel ParknShopIGeoCom = new IGeoComGrabModel();
                    ParknShopIGeoCom.E_Address = shopEn.Address;
                    ParknShopIGeoCom.EnglishName = $"{shopEn.BrandName}-{shopEn.Name}";
                    ParknShopIGeoCom.E_Region = shopEn.Region;
                    ParknShopIGeoCom.E_District = shopEn.District;
                    ParknShopIGeoCom.Latitude = shopEn.Latitude;
                    ParknShopIGeoCom.Longitude = shopEn.Longitude;
                    ParknShopIGeoCom.Tel_No = shopEn.Phone;
                    ParknShopIGeoCom.Class = "CMF";
                    ParknShopIGeoCom.Type = "SMK";
                    ParknShopIGeoCom.Grab_ID = $"parknshop_{shopEn.BrandName}{shopEn.Latitude}{shopEn.Longitude}{shopEn.Phone}".Replace(".", "").Replace(" ","");
                    ParknShopIGeoCom.Web_Site = _options.Value.BaseUrl;
                    foreach (var shopZh in zhResult)
                    {
                        if (shopEn.Latitude == shopZh.Latitude && shopEn.Longitude == shopZh.Longitude && shopEn.Phone == shopZh.Phone)
                        {
                            ParknShopIGeoCom.ChineseName = $"{shopZh.BrandName}-{shopZh.Name}";
                            ParknShopIGeoCom.C_Region = shopZh.Region;
                            ParknShopIGeoCom.C_Address = shopZh.Address;
                            ParknShopIGeoCom.C_District = shopZh.District;
                            break;
                        }
                    }
                    ParknShopIGeoComList.Add(ParknShopIGeoCom);
                }
                return ParknShopIGeoComList;
            }catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge ParknShop En and Zh");
                throw;
            }
            
        }
    }
}
