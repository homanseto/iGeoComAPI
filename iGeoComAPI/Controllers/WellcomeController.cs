using iGeoComAPI.Models;
using iGeoComAPI.Services;
using Microsoft.AspNetCore.Mvc;
using iGeoComAPI.Options;
using Microsoft.Extensions.Options;
using iGeoComAPI.Utilities;
using iGeoComAPI.Repository;
using Newtonsoft.Json;

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
                return Utilities.File.Download(result, name);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("delta/download")]
        public async Task<ActionResult> Testing()
        {
            try
            {
                var newResult = await _iGeoComGrabRepository.GetShopsByShopId("smk2");
                var oldResult = await _iGeoComRepository.GetShops("smk2");
                string[] ignoreList = new string[] { "GeoNameId", "EnglishName", "ChineseName", "Class", "Type", "Subcat", "Easting","Northing","Source",
                    "E_floor", "C_floor", "E_sitename","C_sitename","E_area","C_area","C_Region", "E_Region", "C_District","E_District", "Fax_No", "Tel_No","Web_Site",
                    "E_Address", "C_Address","Latitude", "Longitude","ShopId","Rev_Date","GrabId", "Compare_ChineseName", "Compare_EnglishName", "Compare_Tel"};
                var resultList = Comparator2.GetComparedResult(newResult, oldResult, ignoreList);
                return Utilities.File.Download(resultList, $"wellcome_delta");
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
        public async Task<IActionResult> Post()
        {
            var GrabbedResult = await _wellcomeGrabber.GetWebSiteItems();
            _iGeoComGrabRepository.CreateShops(GrabbedResult);
            return Ok(GrabbedResult);
        }
  
    }
}
