using iGeoComAPI.Repository;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketPlaceController : ControllerBase
    {
        private ILogger<MarketPlaceController> _logger;
        private MarketPlaceGrabber _marketPlaceGrabber;
        private readonly IGeoComRepository _iGeoComRepository;
        private readonly IGeoComGrabRepository _iGeoComGrabRepository;



        public MarketPlaceController(MarketPlaceGrabber marketPlaceGrabber, ILogger<MarketPlaceController> logger, IGeoComRepository iGeoComRepository, IGeoComGrabRepository iGeoComGrabRepository)
        {
            _marketPlaceGrabber = marketPlaceGrabber;
            _logger = logger;
            _iGeoComRepository = iGeoComRepository;
            _iGeoComGrabRepository = iGeoComGrabRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                string name = this.GetType().Name.Replace("Controller", "").ToLower();
                var result = await _marketPlaceGrabber.GetWebSiteItems();
                return Ok(result);
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
                var newResult = await _iGeoComGrabRepository.GetShopsByShopId("smk4");
                var oldResult = await _iGeoComRepository.GetShops("smk4");
                string[] ignoreList = new string[] { "GeoNameId", "EnglishName", "ChineseName", "Class", "Type", "Subcat", "Easting","Northing","Source",
                    "E_floor", "C_floor", "E_sitename","C_sitename","E_area","C_area","C_Region", "E_Region", "C_District","E_District", "Fax_No", "Tel_No","Web_Site",
                    "E_Address", "C_Address","Latitude", "Longitude","ShopId","Rev_Date","GrabId", "Compare_ChineseName", "Compare_EnglishName"};
                var resultList = Comparator2.GetComparedResult(newResult, oldResult, ignoreList);
                return Utilities.File.Download(resultList, $"marketPlace_delta");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var GrabbedResult = await _marketPlaceGrabber.GetWebSiteItems();
            _iGeoComGrabRepository.CreateShops(GrabbedResult);
            return Ok(GrabbedResult);
        }
    }
}
