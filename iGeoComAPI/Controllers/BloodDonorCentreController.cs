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
    public class BloodDonorCentreController : ControllerBase
    {
        private ILogger<BloodDonorCentreController> _logger;
        private BloodDonorCentreGrabber _bloodDonorCentreGrabber;
        private readonly IGeoComGrabRepository _iGeoComGrabRepository;

        //IGeoComModel igeoComModel = new IGeoComModel();

        public BloodDonorCentreController(BloodDonorCentreGrabber bloodDonorCentreGrabber, ILogger<BloodDonorCentreController> logger, IGeoComGrabRepository iGeoComGrabRepository)
        {
            _bloodDonorCentreGrabber = bloodDonorCentreGrabber;
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
                return CsvFile.Download(result, name);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        //[HttpPost]
        //public async Task<IActionResult> Post()
        //{
        //    var GrabbedResult = await _bloodDonorCentreGrabber.GetWebSiteItems();
        //    _iGeoComGrabRepository.CreateShops(GrabbedResult);
        //    return Ok(GrabbedResult);
        //}
    }
}
