namespace iGeoComAPI.Models
{
    public class LatLngModel
    {
        public double Latitude { get; set; }
        public double Longtitude { get; set; }
        public static string getLat
        {
            get { return "(?<=!3d)(.*)(?=!4d)"; }
        }
        public static string getLng
        {
            get { return "(?<=!4d)(.*)"; }
        }
        public static string getLngWithExClamation
        {
            get { return "(.*)(?=!)"; }
        }

        public static string containLetter
        {
            get { return "[a-zA-Z]"; }
        }
    }
}
