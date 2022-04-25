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
        private readonly MyLogger _logger;
        private readonly SevenElevenGrabber _sevenElevenGrabber;
        private IGeoComGrabRepository _iGeoComGrabRepository;

        SevenElevenModel sevenElevenModel = new SevenElevenModel();
        //IGeoComGrabModel igeoComGrabModel = new IGeoComGrabModel();

        public SevenElevenController(SevenElevenGrabber sevenElevenGrabber, MyLogger logger, IGeoComGrabRepository iGeoComGrabRepository)
        {
            _sevenElevenGrabber = sevenElevenGrabber;
            _logger = logger;
            _iGeoComGrabRepository = iGeoComGrabRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<IGeoComGrabModel>>> Get()
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
        public async Task<ActionResult<FileStreamResult>> GetDownload()
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

        [HttpPost]
        public async Task<ActionResult<List<IGeoComGrabModel>>> Post()
        {
            _logger.LogControllerRequest(nameof(SevenElevenController), nameof(Post));
            var GrabbedResult = await _sevenElevenGrabber.GetWebSiteItems();
            if (GrabbedResult == null)
                return BadRequest("Cannot insert grabbed data");
           _iGeoComGrabRepository.CreateShops(GrabbedResult);
            return Ok(GrabbedResult);
        }
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<IGeoComModel>> Delete(string id)
        //{

        //}

    }

}
