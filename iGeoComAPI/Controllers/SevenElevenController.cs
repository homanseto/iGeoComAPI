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
    public class SevenElevenController : ControllerBase
    {
        private readonly MyLogger logger;
        private readonly SevenElevenGrabber sevenElevenGrabber;
        private readonly IGeoComGrabRepository iGeoComGrabRepository;
        private readonly IGeoComRepository iGeoComRepository;
        SevenElevenModel sevenElevenModel = new SevenElevenModel();
        //IGeoComGrabModel igeoComGrabModel = new IGeoComGrabModel();

        public SevenElevenController(SevenElevenGrabber sevenElevenGrabber, MyLogger logger, IGeoComGrabRepository iGeoComGrabRepository, IGeoComRepository iGeoComRepository)
        {
            this.sevenElevenGrabber = sevenElevenGrabber;
            this.logger = logger;
            this.iGeoComGrabRepository = iGeoComGrabRepository;
            this.iGeoComRepository = iGeoComRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<IGeoComGrabModel>>> Get()
        {
            try
            {
                string name = this.GetType().Name.Replace("Controller", "").ToLower();
                var result = await this.iGeoComGrabRepository.GetShopsByName(name);
                //var result = await dataContext.IGeoComGrabModels.ToListAsync();
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
        public async Task<ActionResult> GetDownload()
        {
            try
            {
                string name = this.GetType().Name.Replace("Controller", "").ToLower();
                var result = await this.iGeoComGrabRepository.GetShopsByName(name);
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
                var newResult = await iGeoComGrabRepository.GetShopsByShopId("cvs1");
                var oldResult = await iGeoComRepository.GetShops("cvs1");
                string[] ignoreList = new string[] { "GeoNameId", "EnglishName", "ChineseName", "Class", "Type", "Subcat", "Easting","Northing","Source",
                    "E_floor", "C_floor", "E_sitename","C_sitename","E_area","C_area","C_Region", "E_Region", "C_District","E_District", "Fax_No", "Tel_No","Web_Site",
                    "E_Address", "C_Address","Latitude", "Longitude","ShopId","Rev_Date","GrabId", "Compare_ChineseName", "Compare_EnglishName", "Compare_Tel"};
                var resultList = Comparator2.GetComparedResult(newResult, oldResult, ignoreList);
                return Utilities.File.Download(resultList, $"sevenEleven_delta");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<List<IGeoComGrabModel>>> Post()
        {
            this.logger.LogControllerRequest(nameof(SevenElevenController), nameof(Post));
            var GrabbedResult = await this.sevenElevenGrabber.GetWebSiteItems();
            if (GrabbedResult == null)
                return BadRequest("Cannot insert grabbed data");
            this.iGeoComGrabRepository.CreateShops(GrabbedResult);
            //this.dataContext.IGeoComGrabModels.AddRange(GrabbedResult);
            //await this.dataContext.SaveChangesAsync();
            return Ok(GrabbedResult);
        }
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<IGeoComModel>> Delete(string id)
        //{

        //}

    }

}
