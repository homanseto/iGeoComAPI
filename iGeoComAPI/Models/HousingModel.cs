// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using Newtonsoft.Json;
namespace iGeoComAPI.Models
{
    public class HousingModel
    {
        [JsonProperty("last-revision-date")]
        public string LastRevisionDate { get; set; } = String.Empty;
        public List<RegionArray>? regionArray { get; set; }
    }

    public class RegionArray
    {
        public Area ?area { get; set; }
        public List<double>? center { get; set; }
        public List<District>? district { get; set; }
        public int zoom { get; set; }
        public string id { get; set; } = String.Empty;
    }

    public class District
    {
        public List<Estate>? estates { get; set; }
        public Name? name { get; set; }
    }

    public class Estate
    {
        public List<double>? center { get; set; }
        public Name? name { get; set; }
        public int id { get; set; }
        public string aplySysId { get; set; } = String.Empty;
    }

    public class Name
    {
        [JsonProperty("zh-Hans")]
        public string ZhHans { get; set; } = String.Empty;
        [JsonProperty("zh-Hant")]
        public string ZhHant { get; set; } = String.Empty;
        public string en { get; set; } = String.Empty;
    }

    public class Area
    {
        [JsonProperty("zh-Hans")]
        public string ZhHans { get; set; } = String.Empty;
        [JsonProperty("zh-Hant")]
        public string ZhHant { get; set; } = String.Empty;
        public string en { get; set; } = String.Empty;
    }

}

