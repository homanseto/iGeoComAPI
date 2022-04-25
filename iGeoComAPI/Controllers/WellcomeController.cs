using iGeoComAPI.Models;
using iGeoComAPI.Services;
using Microsoft.AspNetCore.Mvc;
using iGeoComAPI.Options;
using Microsoft.Extensions.Options;
using iGeoComAPI.Utilities;
using iGeoComAPI.Repository;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WellcomeController : ControllerBase
    {
        private readonly ILogger<WellcomeController> _logger;
        private readonly WellcomeGrabber _wellcomeGrabber;
        private readonly IGeoComGrabRepository _iGeoComGrabRepository;
        private readonly IGeoComRepository _iGeoComRepository;

        public WellcomeController(WellcomeGrabber wellcomeGrabber, ILogger<WellcomeController> logger, IGeoComGrabRepository iGeoComGrabRepository, IGeoComRepository iGeoComRepository)
        {
            _wellcomeGrabber = wellcomeGrabber;
            _logger = logger;
            _iGeoComGrabRepository = iGeoComGrabRepository;
            _iGeoComRepository = iGeoComRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                string name = this.GetType().Name.Replace("Controller", "").ToLower();
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
        [HttpGet("download")]
        public async Task<IActionResult> GetDownload()
        {
            try
            {
                string name = this.GetType().Name.Replace("Controller", "").ToLower();
                
                var result = await _iGeoComGrabRepository.GetShopsByName(name);
                return CsvFile.Download(result, name);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("delta/download")]
        public async Task<IActionResult> GetDelta()
        {
            try
            {
                string name = this.GetType().Name.Replace("Controller", "").ToLower();
                var previousResult = await _iGeoComRepository.GetShops("wellcome sup");
                var newResult = await _iGeoComGrabRepository.GetShopsByName(name);
                var removedResult = _wellcomeGrabber.FindRemoved(previousResult, newResult);
                var addedResult = _wellcomeGrabber.FindAdded(newResult, previousResult);
                var leftResult = _wellcomeGrabber.LeftIntersection(newResult, addedResult);
                var rightResult = _wellcomeGrabber.RightIntersection(previousResult, removedResult);
                var orgModified = _wellcomeGrabber.orgModified(rightResult, leftResult);
                var newModified = _wellcomeGrabber.newModified(leftResult, rightResult);
                var result = _wellcomeGrabber.MergeResults(addedResult, removedResult, newModified, orgModified);
                return CsvFile.Download(result, $"{name}_delta");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        public async Task<List<IGeoComGrabModel>?> Post()
        {
            var GrabbedResult = await _wellcomeGrabber.GetWebSiteItems();
            _iGeoComGrabRepository.CreateShops(GrabbedResult);
            return GrabbedResult;
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
            CsvFile.DownloadCsv(finalResult, "Wellcome_complete_Result");
            return finalResult;
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
