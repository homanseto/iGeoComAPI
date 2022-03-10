using iGeoComAPI.Models;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParknShopController : ControllerBase
    {
        private string InsertSql = "INSERT INTO igeocomtable VALUES (@GEONAMEID,@ENGLISHNAME,@CHINESENAME,@ClASS,@TYPE, @SUBCAT,@EASTING,@NORTHING,@SOURCE,@E_FLOOR,@C_FLOOR,@E_SITENAME,@C_SITENAME,@E_AREA,@C_AREA,@E_DISTRICT,@C_DISTRICT,@E_REGION,@C_REGION,@E_ADDRESS,@C_ADDRESS,@TEL_NO,@FAX_NO,@WEB_SITE,@REV_DATE,@GRAB_ID,@Latitude,@Longitude);";
        private string SelectParknShopFromDataBase = "SELECT * FROM iGeoCom_Dec2021 WHERE (ENGLISHNAME like '%park n shop%') or (ENGLISHNAME like '%Fusion-%') or (ENGLISHNAME like '%international-%') or (ENGLISHNAME like '%taste-%') or (ENGLISHNAME like '%FOODLEPARC%');";
        private string SelectParknShop = "SELECT * FROM igeocomtable WHERE GRAB_ID LIKE '%parknshop_%'";
        private ILogger<ParknShopController> _logger;
        private IGrabberAPI<ParknShopModel> _parknShopGrabber;
        private DataAccess _dataAccess;

        public ParknShopController(IGrabberAPI<ParknShopModel> parknShopGrabber, ILogger<ParknShopController> logger, DataAccess dataAccess)
        {
            _parknShopGrabber = parknShopGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<List<IGeoComGrabModel>?> Get()
        {
            var result = await _dataAccess.LoadData<IGeoComGrabModel>(SelectParknShop);
            return result;
        }

        [HttpPost]
        public async Task<List<IGeoComGrabModel?>> Create()
        {
            var GrabbedResult = await _parknShopGrabber.GetWebSiteItems();
            _dataAccess.SaveGrabbedData(InsertSql, GrabbedResult);
            return GrabbedResult;
        }
    }
}
