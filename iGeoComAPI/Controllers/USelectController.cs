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
    public class USelectController : ControllerBase
    {
        private readonly ILogger<USelectController> _logger;
        private readonly USelectGrabber _uSelectGrabber;
        private readonly IGeoComGrabRepository _iGeoComGrabRepository;
        private readonly IGeoComRepository _iGeoComRepository;

        public USelectController(USelectGrabber uSelectGrabber, ILogger<USelectController> logger, IGeoComGrabRepository iGeoComGrabRepository, IGeoComRepository iGeoComRepository)
        {
            _uSelectGrabber = uSelectGrabber;
            _logger = logger;
            _iGeoComGrabRepository = iGeoComGrabRepository;
            _iGeoComRepository = iGeoComRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                //string name = this.GetType().Name.Replace("Controller", "").ToLower();
                var result = await _iGeoComGrabRepository.GetShopsByName("U select");
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
                var result = await _iGeoComGrabRepository.GetShopsByName("U select");
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
                var newResult = await _iGeoComGrabRepository.GetShopsByShopId("smk3");
                var oldResult = await _iGeoComRepository.GetShops("smk3");
                string[] ignoreList = new string[] { "GeoNameId", "EnglishName", "ChineseName", "Class", "Type", "Subcat", "Easting","Northing","Source",
                    "E_floor", "C_floor", "E_sitename","C_sitename","E_area","C_area","C_Region", "E_Region", "C_District","E_District", "Fax_No", "Tel_No","Web_Site",
                    "E_Address", "C_Address","Latitude", "Longitude","ShopId","Rev_Date","GrabId", "Compare_ChineseName", "Compare_EnglishName", "Compare_Tel"};
                var resultList = Comparator2.GetComparedResult(newResult, oldResult, ignoreList);
                return Utilities.File.Download(resultList, $"uselect_delta");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var GrabbedResult = await _uSelectGrabber.GetWebSiteItems();
            _iGeoComGrabRepository.CreateShops(GrabbedResult);
            return Ok(GrabbedResult);
        }
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
