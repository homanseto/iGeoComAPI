using iGeoComAPI.Models;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BmcpcController : ControllerBase
    {
        private ILogger<BmcpcController> _logger;
        private BmcpcGrabber _bmcpcGrabber;
        private DataAccess _dataAccess;

        BmcpcModel bmcpcModel = new BmcpcModel();
        IGeoComModel igeoComModel = new IGeoComModel();

        public BmcpcController(BmcpcGrabber bmcpcGrabber, ILogger<BmcpcController> logger, DataAccess dataAccess)
        {
            _bmcpcGrabber = bmcpcGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<List<IGeoComGrabModel>?> Get()
        {
           var result = await _bmcpcGrabber.GetWebSiteItems();
           return result;
        }
    }
}
