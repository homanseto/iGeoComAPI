namespace iGeoComAPI.Models
{
    public class PeoplesPlaceModel
    {
        public string name { get; set; } = String.Empty;
        public string address { get; set; } = String.Empty;
        public string number { get; set; } = String.Empty;
        public string additionInfo { get; set; } = String.Empty;
        public string latlng { get; set; } = String.Empty;
        public string href { get; set; } = String.Empty;
        public static string ExtraLatLng
        {
            get { return "(ViewportInfoService.GetViewportInfo(?<value1>.*)nonce=\"\"><\\/script><\\/head>)|(ViewportInfoService.GetViewportInfo(?<value2>.*)AuthenticationService.Authenticate)"; }
        }
        public static string ExtraLat
        {
            get { return "2m2&amp;1d(?<lat>.*)&amp;2d";  }
        }
        public static string ExtraLng
        {
            get { return "&amp;2d(?<lng>.*)&amp;2m2&amp;"; }
        }
    }
}
