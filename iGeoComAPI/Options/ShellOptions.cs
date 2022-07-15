namespace iGeoComAPI.Options
{
    public class ShellOptions
    {
        public const string SectionName = "Shell";
        public float lat { get; set; }
        public float lng { get; set; }
        public bool autoload { get; set; }
        public string travel_mode { get; set; } = string.Empty;
        public bool avoid_tolls { get; set; }
        public bool avoid_highways { get; set; }
        public bool avoid_ferries { get; set; }
        public int corridor_radius { get; set; }
        public bool driving_distance { get; set; }
        public string format { get; set; } = string.Empty ;
        public string Url { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
    }
}
