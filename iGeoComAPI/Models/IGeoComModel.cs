namespace iGeoComAPI.Models
{
    public class IGeoComModel 
    {
        // public string? status { get; set; }
        public string? GeoNameId { get; set; }
        public string? EnglishName { get; set; }
        public string? ChineseName { get; set; }
        public string? Class { get; set; }
        public string? Type { get; set; }
        public string? Subcat { get; set; }
        public string? Easting { get; set; }
        public string? Northing { get; set; }
        public string? Source { get; set; }
        public string? E_floor { get; set; }
        public string? C_floor { get; set; }
        public string? E_sitename { get; set; }
        public string? C_sitename { get; set; }
        public string? E_area { get; set; }
        public string? C_area { get; set; }
        public string? E_District { get; set; }
        public string? C_District { get; set; }
        public string? E_Region { get; set; }
        public string? C_Region { get; set; }
        public string? E_Address { get; set; }
        public string? C_Address { get; set; }
        public string? Tel_No { get; set; }
        public string? Fax_No { get; set; }
        public string? Web_Site { get; set; }
        public string? Rev_Date { get; set; }
        public string InsertSql
        {
            get { 
                return "INSERT INTO igeocomtable VALUES (@GEONAMEID,@ENGLISHNAME,@CHINESENAME,@ClASS,@TYPE, @SUBCAT,@EASTING,@NORTHING,@SOURCE,@E_FLOOR,@C_FLOOR,@E_SITENAME,@C_SITENAME,@E_AREA,@C_AREA,@E_DISTRICT,@C_DISTRICT,@E_REGION,@C_REGION,@E_ADDRESS,@C_ADDRESS,@TEL_NO,@FAX_NO,@WEB_SITE,@REV_DATE,@GRAB_ID,@Latitude,@Longitude);";
            }
        }
        //  public string? Grab_ID { get; set; }
        // public string? Latitude { get; set; }
        // public string? Longitude { get; set; }
    }
}
