using iGeoComAPI.Utilities;

namespace iGeoComAPI.Models
{
    public class IGeoComGrabModel : IGeoComModel
    {
        public string Grab_ID { get; set; } = String.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string InsertSql
        {
            get
            {
                return "INSERT INTO igeocomTable" +
                "(GEONAMEID,ENGLISHNAME,CHINESENAME,ClASS,TYPE,SUBCAT,EASTING,NORTHING,SOURCE,E_FLOOR,C_FLOOR,E_SITENAME,C_SITENAME,E_AREA,C_AREA,E_DISTRICT,C_DISTRICT,E_REGION,C_REGION,E_ADDRESS,C_ADDRESS,TEL_NO,FAX_NO,WEB_SITE,REV_DATE,GRAB_ID)" +
                " VALUES (@GEONAMEID,@ENGLISHNAME,@CHINESENAME,@ClASS,@TYPE, @SUBCAT,@EASTING,@NORTHING,@SOURCE,@E_FLOOR,@C_FLOOR,@E_SITENAME,@C_SITENAME,@E_AREA,@C_AREA,@E_DISTRICT,@C_DISTRICT,@E_REGION,@C_REGION,@E_ADDRESS,@C_ADDRESS,@TEL_NO,@FAX_NO,@WEB_SITE,@REV_DATE,@GRAB_ID);";
            }
        }
    }
}
