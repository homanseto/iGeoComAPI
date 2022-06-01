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
                //List<IGeoComDeltaModel> addedMergeResult = new List<IGeoComDeltaModel>();
                //List<IGeoComDeltaModel> removedMergeResult = new List<IGeoComDeltaModel>();
                //List<IGeoComDeltaModel> deltaMergeResult = new List<IGeoComDeltaModel>();
                //for (int i =1; i <= 5; i++)
                //{
                //    var previousResult = await _iGeoComRepository.GetShops(i);
                //    var newResult = await _iGeoComGrabRepository.GetShopsByShopId(i);
                //    var add1esult = Comparator.GetAdded(newResult, previousResult);
                //    var removeResult = Comparator.GetRemoved(previousResult, newResult);
                //    var deltaResult = Comparator.GetDelta(newResult, previousResult);
                //    addedMergeResult = addedMergeResult.Concat(add1esult).ToList();
                //    removedMergeResult = removedMergeResult.Concat(removeResult).ToList();
                //    deltaMergeResult = deltaMergeResult.Concat(deltaResult).ToList();
                //}
                //var result = addedMergeResult.Concat(removedMergeResult).Concat(deltaMergeResult).ToList();
                var previousResult1 = await _iGeoComRepository.GetShops(1);
                var newResult1 = await _iGeoComGrabRepository.GetShopsByShopId(1);
                var add1esult1 = Comparator.GetAdded(newResult1, previousResult1, "tel");
                var removeResult1 = Comparator.GetRemoved(newResult1, previousResult1, "tel");
                var deltaResult1 = Comparator.GetDelta(newResult1, previousResult1, "tel");
                var previousResult2 = await _iGeoComRepository.GetShops(2);
                var newResult2 = await _iGeoComGrabRepository.GetShopsByShopId(2);
                var addResult2 = Comparator.GetAdded(newResult2, previousResult2, "tel");
                var removeResult2 = Comparator.GetRemoved(newResult2, previousResult2, "tel");
                var deltaResult2 = Comparator.GetDelta(newResult2, previousResult2, "tel");
                var previousResult3 = await _iGeoComRepository.GetShops(3);
                var newResult3 = await _iGeoComGrabRepository.GetShopsByShopId(3);
                var addResult3 = Comparator.GetAdded(newResult3, previousResult3,"vango");
                var removeResult3 = Comparator.GetRemoved(newResult3, previousResult3, "vango");
                var deltaResult3 = Comparator.GetDelta(newResult3, previousResult3, "vango");
                var previousResult4 = await _iGeoComRepository.GetShops(4);
                var newResult4 = await _iGeoComGrabRepository.GetShopsByShopId(4);
                var addResult4 = Comparator.GetAdded(newResult4, previousResult4);
                var removeResult4 = Comparator.GetRemoved(newResult4, previousResult4);
                var deltaResult4 = Comparator.GetDelta(newResult4, previousResult4);
                var previousResult5 = await _iGeoComRepository.GetShops(5);
                var newResult5 = await _iGeoComGrabRepository.GetShopsByShopId(5);
                var addResult5 = Comparator.GetAdded(newResult5, previousResult5);
                var removeResult5 = Comparator.GetRemoved(newResult5, previousResult5);
                var deltaResult5 = Comparator.GetDelta(newResult5, previousResult5);
                var previousResult6 = await _iGeoComRepository.GetShops(6);
                var newResult6 = await _iGeoComGrabRepository.GetShopsByShopId(6);
                var addResult6 = Comparator.GetAdded(newResult6, previousResult6);
                var removeResult6 = Comparator.GetRemoved(newResult6, previousResult6);
                var deltaResult6 = Comparator.GetDelta(newResult6, previousResult6);
                var previousResult7 = await _iGeoComRepository.GetShops(7);
                var newResult7 = await _iGeoComGrabRepository.GetShopsByShopId(7);
                var addResult7 = Comparator.GetAdded(newResult7, previousResult7);
                var removeResult7 = Comparator.GetRemoved(newResult7, previousResult7);
                var deltaResult7 = Comparator.GetDelta(newResult7, previousResult7);
                var previousResult8 = await _iGeoComRepository.GetShops(8);
                var newResult8 = await _iGeoComGrabRepository.GetShopsByShopId(8);
                var addResult8 = Comparator.GetAdded(newResult8, previousResult8);
                var removeResult8 = Comparator.GetRemoved(newResult8, previousResult8);
                var deltaResult8 = Comparator.GetDelta(newResult8, previousResult8);
                var previousResult9 = await _iGeoComRepository.GetShops(9);
                var newResult9 = await _iGeoComGrabRepository.GetShopsByShopId(9);
                var addResult9 = Comparator.GetAdded(newResult9, previousResult9, "ambulance");
                var removeResult9 = Comparator.GetRemoved(newResult9, previousResult9, "ambulance");
                var deltaResult9 = Comparator.GetDelta(newResult9, previousResult9, "ambulance");
                var previousResult11 = await _iGeoComRepository.GetShops(11);
                var newResult11 = await _iGeoComGrabRepository.GetShopsByShopId(11);
                var addResult11 = Comparator.GetAdded(newResult11, previousResult11);
                var removeResult11 = Comparator.GetRemoved(newResult11, previousResult11);
                var deltaResult11 = Comparator.GetDelta(newResult11, previousResult11);
                var previousResult12 = await _iGeoComRepository.GetShops(12);
                var newResult12 = await _iGeoComGrabRepository.GetShopsByShopId(12);
                var addResult12 = Comparator.GetAdded(newResult12, previousResult12,"tel");
                var removeResult12 = Comparator.GetRemoved(newResult12, previousResult12, "tel");
                var deltaResult12 = Comparator.GetDelta(newResult12, previousResult12, "tel");
                var addedMerge = add1esult1.Concat(addResult2).Concat(addResult3).Concat(addResult4).Concat(addResult5).Concat(addResult6).Concat(addResult7).Concat(addResult8).Concat(addResult9).Concat(addResult11).Concat(addResult12).ToList();
                var removeMerge = removeResult1.Concat(removeResult2).Concat(removeResult3).Concat(removeResult4).Concat(removeResult5).Concat(removeResult6).Concat(removeResult7).Concat(removeResult8).Concat(removeResult9).Concat(removeResult11).Concat(removeResult12).ToList();
                var deltaMege = deltaResult1.Concat(deltaResult2).Concat(deltaResult3).Concat(deltaResult4).Concat(deltaResult5).Concat(deltaResult6).Concat(deltaResult7).Concat(deltaResult8).Concat(deltaResult9).Concat(deltaResult11).Concat(deltaResult12).ToList();
                var result = addedMerge.Concat(removeMerge).Concat(deltaMege).ToList();
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
