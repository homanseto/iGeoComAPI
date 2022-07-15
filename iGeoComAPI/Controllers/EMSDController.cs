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
    public class EMSDController : ControllerBase
    {
        private ILogger<EMSDController> _logger;
        private EMSDGrabber _EMSDGrabber;
        private IGeoComGrabRepository _iGeoComGrabRepository;
        private readonly IGeoComRepository _iGeoComRepository;


        public EMSDController(EMSDGrabber EMSDGrabber, ILogger<EMSDController> logger, IGeoComGrabRepository iGeoComGrabRepository, IGeoComRepository iGeoComRepository)
        {
            _EMSDGrabber = EMSDGrabber;
            _logger = logger;
            _iGeoComGrabRepository = iGeoComGrabRepository;
            _iGeoComRepository = iGeoComRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var GrabbedResult = await _EMSDGrabber.GetWebSiteItems();
            //_iGeoComGrabRepository.CreateShops(GrabbedResult);
            return Ok(GrabbedResult);
        }
    }
}
