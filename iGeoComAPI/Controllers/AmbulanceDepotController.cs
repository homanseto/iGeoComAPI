using iGeoComAPI.Models;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmbulanceDepotController : ControllerBase
    {
        private ILogger<AmbulanceDepotController> _logger;
        private AmbulanceDepotGrabber _ambulanceDepotGrabber;
        private DataAccess _dataAccess;

        AmbulanceDepotModel ambulanceDepotModel = new AmbulanceDepotModel();
        IGeoComModel igeoComModel = new IGeoComModel();

        public AmbulanceDepotController(AmbulanceDepotGrabber aromeNMaximsCakesGrabber, ILogger<AmbulanceDepotController> logger, DataAccess dataAccess)
        {
            _ambulanceDepotGrabber = aromeNMaximsCakesGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<List<IGeoComGrabModel>?> Get()
        {
            var result = await _ambulanceDepotGrabber.GetWebSiteItems();
            return result;

        }
    }
}
