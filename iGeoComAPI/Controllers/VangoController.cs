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
        private IGeoComRepository _iGeoComRepository;

        public VangoController(VangoGrabber vangoGrabber, ILogger<VangoController> logger, IGeoComGrabRepository iGeoComGrabRepository, IGeoComRepository iGeoComRepository)
        {
            _vangoGrabber = vangoGrabber;
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
                return Utilities.File.Download(result, name);
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

                var previousResult = await _iGeoComRepository.GetShops(3);
                //var newResult = await _iGeoComGrabRepository.GetShopsByName(name);
                var newResult = await _iGeoComGrabRepository.GetShopsByShopId(3);
                var result = Comparator.GetComparedResult(newResult, previousResult, "vango");
                return Utilities.File.Download(result, $"{name}_delta");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        //[HttpGet("delta/download")]
        //public async Task<IActionResult> GetDelta()
        //{
        //    try
        //    {
        //        string name = this.GetType().Name.Replace("Controller", "").ToLower();

        //        var previousResult = await _iGeoComRepository.GetShops("vango");
        //        var newResult = await _iGeoComGrabRepository.GetShopsByName(name);
        //        var result = Comparator.GetComparedResult(newResult, previousResult, "eAddress");
        //        return CsvFile.Download(result, $"{name}_delta");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }

        //}

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var GrabbedResult = await _vangoGrabber.GetWebSiteItems();
            _iGeoComGrabRepository.CreateShops(GrabbedResult);
            return Ok(GrabbedResult);
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
