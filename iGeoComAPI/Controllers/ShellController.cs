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
    public class ShellController : ControllerBase
    {
        private ILogger<ShellController> _logger;
        private ShellGrabber _shellGrabber;
        private IGeoComGrabRepository _iGeoComGrabRepository;
        private readonly IGeoComRepository _iGeoComRepository;


        public ShellController(ShellGrabber shellGrabber, ILogger<ShellController> logger, IGeoComGrabRepository iGeoComGrabRepository, IGeoComRepository iGeoComRepository)
        {
            _shellGrabber = shellGrabber;
            _logger = logger;
            _iGeoComGrabRepository = iGeoComGrabRepository;
            _iGeoComRepository = iGeoComRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var GrabbedResult = await _shellGrabber.GetWebSiteItems();
            //_iGeoComGrabRepository.CreateShops(GrabbedResult);
            return Ok(GrabbedResult);
        }
    }
}
