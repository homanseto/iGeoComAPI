namespace iGeoComAPI.Models
{
    public class ChurchModel
    {
        public string href { get; set; } = String.Empty;
        public string zhName { get; set; } = String.Empty;
        public string enName { get; set; } = String.Empty;
        public string address { set; get; } = String.Empty;
        public string number { set; get; } = String.Empty;
        public string fax { set; get; } = String.Empty;
        public string detail { set; get; } = String.Empty;
        public double latitude { set; get; }
        public double longitude { set; get; }

    }
}
