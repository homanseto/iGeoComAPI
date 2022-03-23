using iGeoComAPI.Models;
using iGeoComAPI.Services;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodDonorCentreController : ControllerBase
    {
        private ILogger<BloodDonorCentreController> _logger;
        private BloodDonorCentreGrabber _bloodDonorCentreGrabber;
        private DataAccess _dataAccess;

        IGeoComModel igeoComModel = new IGeoComModel();
        BloodDonorCentreModel bloodDonorCentreModel = new BloodDonorCentreModel();

        public BloodDonorCentreController(BloodDonorCentreGrabber bloodDonorCentreGrabber, ILogger<BloodDonorCentreController> logger, DataAccess dataAccess)
        {
            _bloodDonorCentreGrabber = bloodDonorCentreGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var result = await _bloodDonorCentreGrabber.GetWebSiteItems();
            return result;
        }
    }
}
