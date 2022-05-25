namespace iGeoComAPI.Models
{
    public class SevenElevenModel
    {
        public string LatLng { get; set; } = String.Empty;
        public string Address { get; set; } = String.Empty;
        public string Opening_24 { get; set; } = String.Empty;
        public string Opening_Weekday { get; set; } = String.Empty;
        public string Daily_Cafe { get; set; } = String.Empty;
        public string District { get; set; } = String.Empty;
        public static string RegLatLngRegex
        {
            get { return "([^|]*)"; }
        } 
    }
}
