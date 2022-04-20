namespace iGeoComAPI.Models
{
    public class CircleKModel
    {
        public int store_no { get; set; }
        public string address { get; set;} = String.Empty;
        public string  zone { get; set;} = String.Empty;
        public string location { get; set;} = String.Empty;
        public string latitude { get; set; } = String.Empty;
        public string longitude { get; set; } = String.Empty;
        public string operation_day { get; set; } = String.Empty;
        public string operation_hour { get; set; } = String.Empty;
        public static string regionListRegex
        {
            get { return @"\s* region_list = jQuery\.parseJSON\('(.*)'\)"; }
        }
        public static string storeListRegex
        {
            get { return @"\s* store_list = jQuery\.parseJSON\('(.*)'\);"; }
        }
    }
}
