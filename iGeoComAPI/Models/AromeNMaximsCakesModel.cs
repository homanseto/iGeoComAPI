namespace iGeoComAPI.Models
{
    public class AromeNMaximsCakesModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Fax { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public static string NumberofShopRegex
        {
            get { return "^.*Total (.*) results"; }
        } 
        public static string RestaurantPathRegex
        {
            get { return "^.*href='(.*)';"; }
        }
        public static string IdRegex
        {
            get { return "^.*id=(.*)&m"; }
        } 
    }
}
