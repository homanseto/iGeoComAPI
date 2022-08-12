namespace iGeoComAPI.Options
{
    public class NorthEastOptions
    {
        public const string SectionName = "NorthEast";

        public string ConvertNE { get; set; } = String.Empty;
        public string InSys { get; set; } = String.Empty;
        public string IutSys {get; set; } = String.Empty;
        public string SmoConvertNE { get; set; } = String.Empty;    
        public string SmoConvertLatLng { get; set; } = String.Empty;
    }
}
