using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllController : ControllerBase
    {
        private readonly ILogger<AllController> _logger;
        private readonly IGeoComModel _iGeoComModel;

        public AllController(ILogger<AllController> logger, IGeoComModel iGeoComModel)
        {
            _logger = logger;
            _iGeoComModel = iGeoComModel;

        }

        [HttpGet("{type}")]
        public async Task<IActionResult> GetShops(string type)
        {
            try
            {
                var result = await _iGeoComModel.GetShops(type);
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
