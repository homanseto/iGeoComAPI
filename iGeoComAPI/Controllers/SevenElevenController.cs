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
        private readonly MyLogger _logger;
        private IGrabberAPI<SevenElevenModel> _sevenElevenGrabber;
        private readonly DataAccess _dataAccess;

        SevenElevenModel sevenElevenModel = new SevenElevenModel();
        //IGeoComGrabModel igeoComGrabModel = new IGeoComGrabModel();

        public SevenElevenController(IGrabberAPI<SevenElevenModel> sevenElevenGrabber, MyLogger logger, DataAccess dataAccess)
        {
            _sevenElevenGrabber = sevenElevenGrabber;
            _logger = logger;
            _dataAccess = dataAccess;
        }
        /*
        [HttpGet]
        public async Task<List<IGeoComGrabModel>?> Get()
        {
            _logger.LogControllerRequest(nameof(SevenElevenController), nameof(Get));
            // var GrabbedResult = await _sevenElevenGrabber.GetWebSiteItems();
            // _dataAccess.SaveGrabbedData(InsertSql, GrabbedResult);
            var result = await _dataAccess.LoadData<IGeoComGrabModel>(sevenElevenModel.SelectSevenElevenFromDataBase);
            CsvFile.DownloadCsv(result, "SevenEleven_grab_Result");

            return result;
        }


        
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
        

        [HttpPost]
        public async Task<List<IGeoComGrabModel>?> Post()
        {
            _logger.LogControllerRequest(nameof(SevenElevenController), nameof(Post));
            var GrabbedResult = await _sevenElevenGrabber.GetWebSiteItems();
            _dataAccess.SaveGrabbedData(igeoComGrabModel.InsertSql, GrabbedResult);
            return GrabbedResult;
        }

        [HttpDelete]
        public async Task DeleteAllFromCache()
        {
            await _dataAccess.DeleteDataFromDataBase<IGeoComGrabModel>(sevenElevenModel.DeleteSevenElevenFromGrabbedCache);
        }
        */


    }

}
