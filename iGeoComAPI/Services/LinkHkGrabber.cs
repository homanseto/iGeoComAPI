using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using iGeoComAPI.Repository;

namespace iGeoComAPI.Services
{
    public class LinkHkGrabber : AbstractGrabber
    {
        //private readonly IOptions<SevenElevenOptions> _options;
        private readonly ConnectClient _httpClient;
        private readonly JsonFunction _json;
        private readonly IOptions<LinkHkOptions> _options;
        private readonly MyLogger _logger;
        private readonly IGeoComGrabRepository _iGeoComGrabRepository;
        private readonly IDataAccess dataAccess;

        public LinkHkGrabber(ConnectClient httpClient, JsonFunction json, IOptions<LinkHkOptions> options, MyLogger logger, IOptions<NorthEastOptions> absOptions, IGeoComGrabRepository iGeoComGrabRepository, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _httpClient = httpClient;
            _json = json;
            _options = options;
            _logger = logger;
            _iGeoComGrabRepository = iGeoComGrabRepository;
        }

        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            try
            {
                var visitUs = await _httpClient.GetAsync(_options.Value.LinkAPI);
                var visitUsResult = _json.Dserialize<VisitUs>(visitUs);
                var test = new List<IGeoComGrabModel>();

                return test;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<List<IGeoComGrabModel>?> Parsing(VisitUs input)
        {
            List<ShopCentreLocation> shopCentreList;
            var LinkHkList = new List<IGeoComGrabModel>();
            string Zhname = "LinkReit";
            if (input != null)
            {
                IGeoComGrabModel LinkHk = new IGeoComGrabModel();
                shopCentreList = input.data.shopCentreLocationList;
                foreach(var shop in shopCentreList)
                {
                    var shopCentreId = new Dictionary<string, string>(){ ["shopCentreId"] = shop.shopCentreId};
                    var shopGrabResult = await _httpClient.GetAsync(_options.Value.LinkMarketAndShopCentralAPI, shopCentreId);
                    var shopResult = _json.Dserialize<ShopCentre>(shopGrabResult);
                    LinkHk.Latitude = shop.latitude;
                    LinkHk.Longitude = shop.longitude;
                    LinkHk.C_Address = shopResult.data.selectedShopCentre.addressTc;
                    LinkHk.E_Address = shopResult.data.selectedShopCentre.addressEn;
                    LinkHk.ChineseName = $"{Zhname}_shopcentre_{shop.shopCentreId}";
                }
            }


            return LinkHkList;
        }
    }
}
