namespace iGeoComAPI.Models
{
    public class FEHDModel
    {
        public string type { get; set; } = String.Empty;
        public string mapID { get; set; } = String.Empty ;
        public string nameEN { get; set; } = String.Empty;
        public string nameTC { get; set; } = String.Empty;
        public string addressEN { get; set; } = String.Empty;
        public string addressTC { get; set; } = String.Empty;
        public string latitude { get; set; } = String.Empty;
        public string openHourEN { get; set; } = String.Empty;
        public static string RegLatLng
        {
            get { return "([^,]*)"; }
        }
    }
}
