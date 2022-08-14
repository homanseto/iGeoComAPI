using iGeoComAPI.Models;
using iGeoComAPI.Repository;
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
        /*
        private static Serilog.ILogger _log2 = Util.CreateLog("Aeon");
        _log2.Iner
        */
            //new Serilog.ILogger(typeof(AeonController));
        private ILogger<AeonController> _logger;
        private AeonGrabber _aeonGrabber;
        private readonly IGeoComRepository _iGeoComRepository;
        private readonly IGeoComGrabRepository _iGeoComGrabRepository;

        AeonModel aeonModel = new AeonModel();


        public AeonController(AeonGrabber aeonGrabber, ILogger<AeonController> logger, IGeoComRepository iGeoComRepository, IGeoComGrabRepository iGeoComGrabRepository)
        {
            _aeonGrabber = aeonGrabber;
            _logger = logger;
            _iGeoComRepository = iGeoComRepository;
            _iGeoComGrabRepository = iGeoComGrabRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                string name = this.GetType().Name.Replace("Controller","").ToLower();
                var result = await _iGeoComGrabRepository.GetShopsByName(name);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("download")]
        public async Task<IActionResult> GetDownload()
        {
            try
            {
                string name = this.GetType().Name.Replace("Controller", "").ToLower();

                var result = await _iGeoComGrabRepository.GetShopsByName(name);
                return Utilities.File.Download(result, name);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("delta/download")]
        public async Task<ActionResult> Testing()
        {
            try
            {
                var newResult = await _iGeoComGrabRepository.GetShopsByShopId("roi1");
                var oldResult = await _iGeoComRepository.GetShops("roi1");
                string[] ignoreList = new string[] { "GeoNameId", "EnglishName", "ChineseName", "Class", "Type", "Subcat", "Easting","Northing","Source",
                    "E_floor", "C_floor", "E_sitename","C_sitename","E_area","C_area","C_Region", "E_Region", "C_District","E_District", "Fax_No", "Tel_No","Web_Site",
                    "E_Address", "C_Address","Latitude", "Longitude","ShopId","Rev_Date","GrabId", "Compare_ChineseName", "Compare_EnglishName"};
                var resultList = Comparator2.GetComparedResult(newResult, oldResult, ignoreList);
                return Utilities.File.Download(resultList, $"aeon_smk_delta");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var GrabbedResult = await _aeonGrabber.GetWebSiteItems();
            _iGeoComGrabRepository.CreateShops(GrabbedResult);
            return Ok(GrabbedResult);
        }
    }
}
