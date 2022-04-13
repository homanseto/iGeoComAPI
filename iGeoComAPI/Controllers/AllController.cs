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
        private readonly IGeoComRepository _iGeoComRepository;

        public AllController(ILogger<AllController> logger, IGeoComRepository iGeoComRepository)
        {
            _logger = logger;
            _iGeoComRepository = iGeoComRepository;

        }

        [HttpGet("{type}")]
        public async Task<IActionResult> GetShops(string type)
        {
            try
            {
                var result = await _iGeoComRepository.GetShops(type);
                if (result == null)
                    return NotFound();
                return Ok(result);
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
