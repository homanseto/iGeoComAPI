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
    public class AmbulanceDepotController : ControllerBase
    {
        private readonly ILogger<AmbulanceDepotController> _logger;
        private readonly AmbulanceDepotGrabber _ambulanceDepotGrabber;
        private readonly IGeoComGrabRepository _iGeoComGrabRepository;
        private readonly IGeoComRepository _iGeoComRepository;

        AmbulanceDepotModel ambulanceDepotModel = new AmbulanceDepotModel();

        public AmbulanceDepotController(AmbulanceDepotGrabber ambulanceDepotGrabber, ILogger<AmbulanceDepotController> logger, IGeoComGrabRepository iGeoComGrabRepository, IGeoComRepository iGeoComRepository)
        {
            _ambulanceDepotGrabber = ambulanceDepotGrabber;
            _logger = logger;
            _iGeoComGrabRepository = iGeoComGrabRepository;
            _iGeoComRepository = iGeoComRepository;

        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                //string name = this.GetType().Name.Replace("Controller", "").ToLower();
                //var result = await _iGeoComGrabRepository.GetShopsByName(name);
                //if (result == null)
                //    return NotFound();
                var result = await _ambulanceDepotGrabber.GetWebSiteItems();

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
        public async Task<IActionResult> GetDelta()
        {
            try
            {
                string name = this.GetType().Name.Replace("Controller", "").ToLower();

                var previousResult = await _iGeoComRepository.GetShops("");
                //var newResult = await _iGeoComGrabRepository.GetShopsByName(name);
                var newResult = await _iGeoComGrabRepository.GetShopsByShopId("");
                var result = Comparator.GetComparedResult(newResult, previousResult, "ambulance");
                return Utilities.File.DownloadDelta(result, $"{name}_delta");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var GrabbedResult = await _ambulanceDepotGrabber.GetWebSiteItems();
            _iGeoComGrabRepository.CreateShops(GrabbedResult);
            return Ok(GrabbedResult);
        }
    }
}
