using iGeoComAPI.Models;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WmoovController : ControllerBase
    {
        private string InsertSql = "INSERT INTO igeocomtable VALUES (@GEONAMEID,@ENGLISHNAME,@CHINESENAME,@ClASS,@TYPE, @SUBCAT,@EASTING,@NORTHING,@SOURCE,@E_FLOOR,@C_FLOOR,@E_SITENAME,@C_SITENAME,@E_AREA,@C_AREA,@E_DISTRICT,@C_DISTRICT,@E_REGION,@C_REGION,@E_ADDRESS,@C_ADDRESS,@TEL_NO,@FAX_NO,@WEB_SITE,@REV_DATE,@GRAB_ID,@Latitude,@Longitude);";
        private string SelectCinemaFromDataBase = "select * from iGeoCom_Dec2021 where  TYPE = 'TNC';";
        private string SelectCinema = "select * from igeocomtable where  TYPE = 'TNC';";
        private readonly ILogger<WmoovController> _logger;
        private readonly WmoovGrabber _wmoovGrabber;
        private readonly DataAccess _dataAccess;
        public WmoovController(WmoovGrabber wmoovGrabber, ILogger<WmoovController> logger, DataAccess dataAccess)
        {
            _wmoovGrabber = wmoovGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<List<IGeoComGrabModel>> Get()
        {
            var result = await _dataAccess.LoadData<IGeoComGrabModel>(SelectCinema);
            return result;

        }

        [HttpGet("database")]
        public async Task<List<IGeoComGrabModel>?> GetTodoItem()
        {
            var previousResult = await _dataAccess.LoadData<IGeoComModel>(SelectCinemaFromDataBase);
            var newResult = await _dataAccess.LoadData<IGeoComGrabModel>(SelectCinema);
            var finalResult = _wmoovGrabber.FindAdded(newResult, previousResult);
            return finalResult;
        }

        [HttpPost]
        public async Task<List<IGeoComGrabModel?>> Create()
        {
            var GrabbedResult = await _wmoovGrabber.GetWebSiteItems();
            _dataAccess.SaveGrabbedData(InsertSql, GrabbedResult);
            return GrabbedResult;
        }
    }
}
