namespace iGeoComAPI.Models
{
    public class Store759Model
    {
        public string id { get; set; } = String.Empty;
        public string name { get; set; } = String.Empty;
        public string address { get; set; } = String.Empty;
        public string number { get; set; } = String.Empty;
        public static string extractId
        {
            get { return "id=(?<id>.*)&type"; }
        }
    }
}
