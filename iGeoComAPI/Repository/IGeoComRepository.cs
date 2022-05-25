using iGeoComAPI.Models;
using iGeoComAPI.Utilities;

namespace iGeoComAPI.Repository
{
    public class IGeoComRepository
    {
        private readonly IDataAccess _dataAccess;
        public IGeoComRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public async Task<List<IGeoComGrabModel>> GetShops(int id)
        {
            var conditionList = new Dictionary<string, string>();
            conditionList.Add("Englishname", "%park n shop%");
            /*
            string query = $"SELECT * FROM iGeoCom_Dec2021 WHERE '' = '' ";
            if (!string.IsNullOrEmpty(keyword))
                query += "AND ENGLISHNAME like @keyword ";
            if (!string.IsNullOrEmpty(type))
                query += "AND TYPE like @type ";
            */
            //name = $"%{name}%";
            string query = "SELECT GEONAMEID,ENGLISHNAME,CHINESENAME,ClASS,TYPE,SUBCAT,EASTING,NORTHING,SOURCE,E_FLOOR,C_FLOOR,E_SITENAME,C_SITENAME,E_AREA,C_AREA,E_DISTRICT,C_DISTRICT,E_REGION,C_REGION,E_ADDRESS,C_ADDRESS,TEL_NO,FAX_NO,WEB_SITE,REV_DATE FROM iGeoCom_Feb22 WHERE Shop = @id ";
            var result = await _dataAccess.LoadData<IGeoComGrabModel>(query, new {  id });
            return result;
        }

        //public async Task<List<IGeoComGrabModel>> GetShopList(string keyword,string type)
        //{
        //    var conditionList = new Dictionary<string, string>();
        //    conditionList.Add("Englishname", "%park n shop%");
            
        //    string query = $"SELECT * FROM iGeoCom_Dec2021 WHERE '' = '' ";
        //    if (!string.IsNullOrEmpty(keyword))
        //        query += "AND ENGLISHNAME like @keyword ";
        //    if (!string.IsNullOrEmpty(type))
        //        query += "AND TYPE like @type ";

        //}

        /*
        public async Task<List<IGeoComModel>> GetShopsByType(string type)
        {
            string query = $"SELECT * FROM iGeoCom_Dec2021 WHERE TYPE = '{type}'";
            var result = await _dataAccess.LoadData<IGeoComModel>(query);
            return result;
        }
        */
    }
}
