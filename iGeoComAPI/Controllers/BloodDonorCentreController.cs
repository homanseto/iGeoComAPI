using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodDonorCentreController : ControllerBase
    {
        private string InsertSql = "INSERT INTO igeocomtable VALUES (@GEONAMEID,@ENGLISHNAME,@CHINESENAME,@ClASS,@TYPE, @SUBCAT,@EASTING,@NORTHING,@SOURCE,@E_FLOOR,@C_FLOOR,@E_SITENAME,@C_SITENAME,@E_AREA,@C_AREA,@E_DISTRICT,@C_DISTRICT,@E_REGION,@C_REGION,@E_ADDRESS,@C_ADDRESS,@TEL_NO,@FAX_NO,@WEB_SITE,@REV_DATE,@GRAB_ID,@Latitude,@Longitude);";
        private string SelectCaltexFromDataBase = "SELECT * FROM iGeoCom_Dec2021 WHERE ENGLISHNAME like '%Caltex%';";
        private string SelectCaltex = "SELECT * FROM igeocomtable WHERE GRAB_ID LIKE '%caltex%'";
        private ILogger<BloodDonorCentreController> _logger;
        private BloodDonorCentreGrabber _bloodDonorCentreGrabber;
        private DataAccess _dataAccess;


        public BloodDonorCentreController(BloodDonorCentreGrabber bloodDonorCentreGrabber, ILogger<BloodDonorCentreController> logger, DataAccess dataAccess)
        {
            _bloodDonorCentreGrabber = bloodDonorCentreGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var result = await _bloodDonorCentreGrabber.GetWebSiteItems();
            return result;
        }
    }
}
