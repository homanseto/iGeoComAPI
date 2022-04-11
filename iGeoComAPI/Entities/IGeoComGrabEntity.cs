namespace iGeoComAPI.Entities
{
    public class IGeoComGrabEntity :IGeoComEntity
    {
        public string Grab_ID { get; set; } = String.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
