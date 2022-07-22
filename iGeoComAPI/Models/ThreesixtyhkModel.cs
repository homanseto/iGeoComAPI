namespace iGeoComAPI.Models
{
    public class ThreesixtyhkModel
    {
        public string name { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string number { get; set; } = string.Empty;
        public string latlng { get; set; } = string.Empty;
        public static string extraLatLng
        {
            get { return "center=(?<latlng>.*)&zoom"; }
        }
        public static string splitLatLng
        {
            get { return "([^, ]*)"; }
        }
        public static string extraFax
        {
            get { return "Fax:(?<fax>.*)"; }
        }
        public static string extraTel
        {
            get { return "Tel:(?<tel>.*)Fax"; }
        }

        public static string extraZhFax
        {
            get { return " 傳真號碼：(?<fax>.*)"; }
        }
        public static string extraZhTel
        {
            get { return "號碼：(?<tel>.*)傳真"; }
        }
    }
}
