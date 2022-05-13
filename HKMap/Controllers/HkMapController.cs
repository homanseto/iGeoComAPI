using Dapper;
using HKMap.Entities;
using HKMap.Helper;


namespace HKMap.Controllers
{
    public class MapRepository
    {
        private readonly DapperContext _context;
        public MapRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<HKRegion> GetRegion(double longitude, double latitude)
        {
            var query = $"SP_MapFunction '{longitude}', '{latitude}'"; //$"DECLARE @point geometry SET @point = geometry::STGeomFromText('POINT({longitude} {latitude})',0); SELECT Region, District, Area2_Enam, Area2_Cnam FROM RegionDistrictAreaHk rdahk WHERE rdahk.Boundary.MakeValid().STIntersects(@point) = 1;";
            using (var connection = _context.CreateConnection())
            {
                var region = await connection.QuerySingleOrDefaultAsync<HKRegion>(query);
                return region;
            }


        }
    }
}