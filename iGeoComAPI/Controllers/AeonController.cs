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
        private static Serilog.ILogger _log2 = Util.CreateLog("Aeon");
        _log2.Iner
            //new Serilog.ILogger(typeof(AeonController));
        private ILogger<AeonController> _logger;
        private IGrabberAPI<AeonModel> _aeonGrabber;
        private DataAccess _dataAccess;

        AeonModel aeonModel = new AeonModel();
        IGeoComModel iGeoComModel = new IGeoComModel();

        public AeonController(IGrabberAPI<AeonModel> aeonGrabber, ILogger<AeonController> logger, DataAccess dataAccess)
        {
            _aeonGrabber = aeonGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<List<IGeoComGrabModel>?> Get()
        {
            //var GrabbedResult = await _aeonGrabber.GetWebSiteItems();
            //_dataAccess.SaveGrabbedData(InsertSql, GrabbedResult);
            var result = await _dataAccess.LoadData<IGeoComGrabModel>(aeonModel.SelectAeon);
            CsvFile.DownloadCsv(result, "Aeon_grab_result");
            return result;
        }

        [HttpPost]
        public async Task<List<IGeoComGrabModel?>> Create()
        {
            var GrabbedResult = await _aeonGrabber.GetWebSiteItems();
            iGeoComModel.Insert(GrabbedResult);
            _dataAccess.SaveGrabbedData(iGeoComModel.InsertSql, GrabbedResult);
            return GrabbedResult;
        }
    }
}
