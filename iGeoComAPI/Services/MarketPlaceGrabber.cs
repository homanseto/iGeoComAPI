using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class MarketPlaceGrabber: AbstractGrabber
    {
        private PuppeteerConnection _puppeteerConnection;
        private IOptions<MarketPlaceOptions> _options;
        private ILogger<MarketPlaceGrabber> _logger;
        private LatLngFunction _function;
        private readonly IDataAccess dataAccess;

        private readonly string infoCode = @"() =>{" +
            @"const selectors = Array.from(document.querySelectorAll('.geolocation-location'));" +
            @"return selectors.map(v => {return {id: v.getAttribute('data-scroll-target-id')," +
            @"address: v.querySelector('.location-content > .views-field-field-address > .field-content').textContent.trim(), tel_no: v.querySelector('.views-field-field-telephone > .field-content').textContent.trim()," +
            @"name: v.querySelector('.location-title').textContent.trim(), latitude: v.getAttribute('data-lat'), longitude: v.getAttribute('data-lng')" +
            @"}});}";
        private string waitSelector = ".view-content";

        public MarketPlaceGrabber(PuppeteerConnection puppeteerConnection, IOptions<MarketPlaceOptions> options, ILogger<MarketPlaceGrabber> logger,
            IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, LatLngFunction function, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _logger = logger;
            _function = function;
        }

        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enResult = await _puppeteerConnection.PuppeteerGrabber<List<MarketPlaceModel>>(_options.Value.EnUrl, infoCode, waitSelector);
            var zhResult = await _puppeteerConnection.PuppeteerGrabber<List<MarketPlaceModel>>(_options.Value.ZhUrl, infoCode, waitSelector);
            var mergeResult = MergeEnAndZh(enResult, zhResult);
            //var result = await this.GetShopInfo(mergeResult);
            //_memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
            return mergeResult;
        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<MarketPlaceModel> enResult, List<MarketPlaceModel> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge Market Place En and Zh");
                List<IGeoComGrabModel> MarketPlaceIGeoComList = new List<IGeoComGrabModel>();
                string name = "Market Place";
                foreach (var item in enResult.Select((value, i) => new { i, value }))
                {
                    var shopEn = item.value;
                    var index = item.i;
                    IGeoComGrabModel MarketPlaceIGeoCom = new IGeoComGrabModel();
                    MarketPlaceIGeoCom.GrabId = $"{name}{shopEn.id}";
                    MarketPlaceIGeoCom.EnglishName = $"{name} {shopEn.name}";
                    MarketPlaceIGeoCom.E_Address = shopEn.address;
                    MarketPlaceIGeoCom.Tel_No = shopEn.tel_no;
                    MarketPlaceIGeoCom.Type = "SMK";
                    MarketPlaceIGeoCom.Class = "CMF";
                    MarketPlaceIGeoCom.ShopId = "smk4";
                    MarketPlaceIGeoCom.Latitude = shopEn.latitude;
                    MarketPlaceIGeoCom.Longitude = shopEn.longitude;
                    foreach (var shopZh in zhResult)
                    {
                        if (shopEn.id == shopZh.id)
                        {
                            MarketPlaceIGeoCom.ChineseName = $"{name} {shopZh.name}";
                            MarketPlaceIGeoCom.C_Address = shopZh.address;
                            break;
                        }
                    }
                    MarketPlaceIGeoComList.Add(MarketPlaceIGeoCom);
                }
                return MarketPlaceIGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge MarketPlace En and Zh");
                throw;
            }

        }
    }
}
