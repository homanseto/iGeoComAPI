namespace iGeoComAPI.Models
{
    public class WmoovModel
    {
        public string? Name { get; set; }
        public string? Region { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        private string SelectCinemaFromDataBase
        {
            get { return "select * from iGeoCom_Dec2021 where  TYPE = 'TNC';";}
        } 
        public string SelectCinema
        {
            get { return "select * from igeocomtable where  TYPE = 'TNC';"; }
        }
        public string WmoovIdRegex
        {
            get { return @"(?<=details\/)(.*)(?=\?)"; }
        } 
    }
}
