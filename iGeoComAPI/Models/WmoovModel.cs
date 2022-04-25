namespace iGeoComAPI.Models
{
    public class WmoovModel
    {
        public string Name { get; set; } = String.Empty;
        public string Region { get; set; } = String.Empty;
        public string Address { get; set; } = String.Empty;
        public string Phone { get; set; } = String.Empty;
        public string Website { get; set; } = String.Empty;
        public string Latitude { get; set; } = String.Empty;
        public string Longitude { get; set; } = String.Empty;
        public static string WmoovIdRegex
        {
            get { return @"(?<=details\/)(.*)(?=\?)"; }
        } 
    }
}
