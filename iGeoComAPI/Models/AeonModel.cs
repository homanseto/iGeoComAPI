namespace iGeoComAPI.Models
{
    public class AeonModel
    {
        public string Id { get; set; }= String.Empty;
        public string Name { get; set; } = String.Empty;
        public string Address { get; set; } = String.Empty;
        public string LatLng { get; set; } = String.Empty;
        public string Phone { get; set; } = String.Empty;
        public static string AeonLatRegex
        {
            get { return @"LatLng(.*),"; }
        }
        public static string AeonLngRegex
        {
            get { return @", (.*)(?=\);m)"; }
        }
    }
}
