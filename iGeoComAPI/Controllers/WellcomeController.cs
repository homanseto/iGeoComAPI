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
    public class WellcomeController : ControllerBase
    {
        private readonly ILogger<WellcomeController> _logger;
        private readonly WellcomeGrabber _wellcomeGrabber;
        private readonly DataAccess _dataAccess;

        WellcomeModel wellcomeModel = new WellcomeModel();
        IGeoComGrabModel igeoComGrabModel = new IGeoComGrabModel();

        public WellcomeController(WellcomeGrabber wellcomeGrabber, ILogger<WellcomeController> logger, DataAccess dataAccess)
        {
            _wellcomeGrabber = wellcomeGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }
        /*
        [HttpGet]
        public async Task<List<IGeoComGrabModel>?> Get()
        {
            //var GrabbedResult = await _wellcomeGrabber.GetWebSiteItems();
            //_dataAccess.SaveGrabbedData(InsertSql, GrabbedResult);
            var result  = await _dataAccess.LoadData<IGeoComGrabModel>(wellcomeModel.SelectWellcome);
            CsvFile.DownloadCsv(result, "Wellcome_grab_result");
            return result;
        }

        [HttpGet("download/grabresult")]
        public async Task<FileStreamResult> GetDownload()
        {
            var result = await _dataAccess.LoadData<IGeoComGrabModel>(wellcomeModel.SelectWellcome);
            return CsvFile.Download(result, "wellcome");
        }

        /*
        [HttpGet("cache")]
        public List<IGeoComGrabModel>? Get()
        {
            var result = _dataAccess.LoadDataCache<IGeoComGrabModel>();
            return result.Where(r => r.Grab_ID.Contains("wellcome")).ToList();
        }
        

        [HttpGet("added")]
        public async Task<List<IGeoComDeltaModel>?> GetAdded()
        {
            var previousResult = await _dataAccess.LoadData<IGeoComModel>(wellcomeModel.SelectWellcomeFromDataBase);
            var newResult = await _dataAccess.LoadData<IGeoComGrabModel>(wellcomeModel.SelectWellcome);
            var finalResult = _wellcomeGrabber.FindAdded(newResult,previousResult);
            return finalResult;
        }

        [HttpGet("removed")]
        public async Task<List<IGeoComDeltaModel>?> GetRemoved()
        {
            var previousResult = await _dataAccess.LoadData<IGeoComModel>(wellcomeModel.SelectWellcomeFromDataBase);
            var newResult = await _dataAccess.LoadData<IGeoComGrabModel>(wellcomeModel.SelectWellcome);
            var finalResult = _wellcomeGrabber.FindRemoved(previousResult, newResult);
            return finalResult;
        }

        [HttpGet("oldmodified")]
        public async Task<List<IGeoComDeltaModel>?> GetLeft()
        {
            var previousResult = await _dataAccess.LoadData<IGeoComModel>(wellcomeModel.SelectWellcomeFromDataBase);
            var newResult = await _dataAccess.LoadData<IGeoComGrabModel>(wellcomeModel.SelectWellcome);
            var removedResult = _wellcomeGrabber.FindRemoved(previousResult, newResult);
            var addedResult = _wellcomeGrabber.FindAdded(newResult, previousResult);
            var leftResult = _wellcomeGrabber.LeftIntersection(newResult, addedResult);
            var rightResult = _wellcomeGrabber.RightIntersection(previousResult, removedResult);
            var finalResult = _wellcomeGrabber.orgModified(rightResult, leftResult);
            return finalResult;
        }

        [HttpGet("download/deltaresult")]
        public async Task<FileStreamResult> GetRight()
        {
            var previousResult = await _dataAccess.LoadData<IGeoComModel>(wellcomeModel.SelectWellcomeFromDataBase);
            var newResult = await _dataAccess.LoadData<IGeoComGrabModel>(wellcomeModel.SelectWellcome);
            var removedResult = _wellcomeGrabber.FindRemoved(previousResult, newResult);
            var addedResult = _wellcomeGrabber.FindAdded(newResult, previousResult);
            var leftResult = _wellcomeGrabber.LeftIntersection(newResult, addedResult);
            var rightResult = _wellcomeGrabber.RightIntersection(previousResult, removedResult);
            var orgModified = _wellcomeGrabber.orgModified(rightResult, leftResult);
            var newModified = _wellcomeGrabber.newModified(leftResult, rightResult);
            var finalResult = _wellcomeGrabber.MergeResults(addedResult, removedResult, newModified, orgModified);
            return CsvFile.Download(finalResult, "wellcomeDelta");
        }

        [HttpPost]
        public async Task<List<IGeoComGrabModel?>> Create()
        {
            var GrabbedResult =await _wellcomeGrabber.GetWebSiteItems();
            _dataAccess.SaveGrabbedData(igeoComGrabModel.InsertSql, GrabbedResult);
            return GrabbedResult;
        }
        */
    }
}
