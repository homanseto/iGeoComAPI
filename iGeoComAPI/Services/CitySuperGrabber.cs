using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;

namespace iGeoComAPI.Services
{
    public class CitySuperGrabber : AbstractGrabber
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        private IOptions<CitySuperOptions> _options;
        private ILogger<CitySuperGrabber> _logger;
        private readonly IMemoryCache _memoryCache;

        private readonly string infoCode = @"() =>{" +
            @"const selectors = Array.from(document.querySelectorAll('.margin-bottom-20px > .top-xs '));" +
            @"return selectors.map(v =>{return {address: v.querySelector('.col-md-6 > p') === null ? '': v.querySelector('.col-md-6 > p').textContent.trim()," +
            @"name: v.querySelector('.font-cormorant-garamond') === null ? '': v.querySelector('.font-cormorant-garamond').textContent.trim() " +
            @"}})" +
            @"}";
        private string waitSelector = ".store-locator-container";



        public CitySuperGrabber(PuppeteerConnection puppeteerConnection, IOptions<CitySuperOptions> options, IMemoryCache memoryCache, ILogger<CitySuperGrabber> logger,
            IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enResult = await _puppeteerConnection.PuppeteerGrabber<List<CitySuperModel>>(_options.Value.EnUrl, infoCode, waitSelector);
            var zhResult = await _puppeteerConnection.PuppeteerGrabber<List<CitySuperModel>>(_options.Value.ZhUrl, infoCode, waitSelector);
            var mergeResult = MergeEnAndZh(enResult, zhResult);
            var result = await this.GetShopInfo(mergeResult);
            return result;
        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<CitySuperModel> enResult, List<CitySuperModel> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge CitySuper En and Zh");
                List<IGeoComGrabModel> CitySuperIGeoComList = new List<IGeoComGrabModel>();
                foreach (var item in enResult.Select((value, i) => new { i, value }))
                {
                    var shopEn = item.value;
                    var index = item.i;
                    IGeoComGrabModel CitySuperIGeoCom = new IGeoComGrabModel();
                    CitySuperIGeoCom.E_Address = shopEn.address!;
                    CitySuperIGeoCom.EnglishName = $"CitySuper {shopEn.name}";
                    CitySuperIGeoCom.Web_Site = _options.Value.BaseUrl!;
                    CitySuperIGeoCom.Type = "SMK";
                    CitySuperIGeoCom.Class = "CMF";
                    CitySuperIGeoCom.GrabId = $"CitySuper_";
                    foreach (var item2 in zhResult.Select((value2, i2) => new { i2, value2 }))
                    {
                        var shopZh = item2.value2;
                        var index2 = item2.i2;
                        if (shopEn.number == shopZh.number)
                        {
                            CitySuperIGeoCom.C_Address = shopZh.address!.Replace(" ", "");
                            CitySuperIGeoCom.ChineseName = $"CitySuper-{shopZh.name}";
                            continue;
                        }

                    }
                    CitySuperIGeoComList.Add(CitySuperIGeoCom);
                }
                return CitySuperIGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge CitySuper En and Zh");
                throw;
            }
        }
    }
}
