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
    public class Store759Controller : ControllerBase
    {
        private ILogger<Store759Controller> _logger;
        private Store759Grabber _Store759Grabber;
        private IGeoComGrabRepository _iGeoComGrabRepository;
        private readonly IGeoComRepository _iGeoComRepository;


        public Store759Controller(Store759Grabber Store759Grabber, ILogger<Store759Controller> logger, IGeoComGrabRepository iGeoComGrabRepository, IGeoComRepository iGeoComRepository)
        {
            _Store759Grabber = Store759Grabber;
            _logger = logger;
            _iGeoComGrabRepository = iGeoComGrabRepository;
            _iGeoComRepository = iGeoComRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var GrabbedResult = await _Store759Grabber.GetWebSiteItems();
            //_iGeoComGrabRepository.CreateShops(GrabbedResult);
            return Ok(GrabbedResult);
        }
    }
}