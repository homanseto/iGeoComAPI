using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class AromeNMaximsCakesController : ControllerBase
    {
        private ILogger<AromeNMaximsCakesController> _logger;
        private IGrabberAPI<AromeNMaximsCakesModel> _aromeNMaximsCakesGrabber;
        private DataAccess _dataAccess;

        //IGeoComModel igeoComModel = new IGeoComModel();
        AromeNMaximsCakesModel aromeNMaximsCakesModel = new AromeNMaximsCakesModel();


        public AromeNMaximsCakesController(IGrabberAPI<AromeNMaximsCakesModel> aromeNMaximsCakesGrabber, ILogger<AromeNMaximsCakesController> logger, DataAccess dataAccess)
        {
            _aromeNMaximsCakesGrabber = aromeNMaximsCakesGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<List<IGeoComGrabModel>?> Get()
        {
             var result = await _aromeNMaximsCakesGrabber.GetWebSiteItems();
            return result;
        }
    }
}
