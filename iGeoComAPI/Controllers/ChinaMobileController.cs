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
    public class ChinaMobileController : ControllerBase
    {
        private readonly ILogger<ChinaMobileController> _logger;
        private readonly ChinaMobileGrabber _chinaMobileGrabber;
        private readonly IGeoComGrabRepository _iGeoComGrabRepository;

        ChinaMobileModel ChinaMobileModel = new ChinaMobileModel();
        //IGeoComModel igeoComModel = new IGeoComModel();

        public ChinaMobileController(ChinaMobileGrabber chinaMobileGrabber, ILogger<ChinaMobileController> logger, IGeoComGrabRepository iGeoComGrabRepository)
        {
            _chinaMobileGrabber = chinaMobileGrabber;
            _logger = logger;
            _iGeoComGrabRepository = iGeoComGrabRepository;
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
                return Utilities.File.Download(result, name);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var GrabbedResult = await _chinaMobileGrabber.GetWebSiteItems();
            _iGeoComGrabRepository.CreateShops(GrabbedResult);
            return Ok(GrabbedResult);
        }
    }
}
