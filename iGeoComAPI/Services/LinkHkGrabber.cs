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
                VisitUs visitUsResult = _json.Dserialize<VisitUs>(visitUs);
                var result = Parsing(visitUsResult);
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
            var LinkHkList = new List<IGeoComGrabModel>();
            string name = "LinkReit";
            if (input != null)
            {

                List<MarketLocation> marketList = input.data.marketLocationList;
                List<ShopCentreLocation> shopCentreList = input.data.shopCentreLocationList;
                List<CarParkFacilityLocation> carparkList = input.data.carParkFacilityLocationList;

                foreach (var shop in shopCentreList)
                {
                    IGeoComGrabModel LinkHk = new IGeoComGrabModel();
                    var shopCentreId = new Dictionary<string, string>() { ["shopCentreId"] = shop.shopCentreId };
                    var shopGrabResult = await _httpClient.GetAsync(_options.Value.LinkMarketAndShopCentralAPI, shopCentreId);
                    var shopResult = _json.Dserialize<ShopCentre>(shopGrabResult);
                    LinkHk.Latitude = shop.latitude;
                    LinkHk.Longitude = shop.longitude;
                    if (shopResult != null)
                    {
                        LinkHk.C_Address = shopResult.data.selectedShopCentre.addressTc;
                        LinkHk.E_Address = shopResult.data.selectedShopCentre.addressEn;
                        LinkHk.EnglishName = shopResult.data.selectedShopCentre.shopCentreNameEn;
                        LinkHk.ChineseName = shopResult.data.selectedShopCentre.shopCentreNameTc;
                        LinkHk.GrabId = $"{name}_{shopResult.data.selectedShopCentre.ShopCentreId}";
                        LinkHk.Class = "CMF";
                        LinkHk.Type = "MAL";
                        LinkHk.Web_Site = _options.Value.BaseUrl;
                    }
                    LinkHkList.Add(LinkHk);
                }
                foreach (var market in marketList)
                {
                    IGeoComGrabModel LinkHk = new IGeoComGrabModel();
                    var marketCode = new Dictionary<string, string>() { ["marketCode"] = market.marketCode };
                    var marketGrabResult = await _httpClient.GetAsync(_options.Value.LinkMarketAndShopCentralAPI, marketCode);
                    var marketResult = _json.Dserialize<MarketCode>(marketGrabResult);
                    LinkHk.Latitude = market.latitude;
                    LinkHk.Longitude = market.longitude;
                    if (marketResult != null)
                    {
                        LinkHk.C_Address = marketResult.data.selectedMarket.addressTc;
                        LinkHk.E_Address = marketResult.data.selectedMarket.addressEn;
                        LinkHk.EnglishName = marketResult.data.selectedMarket.marketNameEn;
                        LinkHk.ChineseName = marketResult.data.selectedMarket.marketNameTc;
                        LinkHk.GrabId = $"{name}_{marketResult.data.selectedMarket.marketCode}";
                        LinkHk.Type = "MKT";
                        LinkHk.Class = "CMF";
                        LinkHk.Web_Site = _options.Value.BaseUrl;
                    }
                    LinkHkList.Add(LinkHk);
                }

                foreach (var carpark in carparkList)
                {
                    IGeoComGrabModel LinkHk = new IGeoComGrabModel();
                    var carparkGrabResult = await _httpClient.GetAsync($"{_options.Value.LinkCarParkAPI}{carpark.facilityKey}");
                    var carparkResult = _json.Dserialize<Parking>(carparkGrabResult);
                    if (carparkResult != null)
                    {
                        if(carparkResult.data.parkinginfo.remarkTc != "此停車場不設時租泊車服務")
                        {
                            LinkHk.Latitude = carpark.latitude;
                            LinkHk.Longitude = carpark.longitude;
                            LinkHk.C_Address = carparkResult.data.parkinginfo.addressTc;
                            LinkHk.E_Address = carparkResult.data.parkinginfo.addressEn;
                            LinkHk.EnglishName = carparkResult.data.parkinginfo.carParkFacilityNameEn;
                            LinkHk.ChineseName = carparkResult.data.parkinginfo.carParkFacilityNameTc;
                            LinkHk.Tel_No = carparkResult.data.parkinginfo.telephone;
                            LinkHk.GrabId = $"{name}_{carparkResult.data.parkinginfo.facilityKey}";
                            LinkHk.Type = "CPO";
                            LinkHk.Class = "TRS";
                            LinkHk.Web_Site = _options.Value.BaseUrl;
                            LinkHkList.Add(LinkHk);
                        }
                        else
                        {
                            continue;
                        }

                    }
                }
            }


            return LinkHkList;
        }
    }
}
