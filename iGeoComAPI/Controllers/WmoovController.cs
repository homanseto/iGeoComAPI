using iGeoComAPI.Models;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WmoovController : ControllerBase
    {
        private readonly ILogger<WmoovController> _logger;
        private readonly WmoovGrabber _wmoovGrabber;
        private readonly DataAccess _dataAccess;
        public WmoovController(WmoovGrabber wmoovGrabber, ILogger<WmoovController> logger, DataAccess dataAccess)
        {
            _wmoovGrabber = wmoovGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        WmoovModel wmoovModel = new WmoovModel();
        IGeoComGrabModel igeoComGrabModel = new IGeoComGrabModel();

        /*
        [HttpGet]
        public async Task<List<IGeoComGrabModel>> Get()
        {
            //var GrabbedResult = await _wmoovGrabber.GetWebSiteItems();
            //_dataAccess.SaveGrabbedData(InsertSql, GrabbedResult);
            var result = await _dataAccess.LoadData<IGeoComGrabModel>(wmoovModel.SelectCinema);
            CsvFile.DownloadCsv(result, "Wmoov_grab_result");
            return result;

        }

        [HttpGet("database")]
        public async Task<List<IGeoComGrabModel>?> GetTodoItem()
        {
            var previousResult = await _dataAccess.LoadData<IGeoComModel>(wmoovModel.SelectCinemaFromDataBase);
            var newResult = await _dataAccess.LoadData<IGeoComGrabModel>(wmoovModel.SelectCinema);
            var finalResult = _wmoovGrabber.FindAdded(newResult, previousResult);
            return finalResult;
        }

        [HttpPost]
        public async Task<List<IGeoComGrabModel?>> Create()
        {
            var GrabbedResult = await _wmoovGrabber.GetWebSiteItems();
            _dataAccess.SaveGrabbedData(igeoComGrabModel.InsertSql, GrabbedResult);
            return GrabbedResult;
        }
        */
    }
}
