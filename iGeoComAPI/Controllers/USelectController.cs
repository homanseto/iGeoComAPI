using iGeoComAPI.Models;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class USelectController : ControllerBase
    {
        private readonly ILogger<USelectController> _logger;
        private readonly USelectGrabber _uSelectGrabber;
        private readonly DataAccess _dataAccess;
        public USelectController(USelectGrabber uSelectGrabber, ILogger<USelectController> logger, DataAccess dataAccess)
        {
            _uSelectGrabber = uSelectGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        USelectModel uSelectModel = new USelectModel();
        IGeoComGrabModel igeoComGrabModel = new IGeoComGrabModel();
        /*
        [HttpGet]
        public async Task<List<IGeoComGrabModel>?> Get()
        {
           // var GrabbedResult = await _uSelectGrabber.GetWebSiteItems();
            //_dataAccess.SaveGrabbedData(InsertSql, GrabbedResult);
            var result = await _dataAccess.LoadData<IGeoComGrabModel>(uSelectModel.SelectUSelect);
            CsvFile.DownloadCsv(result, "USelect_grab_result");
            return result;
        }

        [HttpPost]
        public async Task<List<IGeoComGrabModel?>> Create()
        {
            var GrabbedResult = await _uSelectGrabber.GetWebSiteItems();
            _dataAccess.SaveGrabbedData(igeoComGrabModel.InsertSql, GrabbedResult);
            return GrabbedResult;
        }
        */
    }
}
