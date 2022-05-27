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
    public class LinkHkController : ControllerBase
    {
        private ILogger<LinkHkController> _logger;
        private LinkHkGrabber _linkHkGrabber;
        private readonly IGeoComRepository _iGeoComRepository;
        private readonly IGeoComGrabRepository _iGeoComGrabRepository;



        public LinkHkController(LinkHkGrabber linkHkGrabber, ILogger<LinkHkController> logger, IGeoComRepository iGeoComRepository, IGeoComGrabRepository iGeoComGrabRepository)
        {
            _linkHkGrabber = linkHkGrabber;
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
                var result = await _linkHkGrabber.GetWebSiteItems();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
