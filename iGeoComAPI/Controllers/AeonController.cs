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
    public class AeonController : ControllerBase
    {
        /*
        private static Serilog.ILogger _log2 = Util.CreateLog("Aeon");
        _log2.Iner
        */
            //new Serilog.ILogger(typeof(AeonController));
        private ILogger<AeonController> _logger;
        private IGrabberAPI<AeonModel> _aeonGrabber;
        private readonly IGeoComRepository _iGeoComRepository;
        private readonly IGeoComGrabRepository _iGeoComGrabRepository;

        AeonModel aeonModel = new AeonModel();


        public AeonController(IGrabberAPI<AeonModel> aeonGrabber, ILogger<AeonController> logger, IGeoComRepository iGeoComRepository, IGeoComGrabRepository iGeoComGrabRepository)
        {
            _aeonGrabber = aeonGrabber;
            _logger = logger;
            _iGeoComRepository = iGeoComRepository;
            _iGeoComGrabRepository = iGeoComGrabRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                string name = this.GetType().Name.Replace("Controller","").ToLower();
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
