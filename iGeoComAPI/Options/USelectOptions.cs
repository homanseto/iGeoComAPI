namespace iGeoComAPI.Options
{
    public class USelectOptions
    {
        public const string SectionName = "USelect";

        public string? BaseUrl { get; set; }
        public string? ShopAPI { get; set; }
        public int? select { get; set; }
        public int? selectFood { get; set; }
        public int? selectMini { get; set; }
        public string? ContentType { get; set; }
    }
}
