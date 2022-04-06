namespace iGeoComAPI.Models
{
    public class SevenElevenModel
    {
        public string? LatLng { get; set; } = null;
        public string? Region { get; set; } = null;
        public string? District_key { get; set; } = null;
        public string? District { get; set; } = null;
        public string? Address { get; set; } = null;
        public string? Opening_24 { get; set; } = null;
        public string SelectSevenElevenFromGrabbedCache 
        { 
            get { return "SELECT * FROM igeocomTable WHERE GRAB_ID LIKE '%seveneleven%'";}
        }
        public string DeleteSevenElevenFromGrabbedCache
        {
            get { return "DELETE FROM igeocomTable WHERE GRAB_ID LIKE '%seveneleven%';";  }
        }
        public string SelectSevenElevenFromDataBase
        {
            get { return "SELECT * FROM iGeoCom_Dec2021 WHERE ENGLISHNAME LIKE '%7-Eleven%' "; }
        }
        public string RegLatLngRegex
        {
            get { return "([^|]*)"; }
        } 
    }
}
