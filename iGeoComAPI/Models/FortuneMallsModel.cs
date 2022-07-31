namespace iGeoComAPI.Models
{
    public class FortuneMallsModel
    {
        public string id { get; set; } = String.Empty;
        public string href { get; set; } = String.Empty;
        public string address { get; set; } = String.Empty;
        public string name { get; set; } = String.Empty;
        public string number { get; set; } = String.Empty;
        public string latlng { get; set; } = String.Empty;
        public static string extractId
        {
            get { return "changeMall\\('(?<id>.*)', 'transportation'"; }
        }

    }
}
