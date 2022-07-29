namespace iGeoComAPI.Models
{
    public class CheungKongModel
    {
        public string href { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public string Id { get; set; } = String.Empty;
        public string Address { get; set; } = String.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }  
        public static string ExtractLatLng
        {
            get { return @"LatLng\((?<latlng>.*)\);var marker"; }
        }
        public static string RegLagLngRegex
        {
            get { return "([^, ]*)"; }
        }
        public static string ExtractName
        {
            get { return @"small;\"">(?<name>.*)<br \/>"; }
        }
        public static string ExtractAddress
        {
            get { return @"<\/span><\/strong>(?<address>.*)';var"; }
        }
        public static string ExtractId
        {
            get { return @"\?id=(?<id>.*)"; }
        }
    }
}
