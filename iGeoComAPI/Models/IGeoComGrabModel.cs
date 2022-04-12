using iGeoComAPI.Utilities;

namespace iGeoComAPI.Models
{
    public class IGeoComGrabModel 
    {
        private readonly DataAccess _dataAccess;
        private IGeoComGrabModel() { }
        public IGeoComGrabModel(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public async Task<List<IGeoComGrabModel>> GetGrabShops(string type)
        {
            /*
            string query = $"SELECT * FROM iGeoCom_Dec2021 WHERE '' = '' ";
            if (!string.IsNullOrEmpty(keyword))
                query += "AND ENGLISHNAME like @keyword ";
            if (!string.IsNullOrEmpty(type))
                query += "AND TYPE like @type ";
            */
            string query = "SELECT * FROM igeocomtable WHERE TYPE = @TYPE ";
            var result = await _dataAccess.LoadData<IGeoComModel>(query, new { TYPE = type });
            return result;
        }


        public async Task<List<IGeoComModel>> InsertShops(string type)
        {
            /*
            string query = $"SELECT * FROM iGeoCom_Dec2021 WHERE '' = '' ";
            if (!string.IsNullOrEmpty(keyword))
                query += "AND ENGLISHNAME like @keyword ";
            if (!string.IsNullOrEmpty(type))
                query += "AND TYPE like @type ";
            */
            string query = "SELECT * FROM iGeoCom_Dec2021 WHERE TYPE = @TYPE ";
            var result = await _dataAccess.LoadData<IGeoComModel>(query, new { TYPE = type });
            return result;
        }
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
        public DateTime Rev_Date { get; set; } = DateTime.Now;
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
