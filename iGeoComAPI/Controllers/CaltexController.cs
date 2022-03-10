using iGeoComAPI.Models;
using iGeoComAPI.Services;
using Microsoft.AspNetCore.Mvc;
using iGeoComAPI.Options;
using Microsoft.Extensions.Options;
using iGeoComAPI.Utilities;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaltexController : ControllerBase
    {
        private string InsertSql = "INSERT INTO igeocomtable VALUES (@GEONAMEID,@ENGLISHNAME,@CHINESENAME,@ClASS,@TYPE, @SUBCAT,@EASTING,@NORTHING,@SOURCE,@E_FLOOR,@C_FLOOR,@E_SITENAME,@C_SITENAME,@E_AREA,@C_AREA,@E_DISTRICT,@C_DISTRICT,@E_REGION,@C_REGION,@E_ADDRESS,@C_ADDRESS,@TEL_NO,@FAX_NO,@WEB_SITE,@REV_DATE,@GRAB_ID,@Latitude,@Longitude);";
        private string SelectCaltexFromDataBase = "SELECT * FROM iGeoCom_Dec2021 WHERE ENGLISHNAME like '%Caltex%';";
        private string SelectCaltex = "SELECT * FROM igeocomtable WHERE GRAB_ID LIKE '%caltex%'";
        private  ILogger<CaltexController> _logger;
        private IGrabberAPI<CaltexModel> _caltexGrabber;
        private  DataAccess _dataAccess;


        public CaltexController(IGrabberAPI<CaltexModel> caltexGrabber, ILogger<CaltexController> logger, DataAccess dataAccess)
        {
            _caltexGrabber = caltexGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }
        [HttpGet]
        public async Task<List<IGeoComGrabModel>?> Get()
        {
            var result = await _dataAccess.LoadData<IGeoComGrabModel>(SelectCaltex);
            return result;
        }

        [HttpPost]
        public async Task<List<IGeoComGrabModel?>> Create()
        {
            var GrabbedResult = await _caltexGrabber.GetWebSiteItems();
            _dataAccess.SaveGrabbedData(InsertSql, GrabbedResult);
            return GrabbedResult;
        }
    }
}
