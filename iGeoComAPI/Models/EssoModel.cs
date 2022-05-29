namespace iGeoComAPI.Models
{
    public class EssoLocations
    {
        public List<EssoModel>? locations { get; set; }
    }

    public class EssoModel
    {
        public int locationID { get; set; }
        public string brandName { get; set; } = string.Empty;
        public string locationName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }   
        public string AddressLine1 { get; set; } = string.Empty;
        public List<string>? Telephone { get; set; }
        public Boolean? Open24Hours { get; set; }
    }
}
