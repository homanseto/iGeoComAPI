namespace iGeoComAPI.Models
{
    public class PeoplesPlaceModel
    {
        public string name { get; set; } = String.Empty;
        public string address { get; set; } = String.Empty;
        public string number { get; set; } = String.Empty;
        public string parkingInfo { get; set; } = String.Empty;
        public string marketInfo1 { get; set; } = String.Empty;
        public string marketInfo2 { get; set; } = String.Empty;
        public string marketAddress1 { get; set; } = String.Empty;
        public string marketAddress2 { get; set; } = String.Empty;
        public string marketAddress3 { get; set; } = String.Empty;
        public string marketAddressEn1 { get; set; } = String.Empty;

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
