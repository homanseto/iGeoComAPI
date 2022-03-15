using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace iGeoComAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllController : ControllerBase
    {
        private ILogger<AllController> _logger;
        private DataAccess _dataAccess;
        private string SelectSMK = "SELECT * FROM igeocomtable WHERE TYPE = 'SMK'";

        public AllController( ILogger<AllController> logger, DataAccess dataAccess)
        {
            _logger = logger;
            _dataAccess = dataAccess;
        }

        public async Task<List<IGeoComGrabModel>?> Get()
        {
            var result = await _dataAccess.LoadData<IGeoComGrabModel>(SelectSMK);
            CsvFile.DownloadCsv(result, "SMK_grab_result");
            return result;
        }
    }
}
