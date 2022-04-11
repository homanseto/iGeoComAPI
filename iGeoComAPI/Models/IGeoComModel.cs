using iGeoComAPI.Entities;
using iGeoComAPI.Utilities;

namespace iGeoComAPI.Models
{
    public class IGeoComModel : IIGeoComModel
    {
        private readonly DataAccess _dataAccess;
        public IGeoComModel(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public async Task<List<IGeoComEntity>> GetShops( string keyword)
        {
            string query = $"SELECT * FROM iGeoCom_Dec2021 WHERE ENGLISHNAME like {keyword}";
            var result = await _dataAccess.LoadData<IGeoComEntity>(query);
            return result;
        }

        public async Task<List<IGeoComEntity>> GetShopsByType(string type)
        {
            string query = $"SELECT * FROM iGeoCom_Dec2021 WHERE TYPE = '{type}'";
            var result = await _dataAccess.LoadData<IGeoComEntity>(query);
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


        /*
        public void Insert(List<IGeoComModel> parameters)
        {
           this.SaveGrabbedData<IGeoComModel>("INSERT INTO igeocomtable" +
                "(GEONAMEID,ENGLISHNAME,CHINESENAME,ClASS,TYPE,SUBCAT,EASTING,NORTHING,SOURCE,E_FLOOR,C_FLOOR,E_SITENAME,C_SITENAME,E_AREA,C_AREA,E_DISTRICT,C_DISTRICT,E_REGION,C_REGION,E_ADDRESS,C_ADDRESS,TEL_NO,FAX_NO,WEB_SITE,REV_DATE,GRAB_ID)" +
                " VALUES (@GEONAMEID,@ENGLISHNAME,@CHINESENAME,@ClASS,@TYPE, @SUBCAT,@EASTING,@NORTHING,@SOURCE,@E_FLOOR,@C_FLOOR,@E_SITENAME,@C_SITENAME,@E_AREA,@C_AREA,@E_DISTRICT,@C_DISTRICT,@E_REGION,@C_REGION,@E_ADDRESS,@C_ADDRESS,@TEL_NO,@FAX_NO,@WEB_SITE,@REV_DATE,@GRAB_ID);", parameters);
=======
        public string Rev_Date { get; set; } = String.Empty;
        //  public string? Grab_ID { get; set; }
        // public string? Latitude { get; set; }
        // public string? Longitude { get; set; }

        public bool Insert()
        {
            return this.SaveGrabbedData<T>("INSERT INTO igeocomtable" +
                "(GEONAMEID,ENGLISHNAME,CHINESENAME,ClASS,TYPE,SUBCAT,EASTING,NORTHING,SOURCE,E_FLOOR,C_FLOOR,E_SITENAME,C_SITENAME,E_AREA,C_AREA,E_DISTRICT,C_DISTRICT,E_REGION,C_REGION,E_ADDRESS,C_ADDRESS,TEL_NO,FAX_NO,WEB_SITE,REV_DATE,GRAB_ID)" +
                " VALUES (@GEONAMEID,@ENGLISHNAME,@CHINESENAME,@ClASS,@TYPE, @SUBCAT,@EASTING,@NORTHING,@SOURCE,@E_FLOOR,@C_FLOOR,@E_SITENAME,@C_SITENAME,@E_AREA,@C_AREA,@E_DISTRICT,@C_DISTRICT,@E_REGION,@C_REGION,@E_ADDRESS,@C_ADDRESS,@TEL_NO,@FAX_NO,@WEB_SITE,@REV_DATE,@GRAB_ID);", this);
>>>>>>> 829be149743e04d78eb323169864b1b01d6fdc99
        }
        public static List<IGeoComModel>(string p1, string p2)
            {
            return _dataAccess.Get("SELECT * FROM IGeoComModel WHERE p1 = @p1 AND p2 LIKE @p2", new { p1 = p1, p2 = p2 })?.ToList();
            }
        */
    }
    /*
    public static class IGeoComModelExtensions
    {
        public static IQueryable<IGeoComModel> ToModel(this IQueryable<IGeoComEntity> source)
        {
            return source.Select(entity => new IGeoComModel
            {
                GeoNameId = entity.GeoNameId,
                EnglishName = entity.EnglishName,
                ChineseName = entity.ChineseName,
                Class = entity.Class,
                Type = entity.Type,
                Subcat = entity.Subcat,
                Easting = entity.Easting,
                Northing = entity.Northing,
                Source = entity.Source,
                E_floor = entity.E_floor,
                C_floor = entity.C_floor,
                E_sitename = entity.E_sitename,
                C_sitename = entity.C_sitename,
                E_area = entity.E_area,
                C_area = entity.C_area,
                C_District = entity.C_District,
                E_District = entity.E_District,
                E_Region = entity.E_Region,
                C_Region = entity.C_Region,
                E_Address = entity.E_Address,
                C_Address = entity.C_Address,
                Tel_No = entity.Tel_No,
                Fax_No = entity.Fax_No,
                Web_Site = entity.Web_Site,
                Rev_Date = entity.Rev_Date
            });
        }

    }
    */
}
