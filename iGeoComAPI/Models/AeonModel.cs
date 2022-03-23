namespace iGeoComAPI.Models
{
    public class AeonModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? LatLng { get; set; }
        public string? Phone { get; set; }
        public string SelectAeon
        {
            get { return "SELECT * FROM igeocomtable WHERE GRAB_ID LIKE '%aeon_%'"; }
        }
        public string SelectAeonFromDataBase
        {
            get { return "SELECT * FROM iGeoCom_Dec2021 WHERE ENGLISHNAME like '%aeon%'"; }
        }
        public string AeonLatRegex
        {
            get { return @"LatLng(.*),"; }
        }
        public string AeonLngRegex
        {
            get { return @", (.*)(?=\);m)"; }
        }
    }
}
