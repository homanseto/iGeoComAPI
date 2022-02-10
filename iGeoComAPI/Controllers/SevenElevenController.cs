using iGeoComAPI.Models;
using iGeoComAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SevenElevenController : ControllerBase
    {
        static readonly HttpClient _client = new HttpClient();
        SevenElevenGrabber sevenElevenGrabber = new SevenElevenGrabber(_client);


        private readonly ILogger<SevenElevenController> _logger;

        public SevenElevenController(ILogger<SevenElevenController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetSevenElevenModel")]
        public async Task<List<IGeoComModel>?> Get()
        {
            var result = await sevenElevenGrabber.GetWebSiteItems();
            return result;
        }
    }

}
