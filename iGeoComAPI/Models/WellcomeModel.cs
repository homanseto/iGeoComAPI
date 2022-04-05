namespace iGeoComAPI.Models
{
    public class WellcomeModel
    {
        public string? Address { get; set; }
        public string? Name { get; set; }
        public string? LatLng { get; set; }
        public string? Phone { get; set; }

        public string SelectWellcome
        {
            get { return "SELECT * FROM igeocomTable WHERE GRAB_ID LIKE '%wellcome%'"; }
        }
        public string SelectWellcomeFromDataBase
        {
            get { return "SELECT * FROM iGeoCom_Dec2021 WHERE ENGLISHNAME LIKE '%wellcome super%' "; }
        } 
    }
}
