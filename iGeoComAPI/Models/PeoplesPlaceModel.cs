namespace iGeoComAPI.Models
{
    public class PeoplesPlaceModel
    {
        public string name { get; set; } = String.Empty;
        public string address { get; set; } = String.Empty;
        public List<string> number { get; set; } = new List<string>() { };
        public List<string> infoList { get; set; } = new List<string>() { };
        public string parkingInfo { get; set; } = String.Empty;
        public string latlng { get; set; } = String.Empty;
        public string href { get; set; } = String.Empty;
        public static string ExtractLat
        {
            get { return "maps\\?ll=(?<lat>.*),11";  }
        }
        public static string ExtractLng
        {
            get { return ",(?<lng>.*)&z"; }
        }
    }
}
