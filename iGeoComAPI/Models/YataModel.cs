namespace iGeoComAPI.Models
{
    public class YataModel
    {
        public string href { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public string latlng { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string number { get; set; } = string.Empty;
        public static string ExtraLatLng
        {
            get { return "(\\/@(?<value1>.*)z\\/data)|(&ll=(?<value2>.*)&spn)"; }
        }
        public static string RegLatLng
        {
            get { return "([^,]*)"; }
        }
    }
}
