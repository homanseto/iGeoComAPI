namespace iGeoComAPI.Models
{
    public class IGeoComDeltaModel :IGeoComGrabModel
    {
        public string status { get; set; } = String.Empty;
        public enum Field
        {
            EnglishName,
            ChineseName,
            E_Address,
            C_Address,
            Tel_No,
            Fax_No,
            Easting,
            Northing
        }
    }
}
