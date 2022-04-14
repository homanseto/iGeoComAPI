namespace iGeoComAPI.Models
{
    public class ChinaMobileModel
    {
        public string Id { get; set; } = String.Empty;
        public string Region { get; set; } = String.Empty;
        public string Address { get; set; } = String.Empty;
        public string  LatLng { get; set; } = String.Empty;
        public static string ExtractLink
        {
            get { return @"open\('(.*)','map_district'";  }
        }
        public static string ExtractAddressAndOpeningHour
        {
            get { return @"<\/b><br\/>(.*)<br><br\/>"; }
        }
        public static string ExtractLatLng
        {
            get { return @"LatLng\((.*)\),mapTypeId"; }
        }
        public static string ExtractId
        {
            get { return @"outletIdIQ=(.*)&lang";  }
        }
        public static string RegLatLngRegex
        {
            get { return "([^,]*)"; }
        }
    }
}
