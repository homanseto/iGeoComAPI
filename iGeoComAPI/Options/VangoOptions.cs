namespace iGeoComAPI.Options
{
    public class VangoOptions
    {
        public const string SectionName = "Vango";

        public string BaseUrl { get; set; } = String.Empty;
        public string Url { get; set; } = String.Empty;
        public string KLNRegionID { get; set; } = String.Empty;
        public string NTRegionID { get; set; } = String.Empty;
        public string HKRegionID { get; set; } = String.Empty;
        public string ContentType { get; set; } = String.Empty;
    }
}
