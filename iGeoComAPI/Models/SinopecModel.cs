namespace iGeoComAPI.Models
{
    public class SinopecModel
    {
        public string id { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public string Address { get; set; } = String.Empty;
        public string Phone { get; set; } = String.Empty;
        public float latitude { get; set; }
        public float longitude { get; set; }
        public static string ExtractUrl
        {
            get { return @"window.open\('\/(.*)'\)"; }
        }
        public static string ExtractLat
        {
            get { return @"var s = (.*);\/\/ 纬度";  }
        }
        public static string ExtractLng
        {
            get { return @"var w = (.*);\/\/ 标识"; }
        }
    }
}
