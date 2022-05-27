namespace iGeoComAPI.Models
{
    public class VisitUs
    {
        public Data? data { get; set; }

        public class Data
        {
            public List<ShopCentreLocation>? shopCentreLocationList { get; set; }
            public List<CarParkFacilityLocation>? carParkFacilityLocationList { get; set; }
            public List<MarketLocation>? marketLocationList { get; set; }
        }
    }

    public class ShopCentreLocation
    {
        public string shopCentreId { get; set; } = string.Empty;
        public float latitude { get; set; }
        public float longitude { get; set; }
        public int districtId { get; set; }
    }

    public class CarParkFacilityLocation
    {
        public string facilityKey { get; set; }= string .Empty;
        public float latitude { get; set; }
        public float longitude { get; set; }
        public int districtId { get; set; }
    }

    public class MarketLocation
    {
        public string marketCode { get; set; } = String.Empty;
        public float latitude { get; set; }
        public float longitude { get; set; }
        public int districtId { get; set; }
    }
    

    public class Parking
    {
        public Data? data { get; set; }
        public class Data
        {
            public ParkingInfo? parkinginfo { get; set; }
        }
        public class ParkingInfo
        {
            public string facilityKey { get; set; } = String.Empty;
            public string remarkTc { get; set; } = String.Empty;
            public string carParkFacilityNameTc { get; set; } = String.Empty;
            public string carParkFacilityNameEn { get; set; } = String.Empty;
            public string addressEn { get; set; } = String.Empty;
            public string addressTc { get; set; } = String.Empty;
            public string telephone { get; set; } = String.Empty;

        }
    }

    public class MarketCode
    {
        public Data? data { get; set; }
        public class Data
        {
            public SelectedMarket? selectedMarket { get; set; }
        }

        public class SelectedMarket
        {
            public string marketCode { get; set; } = String.Empty;
            public string marketNameEn { get; set; } = String.Empty;
            public string marketNameTc { get; set; } = String.Empty;
            public string addressEn { get; set; } = String.Empty;
            public string addressTc { get; set; } = String.Empty;
        }

    }

    public class ShopCentre
    {
        public Data? data { get; set; }
        public class Data
        {
            public SelectedShopCentre? selectedShopCentre { get; set; }
        }
        public class SelectedShopCentre
        {
            public string ShopCentreId { get; set; } = String.Empty;
            public string shopCentreNameEn { get; set; } = String.Empty;
            public string shopCentreNameTc { get; set; } = String.Empty;
            public string addressEn { get; set; } = String.Empty;
            public string addressTc { get; set; } = String.Empty;
        }
    }
}
