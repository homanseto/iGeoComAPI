namespace iGeoComAPI.Options
{
    public class FEHDOptions
    {
        public const string SectionName = "FEHD";

        public string BaseUrl { get; set; } = String.Empty;
        public string districtType { get; set; } = String.Empty;
        public string mapType { get; set; } = String.Empty;
        public string marketType { get; set; } = String.Empty;
        public string getMapData { get; set; } = String.Empty;
        public List<string> type { get; set; }
    }
}
