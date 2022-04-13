using iGeoComAPI.Models;
using iGeoComAPI.Utilities;

namespace iGeoComAPI.Repository
{
    public class IGeoComGrabRepository
    {
        private readonly IDataAccess _dataAccess;
        public IGeoComGrabRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<List<IGeoComGrabModel>> GetShopsByName(string name)
        {
            /*
            string query = $"SELECT * FROM iGeoCom_Dec2021 WHERE '' = '' ";
            if (!string.IsNullOrEmpty(keyword))
                query += "AND ENGLISHNAME like @keyword ";
            if (!string.IsNullOrEmpty(type))
                query += "AND TYPE like @type ";
            */
            name = $"%{name}_%";
            string query = "SELECT * FROM igeocomTable  WHERE GRAB_ID LIKE @name ";
            var result = await _dataAccess.LoadData<IGeoComGrabModel>(query, new {  name });
            return result;
        }

        public void CreateShops(List<IGeoComGrabModel> result )
        {
              string query = "INSERT INTO igeocomTable" +
                "(GEONAMEID,ENGLISHNAME,CHINESENAME,ClASS,TYPE,SUBCAT,EASTING,NORTHING,SOURCE,E_FLOOR,C_FLOOR,E_SITENAME,C_SITENAME,E_AREA,C_AREA,E_DISTRICT,C_DISTRICT,E_REGION,C_REGION,E_ADDRESS,C_ADDRESS,TEL_NO,FAX_NO,WEB_SITE,REV_DATE,GRAB_ID)" +
                " VALUES (@GEONAMEID,@ENGLISHNAME,@CHINESENAME,@ClASS,@TYPE, @SUBCAT,@EASTING,@NORTHING,@SOURCE,@E_FLOOR,@C_FLOOR,@E_SITENAME,@C_SITENAME,@E_AREA,@C_AREA,@E_DISTRICT,@C_DISTRICT,@E_REGION,@C_REGION,@E_ADDRESS,@C_ADDRESS,@TEL_NO,@FAX_NO,@WEB_SITE,@REV_DATE,@GRAB_ID);";
              _dataAccess.SaveGrabbedData<IGeoComGrabModel>(query, result);

        }
    }
}
