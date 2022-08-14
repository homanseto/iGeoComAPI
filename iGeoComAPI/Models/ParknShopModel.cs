namespace iGeoComAPI.Models
{
    public class ParknShopModel
    {
        public double boundEastLongitude { get; set; }
        public double boundNorthLatitude { get; set; }
        public double boundSouthLatitude { get; set; }
        public double boundWestLongitude { get; set; }
        public Pagination pagination { get; set; }
        public double sourceLatitude { get; set; }
        public double sourceLongitude { get; set; }
        public List<Store> stores { get; set; }

    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Address
    {
        public bool billingAddress { get; set; }
        public bool contactAddress { get; set; }
        public Country country { get; set; }
        public string county { get; set; }
        public bool defaultAddress { get; set; }
        public string displayAddress1 { get; set; }
        public string displayAddress2 { get; set; }
        public string district { get; set; }
        public string formattedAddress { get; set; }
        public string id { get; set; }
        public string line1 { get; set; }
        public string phone { get; set; }
        public string postalCode { get; set; }
        public bool shippingAddress { get; set; }
        public string town { get; set; }
        public bool visibleInAddressBook { get; set; }
        public string province { get; set; }
    }

    public class ClosingTime
    {
        public int hour { get; set; }
        public int minute { get; set; }
    }

    public class Country
    {
        public string isocode { get; set; }
        public string name { get; set; }
    }

    public class ElabFeature
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class GeoPoint
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    public class MarkerLogo
    {
        public string catalogId { get; set; }
        public string catalogVersion { get; set; }
        public string code { get; set; }
        public string downloadUrl { get; set; }
        public string mime { get; set; }
        public string url { get; set; }
        public string uuid { get; set; }
    }

    public class OpeningHours
    {
        public string code { get; set; }
        public string name { get; set; }
        public List<object> pharmacyOpeningList { get; set; }
        public List<object> specialDayOpeningList { get; set; }
        public List<WeekDayOpeningList> weekDayOpeningList { get; set; }
    }

    public class OpeningTime
    {
        public int hour { get; set; }
        public int minute { get; set; }
    }

    public class Pagination
    {
        public int currentPage { get; set; }
        public int pageSize { get; set; }
        public int totalPages { get; set; }
        public int totalResults { get; set; }
    }


    public class SmallLogo
    {
        public string catalogId { get; set; }
        public string catalogVersion { get; set; }
        public string code { get; set; }
        public string downloadUrl { get; set; }
        public string mime { get; set; }
        public string url { get; set; }
        public string uuid { get; set; }
    }

    public class Store
    {
        public Address address { get; set; }
        public string description { get; set; }
        public string displayName { get; set; }
        public double distanceKm { get; set; }
        public bool elabFastDelivery { get; set; }
        public List<ElabFeature> elabFeatures { get; set; }
        public bool elabPayCollect { get; set; }
        public string elabStoreNumber { get; set; }
        public bool familyMartStore { get; set; }
        public string formattedDistance { get; set; }
        public GeoPoint geoPoint { get; set; }
        public bool hiLifeStore { get; set; }
        public bool inStock { get; set; }
        public string mapUrl { get; set; }
        public MarkerLogo markerLogo { get; set; }
        public string name { get; set; }
        public bool okStore { get; set; }
        public OpeningHours openingHours { get; set; }
        public bool pickupExpressEligible { get; set; }
        public bool pickupInStoreAvailable { get; set; }
        public string seoName { get; set; }
        public SmallLogo smallLogo { get; set; }
        public string storeContent { get; set; }
        public List<object> storeImages { get; set; }
        public StoreLogo storeLogo { get; set; }
        public string url { get; set; }
        public bool watsonStore { get; set; }
    }

    public class StoreLogo
    {
        public string catalogId { get; set; }
        public string catalogVersion { get; set; }
        public string code { get; set; }
        public string downloadUrl { get; set; }
        public string mime { get; set; }
        public string url { get; set; }
        public string uuid { get; set; }
    }

    public class WeekDayOpeningList
    {
        public ClosingTime closingTime { get; set; }
        public OpeningTime openingTime { get; set; }
        public bool closed { get; set; }
        public string note { get; set; }
        public string weekDay { get; set; }
    }


}

