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
    public class WmoovController : ControllerBase
    {
        private readonly ILogger<WmoovController> _logger;
        private readonly WmoovGrabber _wmoovGrabber;
        private readonly IGeoComGrabRepository _iGeoComGrabRepository;
        public WmoovController(WmoovGrabber wmoovGrabber, ILogger<WmoovController> logger, IGeoComGrabRepository iGeoComGrabRepository)
        {
            _wmoovGrabber = wmoovGrabber;
            _logger = logger;
            _iGeoComGrabRepository = iGeoComGrabRepository;
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
                return Utilities.File.Download(result, name);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var GrabbedResult = await _wmoovGrabber.GetWebSiteItems();
            _iGeoComGrabRepository.CreateShops(GrabbedResult);
            return Ok(GrabbedResult);
        }

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
