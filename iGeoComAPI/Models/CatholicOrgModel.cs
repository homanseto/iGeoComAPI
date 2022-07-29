namespace iGeoComAPI.Models
{
    public class CatholicOrgModel
    {
        public string id { get; set; } = String.Empty;
        public string name { get; set; } = String.Empty;
        public string address { get; set; } = String.Empty;
        public string href { get; set; } = String.Empty;
        public string phone { get; set; } = String.Empty;
        public string fax { get; set; } = String.Empty;
        public LatLng ?latlng { get; set; }
    }

    public class LatLng
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }
}
