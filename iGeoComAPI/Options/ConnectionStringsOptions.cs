namespace iGeoComAPI.Options
{
    public class ConnectionStringsOptions
    {
        public const string SectionName = "ConnectionStrings";

        public string Default { get; set; } = String.Empty;

        public string DefaultConnection { get; set; } = String.Empty;
        public string Default_3DM { get; set; } = String.Empty;
    }
}
