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
        private readonly DataContext dataContext;
        SevenElevenModel sevenElevenModel = new SevenElevenModel();
        //IGeoComGrabModel igeoComGrabModel = new IGeoComGrabModel();

        public SevenElevenController(SevenElevenGrabber sevenElevenGrabber, MyLogger logger, IGeoComGrabRepository iGeoComGrabRepository, DataContext dataContext, IGeoComRepository iGeoComRepository)
        {
            this.sevenElevenGrabber = sevenElevenGrabber;
            this.logger = logger;
            this.iGeoComGrabRepository = iGeoComGrabRepository;
            this.dataContext = dataContext;
            this.iGeoComRepository = iGeoComRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<IGeoComGrabModel>>> Get()
        {
            try
            {
                string name = this.GetType().Name.Replace("Controller", "").ToLower();
                //var result = await this.iGeoComGrabRepository.GetShopsByName(name);
                var result = await dataContext.IGeoComGrabModels.ToListAsync();
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
        public async Task<ActionResult<FileStreamResult>> GetDownload()
        {
            try
            {
                string name = this.GetType().Name.Replace("Controller", "").ToLower();
                var result = await this.iGeoComGrabRepository.GetShopsByName(name);
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

                var previousResult = await this.iGeoComRepository.GetShops("7-eleven");
                var newResult = await this.iGeoComGrabRepository.GetShopsByName(name);
                var result = Comparator.GetComparedResult(newResult, previousResult, "tel");
                return CsvFile.Download(result, $"{name}_delta");
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
            //this.iGeoComGrabRepository.CreateShops(GrabbedResult);
            this.dataContext.IGeoComGrabModels.AddRange(GrabbedResult);
            await this.dataContext.SaveChangesAsync();
            return Ok(GrabbedResult);
        }
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<IGeoComModel>> Delete(string id)
        //{

        //}

    }

}
