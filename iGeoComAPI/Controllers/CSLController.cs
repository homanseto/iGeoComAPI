using iGeoComAPI.Models;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CSLController : ControllerBase
    {
        private ILogger<CSLController> _logger;
        private CSLGrabber _cslGrabber;
        private DataAccess _dataAccess;

        CSLModel csl1010Model = new CSLModel();
       // IGeoComModel igeoComModel = new IGeoComModel();

        public CSLController(CSLGrabber cslGrabber, ILogger<CSLController> logger, DataAccess dataAccess)
        {
            _cslGrabber = cslGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task Get()
        {
            
            
            await _cslGrabber.GetWebSiteItems();
        }
    }
}
