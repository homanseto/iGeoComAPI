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
        private readonly IGeoComRepository _iGeoComRepository;

        public AllController(ILogger<AllController> logger, IGeoComGrabRepository iGeoComGrabRepository, IGeoComRepository iGeoComRepository)
        {
            _logger = logger;
            _iGeoComGrabRepository = iGeoComGrabRepository;
            _iGeoComRepository = iGeoComRepository;

        }

        [HttpGet("{type}")]
        public async Task<ActionResult<List<IGeoComGrabModel>>> GetShopsByType(string type)
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
        public async Task<ActionResult<FileStreamResult>> GetDownload(string type)
        {
            try
            {
                var result = await _iGeoComGrabRepository.GetShopsByType(type);
                if (result == null)
                    return BadRequest("type not found.");
                return Utilities.File.Download(result, "All");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("delta/download")]
        public async Task<IActionResult> GetDelta()
        {
            try
            {
                string name = this.GetType().Name.Replace("Controller", "").ToLower();
                List<IGeoComDeltaModel> addedMergeResult = new List<IGeoComDeltaModel>();
                List<IGeoComDeltaModel> removedMergeResult = new List<IGeoComDeltaModel>();
                List<IGeoComDeltaModel> deltaMergeResult = new List<IGeoComDeltaModel>();
                for (int i =1; i <= 5; i++)
                {
                    var previousResult = await _iGeoComRepository.GetShops(i);
                    var newResult = await _iGeoComGrabRepository.GetShopsByShopId(i);
                    var add1esult = Comparator.GetAdded(newResult, previousResult);
                    var removeResult = Comparator.GetRemoved(previousResult, newResult);
                    var deltaResult = Comparator.GetDelta(newResult, previousResult);
                    addedMergeResult = addedMergeResult.Concat(add1esult).ToList();
                    removedMergeResult = removedMergeResult.Concat(removeResult).ToList();
                    deltaMergeResult = deltaMergeResult.Concat(deltaResult).ToList();
                }
                var result = addedMergeResult.Concat(removedMergeResult).Concat(deltaMergeResult).ToList();
                //var previousResult1 = await _iGeoComRepository.GetShops(1);
                //var newResult1 = await _iGeoComGrabRepository.GetShopsByShopId(1);
                //var add1esult1 = Comparator.GetAdded(newResult1, previousResult1);
                //var removeResult1 = Comparator.GetRemoved(previousResult1, newResult1);
                //var deltaResult1 = Comparator.GetDelta(newResult1, previousResult1);
                //var previousResult2 = await _iGeoComRepository.GetShops(2);
                //var newResult2 = await _iGeoComGrabRepository.GetShopsByShopId(2);
                //var addResult2 = Comparator.GetAdded(newResult2, previousResult2);
                //var removeResult2 = Comparator.GetRemoved(previousResult2, newResult2);
                //var deltaResult2 = Comparator.GetDelta(newResult2, previousResult2);
                //var previousResult3 = await _iGeoComRepository.GetShops(2);
                //var newResult3 = await _iGeoComGrabRepository.GetShopsByShopId(2);
                //var addResult3 = Comparator.GetAdded(newResult2, previousResult2);
                //var removeResult3 = Comparator.GetRemoved(previousResult2, newResult3);
                //var deltaResult3 = Comparator.GetDelta(newResult3, previousResult3);
                //var addedMerge = add1esult1.Concat(addResult2).ToList();
                //var removeMerge = removeResult1.Concat(removeResult2).ToList();
                //var deltaMege = deltaResult1.Concat(deltaResult2).ToList();
                //var result = addedMerge.Concat(removeMerge).Concat(deltaMege).ToList();
                return Utilities.File.DownloadDelta(result, $"{name}_delta");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("download")]
        public async Task<IActionResult> GetDownloadAll()
        {
            try
            {
                var result = await _iGeoComGrabRepository.GetShops();
                if (result == null)
                    return BadRequest("type not found.");
                return Utilities.File.Download(result, "allshop");
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
