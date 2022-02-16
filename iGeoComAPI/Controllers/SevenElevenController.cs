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
    public class SevenElevenController : ControllerBase

    {
        private string InsertSql = "INSERT INTO igeocomtable VALUES (@GEONAMEID,@ENGLISHNAME,@CHINESENAME,@ClASS,@TYPE, @SUBCAT,@EASTING,@NORTHING,@SOURCE,@E_FLOOR,@C_FLOOR,@E_SITENAME,@C_SITENAME,@E_AREA,@C_AREA,@E_DISTRICT,@C_DISTRICT,@E_REGION,@C_REGION,@E_ADDRESS,@C_ADDRESS,@TEL_NO,@FAX_NO,@WEB_SITE,@REV_DATE,@GRAB_ID,@Latitude,@Longitude);";
        private string SelectSevenEleven = "SELECT * FROM igeocomtable WHERE GRAB_ID LIKE '%seveneleven%'";
        private readonly ILogger<SevenElevenController> _logger;
        private readonly SevenElevenGrabber _sevenElevenGrabber;
        private readonly DataAccess _dataAccess;
        public SevenElevenController(SevenElevenGrabber sevenElevenGrabber, ILogger<SevenElevenController> logger, DataAccess dataAccess)
        {
            _sevenElevenGrabber = sevenElevenGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<List<IGeoComModel>?> Get()
        {
            var result = await _dataAccess.LoadData<IGeoComModel>(SelectSevenEleven);
            return result;
        }

        [HttpGet("cache")]
        public List<IGeoComModel>? GetTodoItem()
        {
            var result = _dataAccess.LoadDataCache<IGeoComModel>();
            return result.Where(r => r.Grab_ID.Contains("seveneleven")).ToList();
        }

        [HttpPost]
        public async Task<List<IGeoComModel?>> Create()
        {
            var GrabbedResult = await _sevenElevenGrabber.GetWebSiteItems();
            _dataAccess.SaveGrabbedData(InsertSql, GrabbedResult);
            return GrabbedResult;
        }


    }

}
