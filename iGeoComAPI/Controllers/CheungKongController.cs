using iGeoComAPI.Models;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheungKongController : ControllerBase
    {
        private ILogger<CheungKongController> _logger;
        private CheungKongGrabber _cheungKongGrabber;
        private DataAccess _dataAccess;

        CheungKongModel cheungKongModel = new CheungKongModel();
        //IGeoComModel igeoComModel = new IGeoComModel();

        public CheungKongController(CheungKongGrabber cheungKongGrabber, ILogger<CheungKongController> logger, DataAccess dataAccess)
        {
            _cheungKongGrabber = cheungKongGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<List<CheungKongModel>?> Get()
        {
            var result = await _cheungKongGrabber.GetWebSiteItems();
            return result;
        }
    }
}
