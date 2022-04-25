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
        public async Task<List<IGeoComModel>> GetShops(string name)
        {
            /*
            string query = $"SELECT * FROM iGeoCom_Dec2021 WHERE '' = '' ";
            if (!string.IsNullOrEmpty(keyword))
                query += "AND ENGLISHNAME like @keyword ";
            if (!string.IsNullOrEmpty(type))
                query += "AND TYPE like @type ";
            */
            name = $"%{name}_%";
            string query = "SELECT * FROM iGeoCom_Dec2021 WHERE ENGLISHNAME LIKE @NAME ";
            var result = await _dataAccess.LoadData<IGeoComModel>(query, new {  name });
            return result;
        }

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
