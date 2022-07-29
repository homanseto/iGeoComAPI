namespace iGeoComAPI.Options
{
    public class ChurchOptions
    {
        public const string SectionName = "Church";
        public string BaseUrl { get; set; } = String.Empty;
        public string ZhUrl { get; set; } = String.Empty;
        public int perPageNum { get; set; }
        public string hkCookie { get; set; } = String.Empty;
        public string cookieName { get; set; } = String.Empty;
    }
}
