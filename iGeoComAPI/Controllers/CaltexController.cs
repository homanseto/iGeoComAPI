using iGeoComAPI.Models;
using iGeoComAPI.Services;
using Microsoft.AspNetCore.Mvc;
using iGeoComAPI.Options;
using Microsoft.Extensions.Options;
using iGeoComAPI.Utilities;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaltexController : ControllerBase
    {
        private  ILogger<CaltexController> _logger;
        private IGrabberAPI<CaltexModel> _caltexGrabber;
        private  DataAccess _dataAccess;

        IGeoComModel igeoComModel = new IGeoComModel();
        CaltexModel caltexModel = new CaltexModel();

        public CaltexController(IGrabberAPI<CaltexModel> caltexGrabber, ILogger<CaltexController> logger, DataAccess dataAccess)
        {
            _caltexGrabber = caltexGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }
        [HttpGet]
        public async Task<List<IGeoComGrabModel>?> Get()
        {
            //var GrabbedResult = await _caltexGrabber.GetWebSiteItems();
            //_dataAccess.SaveGrabbedData(InsertSql, GrabbedResult);
            var result = await _dataAccess.LoadData<IGeoComGrabModel>(caltexModel.SelectCaltex);
            CsvFile.DownloadCsv(result, "Caltex_grab_result");
            return result;
        }

        [HttpPost]
        public async Task<List<IGeoComGrabModel?>> Create()
        {
            var GrabbedResult = await _caltexGrabber.GetWebSiteItems();
            _dataAccess.SaveGrabbedData(igeoComModel.InsertSql, GrabbedResult);
            return GrabbedResult;
        }
    }
}
