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
        public async Task<IActionResult> GetDelta()
        {
            try
            {
                string name = this.GetType().Name.Replace("Controller", "").ToLower();

                var previousResult = await _iGeoComRepository.GetShops(8);
                //var newResult = await this.iGeoComGrabRepository.GetShopsByName(name);
                var newResult = await _iGeoComGrabRepository.GetShopsByShopId(8);
                var result = Comparator.GetComparedResult(newResult, previousResult);
                return CsvFile.Download(result, $"{name}_delta");
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
            if (GrabbedResult == null)
                return BadRequest("Cannot insert grabbed data");
            _iGeoComGrabRepository.CreateShops(GrabbedResult);
            return Ok(GrabbedResult);
        }
    }
}
