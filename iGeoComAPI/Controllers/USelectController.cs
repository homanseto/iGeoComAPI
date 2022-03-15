using iGeoComAPI.Models;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class USelectController : ControllerBase
    {
        private string InsertSql = "INSERT INTO igeocomtable VALUES (@GEONAMEID,@ENGLISHNAME,@CHINESENAME,@ClASS,@TYPE, @SUBCAT,@EASTING,@NORTHING,@SOURCE,@E_FLOOR,@C_FLOOR,@E_SITENAME,@C_SITENAME,@E_AREA,@C_AREA,@E_DISTRICT,@C_DISTRICT,@E_REGION,@C_REGION,@E_ADDRESS,@C_ADDRESS,@TEL_NO,@FAX_NO,@WEB_SITE,@REV_DATE,@GRAB_ID,@Latitude,@Longitude);";
        private string SelectUSelectFromDataBase = "SELECT * FROM iGeoCom_Dec2021 WHERE ENGLISHNAME like '%u slect%'";
        private string SelectUSelect = "SELECT * FROM igeocomtable WHERE GRAB_ID LIKE '%u select%'";
        private readonly ILogger<USelectController> _logger;
        private readonly USelectGrabber _uSelectGrabber;
        private readonly DataAccess _dataAccess;
        public USelectController(USelectGrabber uSelectGrabber, ILogger<USelectController> logger, DataAccess dataAccess)
        {
            _uSelectGrabber = uSelectGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<List<IGeoComGrabModel>?> Get()
        {
            var result = await _dataAccess.LoadData<IGeoComGrabModel>(SelectUSelect);
            CsvFile.DownloadCsv(result, "USelect_grab_result");
            return result;
        }

        [HttpPost]
        public async Task<List<IGeoComGrabModel?>> Create()
        {
            var GrabbedResult = await _uSelectGrabber.GetWebSiteItems();
            _dataAccess.SaveGrabbedData(InsertSql, GrabbedResult);
            return GrabbedResult;
        }
    }
}
