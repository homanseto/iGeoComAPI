using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class AromeNMaximsCakesController : ControllerBase
    {
        private string InsertSql = "INSERT INTO igeocomtable VALUES (@GEONAMEID,@ENGLISHNAME,@CHINESENAME,@ClASS,@TYPE, @SUBCAT,@EASTING,@NORTHING,@SOURCE,@E_FLOOR,@C_FLOOR,@E_SITENAME,@C_SITENAME,@E_AREA,@C_AREA,@E_DISTRICT,@C_DISTRICT,@E_REGION,@C_REGION,@E_ADDRESS,@C_ADDRESS,@TEL_NO,@FAX_NO,@WEB_SITE,@REV_DATE,@GRAB_ID,@Latitude,@Longitude);";
        private string SelectCaltexFromDataBase = "SELECT * FROM iGeoCom_Dec2021 WHERE ENGLISHNAME like '%Caltex%';";
        private string SelectCaltex = "SELECT * FROM igeocomtable WHERE GRAB_ID LIKE '%caltex%'";
        private ILogger<AromeNMaximsCakesController> _logger;
        private IGrabberAPI<AromeNMaximsCakesModel> _aromeNMaximsCakesGrabber;
        private DataAccess _dataAccess;


        public AromeNMaximsCakesController(IGrabberAPI<AromeNMaximsCakesModel> aromeNMaximsCakesGrabber, ILogger<AromeNMaximsCakesController> logger, DataAccess dataAccess)
        {
            _aromeNMaximsCakesGrabber = aromeNMaximsCakesGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<List<IGeoComGrabModel>?> Get()
        {
             var result = await _aromeNMaximsCakesGrabber.GetWebSiteItems();
            return result;
        }
    }
}
