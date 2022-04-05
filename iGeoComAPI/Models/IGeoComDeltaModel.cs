namespace iGeoComAPI.Models
{
    public class IGeoComDeltaModel :IGeoComGrabModel
    {
        public string status { get; set; } = String.Empty;
        public enum Field
        {
            EnglishName,
            ChineseName,
            E_area,
            C_area,
            E_district,
            C_district,
            E_Region,
            C_Region,
            E_Address,
            C_Address,
            Tel_No,
            Fax_No,
            Latitude,
            Longtitude
        }
    }
}
