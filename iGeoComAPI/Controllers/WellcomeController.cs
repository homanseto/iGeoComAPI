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
    public class WellcomeController : ControllerBase
    {
        private string InsertSql = "INSERT INTO igeocomtable VALUES (@GEONAMEID,@ENGLISHNAME,@CHINESENAME,@ClASS,@TYPE, @SUBCAT,@EASTING,@NORTHING,@SOURCE,@E_FLOOR,@C_FLOOR,@E_SITENAME,@C_SITENAME,@E_AREA,@C_AREA,@E_DISTRICT,@C_DISTRICT,@E_REGION,@C_REGION,@E_ADDRESS,@C_ADDRESS,@TEL_NO,@FAX_NO,@WEB_SITE,@REV_DATE,@GRAB_ID,@Latitude,@Longitude);";
        private string SelectWellcome = "SELECT * FROM igeocomtable WHERE GRAB_ID LIKE '%wellcome%'";

        private readonly ILogger<WellcomeController> _logger;
        private readonly WellcomeGrabber _wellcomeGrabber;
        private readonly DataAccess _dataAccess;
        public WellcomeController(WellcomeGrabber wellcomeGrabber, ILogger<WellcomeController> logger, DataAccess dataAccess)
        {
            _wellcomeGrabber = wellcomeGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<List<IGeoComModel>?> Get()
        {
            var result  = await _dataAccess.LoadData<IGeoComModel>(SelectWellcome);
            return result;
        }

        [HttpGet("cache")]
        public List<IGeoComModel>? GetTodoItem()
        {
            var result = _dataAccess.LoadDataCache<IGeoComModel>();
            return result.Where(r => r.Grab_ID.Contains("wellcome")).ToList();

        }

        [HttpPost]
        public async Task<List<IGeoComModel?>> Create()
        {
            var GrabbedResult =await _wellcomeGrabber.GetWebSiteItems();
            _dataAccess.SaveGrabbedData(InsertSql, GrabbedResult);
            return GrabbedResult;
        }
    }
}
