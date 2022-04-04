namespace iGeoComAPI.Models
{
    public class IGeoComDeltaModel :IGeoComModel
    {
        public string status { get; set; } = String.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
