namespace iGeoComAPI.Models
{
    public class ChinaMobileModel
    {
        public string? Id { get; set; }
        public string? Region { get; set; }
        public string? Address { get; set; }
        public string?  LagLng { get; set; }
        public string ExtractLink
        {
            get { return @"open\('(.*)','map_district'";  }
        }
    }
}
