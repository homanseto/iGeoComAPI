namespace iGeoComAPI.Options
{
    public class AromeNMaximsCakesOptions
    {
        public const string SectionName = "AromeNMaximsCakes";

        public string BaseUrl { get; set; } = String.Empty;
        public string EnUrl { get; set; } = String.Empty;
        public string ZhUrl { get; set; } = String.Empty;
        public int EachPageNumber { get; set; } 
        public string EnSearchpath { get; set; } = String.Empty;
        public string ZhSearchpath { get; set; } = String.Empty;
    }
}
