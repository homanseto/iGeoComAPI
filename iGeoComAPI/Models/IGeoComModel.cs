using iGeoComAPI.Utilities;

namespace iGeoComAPI.Models
{
    public class IGeoComModel :DataAccess
    {
        // public string? status { get; set; }
        public string GeoNameId { get; set; } = String.Empty;
        public string EnglishName { get; set; } = String.Empty;
        public string ChineseName { get; set; } = String.Empty;
        public string Class { get; set; } = String.Empty;
        public string Type { get; set; } = String.Empty;
        public string Subcat { get; set; } = String.Empty;
        public double Easting { get; set; } 
        public double Northing { get; set; } 
        public string Source { get; set; } = String.Empty;
        public string E_floor { get; set; } = String.Empty;
        public string C_floor { get; set; } = String.Empty;
        public string E_sitename { get; set; } = String.Empty;
        public string C_sitename { get; set; } = String.Empty;
        public string E_area { get; set; } = String.Empty;
        public string C_area { get; set; } = String.Empty;
        public string E_District { get; set; } = String.Empty;
        public string C_District { get; set; } = String.Empty;
        public string E_Region { get; set; } = String.Empty;
        public string C_Region { get; set; } = String.Empty;
        public string E_Address { get; set; } = String.Empty;
        public string C_Address { get; set; } = String.Empty;
        public string Tel_No { get; set; } = String.Empty;
        public string Fax_No { get; set; } = String.Empty;
        public string Web_Site { get; set; } = String.Empty;
        public string Rev_Date { get; set; } = String.Empty;
        //  public string? Grab_ID { get; set; }
        // public string? Latitude { get; set; }
        // public string? Longitude { get; set; }

        public bool Insert()
        {
            return this.SaveGrabbedData<T>("INSERT INTO igeocomtable" +
                "(GEONAMEID,ENGLISHNAME,CHINESENAME,ClASS,TYPE,SUBCAT,EASTING,NORTHING,SOURCE,E_FLOOR,C_FLOOR,E_SITENAME,C_SITENAME,E_AREA,C_AREA,E_DISTRICT,C_DISTRICT,E_REGION,C_REGION,E_ADDRESS,C_ADDRESS,TEL_NO,FAX_NO,WEB_SITE,REV_DATE,GRAB_ID)" +
                " VALUES (@GEONAMEID,@ENGLISHNAME,@CHINESENAME,@ClASS,@TYPE, @SUBCAT,@EASTING,@NORTHING,@SOURCE,@E_FLOOR,@C_FLOOR,@E_SITENAME,@C_SITENAME,@E_AREA,@C_AREA,@E_DISTRICT,@C_DISTRICT,@E_REGION,@C_REGION,@E_ADDRESS,@C_ADDRESS,@TEL_NO,@FAX_NO,@WEB_SITE,@REV_DATE,@GRAB_ID);", this);
        }
        public static List<IGeoComModel>(string p1, string p2){
            return _dataAccess.Get("SELECT * FROM IGeoComModel WHERE p1 = @p1 AND p2 LIKE @p2", new { p1 = p1, p2 = p2 })?.ToList();
            }
    }
}
