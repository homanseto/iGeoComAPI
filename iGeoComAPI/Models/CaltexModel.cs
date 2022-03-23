namespace iGeoComAPI.Models
{
    public class CaltexModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Street { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string SelectCaltexFromDataBase
        {
            get { return "SELECT * FROM iGeoCom_Dec2021 WHERE ENGLISHNAME like '%Caltex%';"; }
        } 
        public string SelectCaltex
        {
            get { return "SELECT * FROM igeocomtable WHERE GRAB_ID LIKE '%caltex%'"; }
        } 


    }
}
