namespace iGeoComAPI.Models
{
    public class EMSDModel
    {
        public string id { get; set; } = string.Empty; 
        public string Address { get; set; } = string.Empty ;
        public string Brand { get; set; } = string.Empty ;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Number { get; set; } = string.Empty;
        public static string records
        {
            get { return @"var records = (.*);"; }
        }
    }
}
