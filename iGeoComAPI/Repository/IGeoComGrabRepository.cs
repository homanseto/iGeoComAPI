﻿using iGeoComAPI.Models;
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

        public async Task<List<IGeoComGrabModel>> GetShops()
        {

            string query = "SELECT * FROM igeocomTable";
            var result = await _dataAccess.LoadData<IGeoComGrabModel>(query, new {});
            return result;
        }

        public async Task<List<IGeoComGrabModel>> GetShopsByShopId(int id)
        {
            
            string query = "SELECT GEONAMEID,ENGLISHNAME,CHINESENAME,ClASS,TYPE,SUBCAT,EASTING,NORTHING,SOURCE,E_FLOOR,C_FLOOR,E_SITENAME,C_SITENAME,E_AREA,C_AREA,E_DISTRICT,C_DISTRICT,E_REGION,C_REGION,E_ADDRESS,C_ADDRESS,TEL_NO,FAX_NO,WEB_SITE,REV_DATE FROM igeocomTable WHERE SHOP = @id ";
            var result = await _dataAccess.LoadData<IGeoComGrabModel>(query, new { id });
            return result;
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
            string query = "SELECT GEONAMEID,ENGLISHNAME,CHINESENAME,ClASS,TYPE,SUBCAT,EASTING,NORTHING,SOURCE,E_FLOOR,C_FLOOR,E_SITENAME,C_SITENAME,E_AREA,C_AREA,E_DISTRICT,C_DISTRICT,E_REGION,C_REGION,E_ADDRESS,C_ADDRESS,TEL_NO,FAX_NO,WEB_SITE,REV_DATE FROM igeocomTable WHERE GrabId LIKE @name ";
            var result = await _dataAccess.LoadData<IGeoComGrabModel>(query, new {  name });
            return result;
        }
        public async Task<List<IGeoComModel>> GetShopsByType(string type)
        {

            string query = "SELECT GEONAMEID,ENGLISHNAME,CHINESENAME,ClASS,TYPE,SUBCAT,EASTING,NORTHING,SOURCE,E_FLOOR,C_FLOOR,E_SITENAME,C_SITENAME,E_AREA,C_AREA,E_DISTRICT,C_DISTRICT,E_REGION,C_REGION,E_ADDRESS,C_ADDRESS,TEL_NO,FAX_NO,WEB_SITE,REV_DATE FROM iGeoCom_Feb22 WHERE TYPE = @type ";
            var result = await _dataAccess.LoadData<IGeoComModel>(query, new { type });
            return result;
        }

        public void CreateShops(List<IGeoComGrabModel> result )
        {
              string query = "INSERT INTO igeocomTable" +
                "(GEONAMEID,ENGLISHNAME,CHINESENAME,ClASS,TYPE,SUBCAT,EASTING,NORTHING,SOURCE,E_FLOOR,C_FLOOR,E_SITENAME,C_SITENAME,E_AREA,C_AREA,E_DISTRICT,C_DISTRICT,E_REGION,C_REGION,E_ADDRESS,C_ADDRESS,TEL_NO,FAX_NO,WEB_SITE,REV_DATE,GrabId, Latitude, Longitude, Shop)" +
                " VALUES (@GEONAMEID,@ENGLISHNAME,@CHINESENAME,@ClASS,@TYPE, @SUBCAT,@EASTING,@NORTHING,@SOURCE,@E_FLOOR,@C_FLOOR,@E_SITENAME,@C_SITENAME,@E_AREA,@C_AREA,@E_DISTRICT,@C_DISTRICT,@E_REGION,@C_REGION,@E_ADDRESS,@C_ADDRESS,@TEL_NO,@FAX_NO,@WEB_SITE,@REV_DATE,@GrabId,@Latitude, @Longitude, @Shop);";
              _dataAccess.SaveGrabbedData<IGeoComGrabModel>(query, result);

        }
        public async Task<HKMapInfo> MapInfoFunction(double lng, double lat)

        {
            string query = $"MAP_FUNCTION {lng}, {lat}";
            var result = await _dataAccess.LoadSingleData<HKMapInfo>(query);
            return result;
        }
    }
}