using iGeoComAPI.Models;
using iGeoComAPI.Services;
using Microsoft.AspNetCore.Mvc;
using iGeoComAPI.Options;
using Microsoft.Extensions.Options;
using iGeoComAPI.Utilities;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WellcomeController : ControllerBase
    {
        private readonly ILogger<WellcomeController> _logger;
        private readonly WellcomeGrabber _wellcomeGrabber;
        private readonly DataAccess _dataAccess;
        private readonly IOptions<DataSQLOptions> _options;
        public WellcomeController(WellcomeGrabber wellcomeGrabber, ILogger<WellcomeController> logger, DataAccess dataAccess, IOptions<DataSQLOptions> options)
        {
            _wellcomeGrabber = wellcomeGrabber;
            _logger = logger;
            _options = options;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<List<IGeoComModel>?> Get()
        {
            var result = await _wellcomeGrabber.GetWebSiteItems();
            _dataAccess.SaveGrabbedData(_options.Value.InsertSql, result);
            return result;
        }
    }
}
