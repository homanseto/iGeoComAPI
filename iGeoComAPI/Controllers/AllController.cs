using iGeoComAPI.Models;
using iGeoComAPI.Repository;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllController : ControllerBase
    {
        private readonly ILogger<AllController> _logger;
        private readonly IGeoComGrabRepository _iGeoComGrabRepository;

        public AllController(ILogger<AllController> logger, IGeoComGrabRepository iGeoComGrabRepository)
        {
            _logger = logger;
            _iGeoComGrabRepository = iGeoComGrabRepository;

        }

        [HttpGet("{type}")]
        public async Task<IActionResult> GetShopsByType(string type)
        {
            try
            {
                var result = await _iGeoComGrabRepository.GetShopsByType(type);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("{type}/download")]
        public async Task<IActionResult> GetDownload(string type)
        {
            try
            {
                var result = await _iGeoComGrabRepository.GetShopsByType(type);
                return CsvFile.Download(result, type);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        /*
        [Route("api/@ID/@action")]
        public async bool DO(string ID, string action)
        {
            swtich(ID){
                
            }
        }
        */
    }
}
