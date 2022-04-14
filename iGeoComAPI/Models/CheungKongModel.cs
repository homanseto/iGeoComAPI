namespace iGeoComAPI.Models
{
    public class CheungKongModel
    {
        public string Name { get; set; } = String.Empty;
        public string Id { get; set; } = String.Empty;
        public string Address { get; set; } = String.Empty;
        public string LatLng { get; set; } = String.Empty;
        public static string ExtractLatLng
        {
            get { return @"LatLng\((.*)\);var marker"; }
        }
        public static string RegLagLngRegex
        {
            get { return "([^,]*)"; }
        }
        public static string ExtractName
        {
            get { return @"small;\"">(.*)<br \/>"; }
        }
        public static string ExtractAdrress
        {
            get { return @"<\/span><\/strong>(.*)';var"; }
        }
        public static string ExtractId
        {
            get { return @"\?id=(.*)"; }
        }
    }
}
