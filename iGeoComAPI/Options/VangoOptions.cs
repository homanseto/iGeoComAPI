namespace iGeoComAPI.Options
{
    public class VangoOptions
    {
        public const string SectionName = "Vango";

        public string? BaseUrl { get; set; }
        public string? Url { get; set; }
        public string? KLNRegionID { get; set; }
        public string? NTRegionID { get; set; }
        public string? HKRegionID { get; set; }
        public string? ContentType { get; set; }
    }
}
