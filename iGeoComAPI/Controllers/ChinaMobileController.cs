using iGeoComAPI.Models;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChinaMobileController : ControllerBase
    {
        private ILogger<ChinaMobileController> _logger;
        private ChinaMobileGrabber _chinaMobileGrabber;
        private DataAccess _dataAccess;

        ChinaMobileModel ChinaMobileModel = new ChinaMobileModel();
        //IGeoComModel igeoComModel = new IGeoComModel();

        public ChinaMobileController(ChinaMobileGrabber chinaMobileGrabber, ILogger<ChinaMobileController> logger, DataAccess dataAccess)
        {
            _chinaMobileGrabber = chinaMobileGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<List<IGeoComGrabModel>?> Get()
        {
             var result = await _chinaMobileGrabber.GetWebSiteItems();
            return result;
        }
    }
}
