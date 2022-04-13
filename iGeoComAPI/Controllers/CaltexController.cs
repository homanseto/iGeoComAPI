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
    public class CaltexController : ControllerBase
    {
        private  ILogger<CaltexController> _logger;
        private IGrabberAPI<CaltexModel> _caltexGrabber;
        private  DataAccess _dataAccess;
        private IGeoComGrabRepository _iGeoComGrabRepository;

        IGeoComGrabModel igeoComGrabModel = new IGeoComGrabModel();
        CaltexModel caltexModel = new CaltexModel();

        public CaltexController(IGrabberAPI<CaltexModel> caltexGrabber, ILogger<CaltexController> logger, IGeoComGrabRepository iGeoComGrabRepository)
        {
            _caltexGrabber = caltexGrabber;
            _logger = logger;
            _iGeoComGrabRepository = iGeoComGrabRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _iGeoComGrabRepository.GetShopsByName("%caltex_%");
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
        [HttpGet]
        public async Task<List<IGeoComGrabModel>?> Get()
        {
            //var GrabbedResult = await _caltexGrabber.GetWebSiteItems();
            //_dataAccess.SaveGrabbedData(InsertSql, GrabbedResult);
            var result = await _dataAccess.LoadData<IGeoComGrabModel>(caltexModel.SelectCaltex);
            CsvFile.DownloadCsv(result, "Caltex_grab_result");
            return result;
        }

        [HttpPost]
        public async Task<List<IGeoComGrabModel?>> Create()
        {
            var GrabbedResult = await _caltexGrabber.GetWebSiteItems();
            _dataAccess.SaveGrabbedData(igeoComGrabModel.InsertSql, GrabbedResult);
            return GrabbedResult;
        }
        */
    }
}
