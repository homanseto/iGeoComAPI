using iGeoComAPI.Models;
using iGeoComAPI.Repository;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CircleKController : ControllerBase
    {
        private ILogger<CircleKController> _logger;
        private CircleKGrabber _circleKGrabber;
        private readonly IGeoComGrabRepository _iGeoComGrabRepository;
        private readonly IGeoComRepository _iGeoComRepository;


        public CircleKController(CircleKGrabber circleKGrabber, ILogger<CircleKController> logger, IGeoComGrabRepository iGeoComGrabRepository, IGeoComRepository iGeoComRepository)
        {
            _circleKGrabber = circleKGrabber;
            _logger = logger;
            _iGeoComGrabRepository = iGeoComGrabRepository;
            _iGeoComRepository = iGeoComRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                string name = this.GetType().Name.Replace("Controller", "").ToLower();
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
                return CsvFile.Download(result, name);
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

                var previousResult = await _iGeoComRepository.GetShops(2);
                //var newResult = await this.iGeoComGrabRepository.GetShopsByName(name);
                var newResult = await _iGeoComGrabRepository.GetShopsByShopId(2);
                var result = Comparator.GetComparedResult(newResult, previousResult,"tel");
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
            var GrabbedResult = await _circleKGrabber.GetWebSiteItems();
            _iGeoComGrabRepository.CreateShops(GrabbedResult);
            return Ok(GrabbedResult);
        }
    }
}
