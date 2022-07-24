namespace iGeoComAPI.Models
{
    public class WilsonParkingModel
    {
        public CarPark? carPark { get; set; }

        public string[] ServiceTypeIds { get; set; } = new string[0];
        public List<Translation>? addressTranslation { get; set; }

        public List<Translation>? nameTranslation { get; set; }
    }

    public class CarPark
    {
        public string id { get; set; } = string.Empty;
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    public class Translation
    {
        public string content { get; set; } = string.Empty;
    }
}
