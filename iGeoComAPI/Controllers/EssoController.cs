using iGeoComAPI.Repository;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EssoController : ControllerBase
    {
        private ILogger<EssoController> _logger;
        private EssoGrabber _essoGrabber;
        private readonly IGeoComRepository _iGeoComRepository;
        private readonly IGeoComGrabRepository _iGeoComGrabRepository;



        public EssoController(EssoGrabber essoGrabber, ILogger<EssoController> logger, IGeoComRepository iGeoComRepository, IGeoComGrabRepository iGeoComGrabRepository)
        {
            _essoGrabber = essoGrabber;
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
                var result = await _essoGrabber.GetWebSiteItems();
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

                var previousResult = await _iGeoComRepository.GetShops(10);
                //var newResult = await _iGeoComGrabRepository.GetShopsByName(name);
                var newResult = await _iGeoComGrabRepository.GetShopsByShopId(10);
                var result = Comparator.GetComparedResult(newResult, previousResult, "tel");
                return Utilities.File.Download(result, $"{name}_delta");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var GrabbedResult = await _essoGrabber.GetWebSiteItems();
            _iGeoComGrabRepository.CreateShops(GrabbedResult);
            return Ok(GrabbedResult);
        }
    }
}
