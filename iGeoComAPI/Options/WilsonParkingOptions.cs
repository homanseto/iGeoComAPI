namespace iGeoComAPI.Options
{
    public class WilsonParkingOptions
    {
        public const string SectionName = "WilsonParking";
        public string BaseUrl { get; set; } = String.Empty;
        public string Url { get; set; } = String.Empty;
        public string XRequestedWith { get; set; } = String.Empty;
        public string HeaderKey { get; set; } = String.Empty;
    }
}
