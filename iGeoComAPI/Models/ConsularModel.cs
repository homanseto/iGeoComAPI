namespace iGeoComAPI.Models
{
    public class ConsularModel
    {
        public int id { get; set; } 
        public string CountryName_tc { get; set; } = string.Empty;
        public string CountryName_en { get; set; } = string.Empty;
    }

    public class CountryInfo
    {
        public string Name_en { get; set; } = string.Empty;
        public string Name_tc { get; set; } = string.Empty;
        public List<DetailInfo>? Detail { get; set; }
    }
    public class DetailInfo
    {
       public int id { get; set; }
        public string address_en { get; set; } = string.Empty;
        public string address_tc { get; set; } = string.Empty;
        public string telephone { get; set; } = string.Empty;
        public string fax { get; set; } = string.Empty;
    }



}
