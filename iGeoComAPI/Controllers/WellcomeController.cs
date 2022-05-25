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

                var previousResult = await _iGeoComRepository.GetShops(4);
                //var newResult = await _iGeoComGrabRepository.GetShopsByName(name);
                var newResult = await _iGeoComGrabRepository.GetShopsByShopId(4);
                var result = Comparator.GetComparedResult(newResult, previousResult);
                return CsvFile.Download(result, $"{name}_delta");
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

        //        var previousResult = await _iGeoComRepository.GetShops("wellcome sup");
        //        var newResult = await _iGeoComGrabRepository.GetShopsByName(name);
        //        var result = Comparator.GetComparedResult(newResult, previousResult);
        //        return CsvFile.Download(result, $"{name}_delta");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }

        //}

        [HttpPost]
        public async Task<List<IGeoComGrabModel>?> Post()
        {
            var GrabbedResult = await _wellcomeGrabber.GetWebSiteItems();
            _iGeoComGrabRepository.CreateShops(GrabbedResult);
            return GrabbedResult;
        }
  
    }
}
