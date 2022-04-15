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
    public class VangoController : ControllerBase
    {
        private readonly ILogger<VangoController> _logger;
        private readonly VangoGrabber _vangoGrabber;
        private IGeoComGrabRepository _iGeoComGrabRepository;

        public VangoController(VangoGrabber vangoGrabber, ILogger<VangoController> logger, IGeoComGrabRepository iGeoComGrabRepository)
        {
            _vangoGrabber = vangoGrabber;
            _logger = logger;
            _iGeoComGrabRepository = iGeoComGrabRepository;
        }

        [HttpPost]
        public async Task<List<IGeoComGrabModel>?> Post()
        {
            //_logger.LogControllerRequest(nameof(VangoController), nameof(Post));
            var GrabbedResult = await _vangoGrabber.GetWebSiteItems();
            _iGeoComGrabRepository.CreateShops(GrabbedResult);
            return GrabbedResult;
        }

        /*
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
            _dataAccess.SaveGrabbedData(igeoComGrabModel.InsertSql, GrabbedResult);
            return GrabbedResult;
        }
        */
    }
}
