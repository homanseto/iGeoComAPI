using iGeoComAPI.Models;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AeonController : ControllerBase
    {
        /*
        private static Serilog.ILogger _log2 = Util.CreateLog("Aeon");
        _log2.Iner
        */
            //new Serilog.ILogger(typeof(AeonController));
        private ILogger<AeonController> _logger;
        private IGrabberAPI<AeonModel> _aeonGrabber;
        private DataAccess _dataAccess;
        private readonly IGeoComModel _iGeoComModel;

        AeonModel aeonModel = new AeonModel();


        public AeonController(IGrabberAPI<AeonModel> aeonGrabber, ILogger<AeonController> logger, DataAccess dataAccess, IGeoComModel iGeoComModel)
        {
            _aeonGrabber = aeonGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
            _iGeoComModel = iGeoComModel;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string keyword)
        {
            try
            {
                var result = await _iGeoComModel.GetShops(keyword);
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
        public async Task<List<IGeoComGrabModel?>> Create()
        {
            var GrabbedResult = await _aeonGrabber.GetWebSiteItems();
            /*
            iGeoComModel.Insert(GrabbedResult);
            _dataAccess.SaveGrabbedData(iGeoComModel.InsertSql, GrabbedResult);
            */
            return GrabbedResult;
        }
    }
}
