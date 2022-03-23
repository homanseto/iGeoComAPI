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
    public class SevenElevenController : ControllerBase

    {
        private readonly ILogger<SevenElevenController> _logger;
        private IGrabberAPI<SevenElevenModel> _sevenElevenGrabber;
        private readonly DataAccess _dataAccess;

        SevenElevenModel sevenElevenModel = new SevenElevenModel();
        IGeoComModel igeoComModel = new IGeoComModel();

        public SevenElevenController(IGrabberAPI<SevenElevenModel> sevenElevenGrabber, ILogger<SevenElevenController> logger, DataAccess dataAccess)
        {
            _sevenElevenGrabber = sevenElevenGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<List<IGeoComGrabModel>?> Get()
        {
           // var GrabbedResult = await _sevenElevenGrabber.GetWebSiteItems();
           // _dataAccess.SaveGrabbedData(InsertSql, GrabbedResult);
            var result = await _dataAccess.LoadData<IGeoComGrabModel>(sevenElevenModel.SelectSevenEleven);
            List<String> lists  = new List<string> { "Latitude", "Longitude"};
            CsvFile.DownloadCsv(result, "SevenEleven_grab_Result");
            return result;
        }


        /*
        [HttpGet("cache")]
        public List<IGeoComGrabModel>? GetTodoItem()
        {
            var result = _dataAccess.LoadDataCache<IGeoComGrabModel>();
            return result.Where(r => r.Grab_ID.Contains("seveneleven")).ToList();
        }
        
        [HttpGet("database")]
        public async Task<List<IGeoComGrabModel>?> GetTodoItem()
        {
            var previousResult = await _dataAccess.LoadData<IGeoComModel>(SelectSevenElevenFromDataBase);
            var newResult = await _dataAccess.LoadData<IGeoComGrabModel>(SelectSevenEleven);
            var finalResult = _sevenElevenGrabber.FindAdded(newResult, previousResult);
            return finalResult;
        }
        */
        
        [HttpPost]
        public async Task<List<IGeoComGrabModel?>> Create()
        {
            var GrabbedResult = await _sevenElevenGrabber.GetWebSiteItems();
            _dataAccess.SaveGrabbedData(igeoComModel.InsertSql, GrabbedResult);
            return GrabbedResult;
        }
        

    }

}
