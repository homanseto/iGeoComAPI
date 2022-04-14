namespace iGeoComAPI.Models
{
    public class BmcpcModel
    {
        public string Name { get; set; } = string.Empty;
        public string Info { get; set; } = string.Empty;
        public static string AddressRegex{
            get { return @"^.*地址：(.*)（位置）.*$";  }
        }
        public static string PhoneRegex
        {
            get { return @"^.*查詢電話：(\d+ \d+).*$"; }
        }
    }
}
