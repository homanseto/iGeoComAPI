namespace iGeoComAPI.Models
{
    public class CheungKongModel
    {
        public string? Name { get; set; }
        public string? Id { get; set; }
        public string? Address { get; set; }
        public string? LatLng { get; set; }
        public string ExtractLatLng
        {
            get { return @"LatLng\((.*)\);var marker"; }
        }
        public string RegLagLngRegex
        {
            get { return "([^,]*)"; }
        }
        public string ExtractName
        {
            get { return @"small;\"">(.*)<br \/>"; }
        }
        public string ExtractAdrress
        {
            get { return @"<\/span><\/strong>(.*)';var"; }
        }
        public string ExtractId
        {
            get { return @"\?id=(.*)"; }
        }
    }
}
