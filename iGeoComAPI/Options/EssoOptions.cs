namespace iGeoComAPI.Options
{
    public class EssoOptions
    {
        public const string SectionName = "Esso";

        public string BaseUrl { get; set; } = String.Empty;
        public string EnUrl { get; set; } = String.Empty;
        public string ZhUrl { get; set; } = String.Empty;
        public string Latitude1 { get; set; } = String.Empty;
        public string Latitude2 { get; set; } = String.Empty;
        public string Longitude1 { get; set; } = String.Empty;
        public string Longitude2 { get; set; } = String.Empty;
        public string DataSource { get; set; } = String.Empty;
        public string Country { get; set; } = String.Empty;
        public string ResultLimit { get; set; } = String.Empty;

    }
}
