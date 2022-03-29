namespace iGeoComAPI.Models
{
    public class ChinaMobileModel
    {
        public string? Id { get; set; }
        public string? Region { get; set; }
        public string? Address { get; set; }
        public string?  LatLng { get; set; }
        public string ExtractLink
        {
            get { return @"open\('(.*)','map_district'";  }
        }
        public string ExtractAddressAndOpeningHour
        {
            get { return @"<\/b><br\/>(.*)<br><br\/>"; }
        }
        public string ExtractLatLng
        {
            get { return @"LatLng\((.*)\),mapTypeId"; }
        }
        public string ExtractId
        {
            get { return @"outletIdIQ=(.*)&lang";  }
        }
        public string RegLatLngRegex
        {
            get { return "([^,]*)"; }
        }
    }
}
