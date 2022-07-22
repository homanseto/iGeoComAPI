namespace iGeoComAPI.Models
{
    public class MarketPlaceModel
    {
        public string id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string tel_no { get; set; } = string.Empty;
        public string openingHour { get; set; } = string.Empty;
        public double latitude { get; set; } 
        public double longitude { get; set; }
    }
}
