using iGeoComAPI.Models;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VangoController : ControllerBase
    {
        private readonly ILogger<VangoController> _logger;
        private readonly VangoGrabber _vangoGrabber;
        private readonly DataAccess _dataAccess;
        VangoModel vangoModel = new VangoModel();
        IGeoComModel igeoComModel = new IGeoComModel();

        public VangoController(VangoGrabber vangoGrabber, ILogger<VangoController> logger, DataAccess dataAccess)
        {
            _vangoGrabber = vangoGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<List<IGeoComGrabModel>?> Get()
        {
           // var GrabbedResult = await _vangoGrabber.GetWebSiteItems();
            //_dataAccess.SaveGrabbedData(InsertSql, GrabbedResult);
            var result = await _dataAccess.LoadData<IGeoComGrabModel>(vangoModel.SelectVango);
            CsvFile.DownloadCsv(result, "Vango_grab_result");
            return result;
        }

        [HttpPost]
        public async Task<List<IGeoComGrabModel?>> Create()
        {
            var GrabbedResult = await _vangoGrabber.GetWebSiteItems();
            _dataAccess.SaveGrabbedData(igeoComModel.InsertSql, GrabbedResult);
            return GrabbedResult;
        }
    }
}
