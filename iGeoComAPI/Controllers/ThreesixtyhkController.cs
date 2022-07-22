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
    public class ThreesixtyhkController : ControllerBase
    {
        private ILogger<ThreesixtyhkController> _logger;
        private ThreesixtyhkGrabber _ThreesixtyhkGrabber;
        private IGeoComGrabRepository _iGeoComGrabRepository;
        private readonly IGeoComRepository _iGeoComRepository;


        public ThreesixtyhkController(ThreesixtyhkGrabber ThreesixtyhkGrabber, ILogger<ThreesixtyhkController> logger, IGeoComGrabRepository iGeoComGrabRepository, IGeoComRepository iGeoComRepository)
        {
            _ThreesixtyhkGrabber = ThreesixtyhkGrabber;
            _logger = logger;
            _iGeoComGrabRepository = iGeoComGrabRepository;
            _iGeoComRepository = iGeoComRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var GrabbedResult = await _ThreesixtyhkGrabber.GetWebSiteItems();
            //_iGeoComGrabRepository.CreateShops(GrabbedResult);
            return Ok(GrabbedResult);
        }
    }
}