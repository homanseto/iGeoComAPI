using iGeoComAPI.Models;
using iGeoComAPI.Services;
using Microsoft.AspNetCore.Mvc;
using iGeoComAPI.Options;
using Microsoft.Extensions.Options;
using iGeoComAPI.Utilities;
using iGeoComAPI.Repository;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaltexController : ControllerBase
    {
        private  ILogger<CaltexController> _logger;
        private CaltexGrabber _caltexGrabber;
        private IGeoComGrabRepository _iGeoComGrabRepository;

        IGeoComGrabModel igeoComGrabModel = new IGeoComGrabModel();
        CaltexModel caltexModel = new CaltexModel();

        public CaltexController(CaltexGrabber caltexGrabber, ILogger<CaltexController> logger, IGeoComGrabRepository iGeoComGrabRepository)
        {
            _caltexGrabber = caltexGrabber;
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

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var GrabbedResult = await _caltexGrabber.GetWebSiteItems();
            _iGeoComGrabRepository.CreateShops(GrabbedResult);
            return Ok(GrabbedResult);
        }
    }
}
