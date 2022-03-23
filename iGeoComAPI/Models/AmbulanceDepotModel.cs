namespace iGeoComAPI.Models
{
    public class AmbulanceDepotModel
    {
        public string? Name { get; set; }
        public string? Id   { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? Region { get; set; }
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
