using iGeoComAPI.Models;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatholicOrgController : ControllerBase
    {
        private ILogger<CatholicOrgController> _logger;
        private CatholicOrgGrabber _catholicOrgGrabber;
        private DataAccess _dataAccess;

        AmbulanceDepotModel ambulanceDepotModel = new AmbulanceDepotModel();
        //IGeoComModel igeoComModel = new IGeoComModel();

        public CatholicOrgController(CatholicOrgGrabber catholicOrgGrabber, ILogger<CatholicOrgController> logger, DataAccess dataAccess)
        {
            _catholicOrgGrabber = catholicOrgGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task Get()
        {
            await _catholicOrgGrabber.GetWebSiteItems();
        }
    }
}
