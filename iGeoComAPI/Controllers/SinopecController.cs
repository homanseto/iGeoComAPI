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
    public class SinopecController : ControllerBase
    {
        private ILogger<SinopecController> _logger;
        private SinopecGrabber _SinopecGrabber;
        private IGeoComGrabRepository _iGeoComGrabRepository;
        private readonly IGeoComRepository _iGeoComRepository;


        public SinopecController(SinopecGrabber SinopecGrabber, ILogger<SinopecController> logger, IGeoComGrabRepository iGeoComGrabRepository, IGeoComRepository iGeoComRepository)
        {
            _SinopecGrabber = SinopecGrabber;
            _logger = logger;
            _iGeoComGrabRepository = iGeoComGrabRepository;
            _iGeoComRepository = iGeoComRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var GrabbedResult = await _SinopecGrabber.GetWebSiteItems();
            //_iGeoComGrabRepository.CreateShops(GrabbedResult);
            return Ok(GrabbedResult);
        }
    }
}
