using iGeoComAPI.Models;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AeonController : ControllerBase
    {
        private string InsertSql = "INSERT INTO igeocomtable VALUES (@GEONAMEID,@ENGLISHNAME,@CHINESENAME,@ClASS,@TYPE, @SUBCAT,@EASTING,@NORTHING,@SOURCE,@E_FLOOR,@C_FLOOR,@E_SITENAME,@C_SITENAME,@E_AREA,@C_AREA,@E_DISTRICT,@C_DISTRICT,@E_REGION,@C_REGION,@E_ADDRESS,@C_ADDRESS,@TEL_NO,@FAX_NO,@WEB_SITE,@REV_DATE,@GRAB_ID,@Latitude,@Longitude);";
        private ILogger<AeonController> _logger;
        private IGrabberAPI<AeonModel> _aeonGrabber;
        private DataAccess _dataAccess;

        public AeonController(IGrabberAPI<AeonModel> aeonGrabber, ILogger<AeonController> logger, DataAccess dataAccess)
        {
            _aeonGrabber = aeonGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }
        [HttpGet]
        public async Task<List<IGeoComGrabModel>?> Get()
        {
            //var result = await _dataAccess.LoadData<IGeoComGrabModel>(SelectCaltex);
            
            var result = await _aeonGrabber.GetWebSiteItems();
            _dataAccess.SaveGrabbedData(InsertSql, result);
            return result;
        }

        /*
        [HttpPost]
        public async Task<List<IGeoComGrabModel?>> Create()
        {
            var GrabbedResult = await _caltexGrabber.GetWebSiteItems();
            _dataAccess.SaveGrabbedData(InsertSql, GrabbedResult);
            return GrabbedResult;
        }
        */
    }
}
