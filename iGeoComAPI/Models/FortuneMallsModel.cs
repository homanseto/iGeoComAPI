namespace iGeoComAPI.Models
{
    public class FortuneMallsModel
    {
        public string id { get; set; } = String.Empty;
        public string href { get; set; } = String.Empty;
        public string address { get; set; } = String.Empty;
        public string number { get; set; } = String.Empty;
        public List<string> infoList { get; set; } = new List<string>() { };
        public string name { get; set; } = String.Empty;
        public double latitude { get; set; }
        public double longitude { get; set; }
        public static string extractId
        {
            get { return "changeMall\\('(?<id>.*)', 'transportation'"; }
        }
        public static string extractLatLng
        {
            get { return "ll=(?<latlng>.*)&z"; }
        }

    }
}
