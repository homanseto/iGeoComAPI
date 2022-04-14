namespace iGeoComAPI.Options
{
    public class CaltexOptions
    {
        public const string SectionName = "Caltex";

        public string BaseUrl { get; set; } = String.Empty;
        public string Url { get; set; } = String.Empty;
        public string PagePathEn { get; set; } = String.Empty;
        public string PagePathZh { get; set; } = String.Empty;
        public string SiteType { get; set; } = String.Empty;
    }
}
