using iGeoComAPI.Models;
using iGeoComAPI.Repository;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CircleKController : ControllerBase
    {
        private string SelectCircleKFromDataBase = "SELECT * FROM iGeoCom_Dec2021 WHERE ENGLISHNAME like '%Caltex%';";
        private string SelectCaltex = "SELECT * FROM igeocomtable WHERE GRAB_ID LIKE '%caltex%'";
        private ILogger<CircleKController> _logger;
        private CircleKGrabber _circleKGrabber;
        private readonly IGeoComGrabRepository _iGeoComGrabRepository;


        public CircleKController(CircleKGrabber circleKGrabber, ILogger<CircleKController> logger, IGeoComGrabRepository iGeoComGrabRepository)
        {
            _circleKGrabber = circleKGrabber;
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

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var GrabbedResult = await _circleKGrabber.GetWebSiteItems();
            _iGeoComGrabRepository.CreateShops(GrabbedResult);
            return Ok(GrabbedResult);
        }
    }
}
