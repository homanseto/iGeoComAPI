using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class AromeNMaximsCakesGrabber : IGrabberAPI<AromeNMaximsCakesModel>
    {
        private PuppeteerConnection _puppeteerConnection;
        private IOptions<AromeNMaximsCakesOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<AromeNMaximsCakesGrabber> _logger;
        private string shopNumber = @"()=>{
                                 const selector = document.querySelector('td#contentcell > table > tbody > tr > .general_txt');
                                 return selector.textContent.trim();
                                 }";
        private string infoCode1stLevel = @"()=>{
                                 const selectors = Array.from(document.querySelectorAll('td#contentcell > table > tbody > tr'));
                                 return selectors.map(v=>{return v.querySelector('td.shoplink') != null ? {Website: v.querySelector('td.shoplink').getAttribute('onclick')}: '';
                                 })
                                 }";
        private string infoCode2ndLevel = @"()=>{
                                 const selector = Array.from(document.querySelectorAll('table.restaurant_details_tb > tbody > tr > td > table > tbody >tr'));
                                 return {Name: selector[0].querySelector('.restaurant_location_txt').textContent.trim(),Address: selector[1].querySelector('.restaurant_location_txt').textContent.trim() ,
                                 Phone: selector[2].querySelector('.restaurant_location_txt').textContent.trim(),Fax: selector[3].querySelector('.restaurant_location_txt').textContent.trim()}
                                 }";
        private string waitSelector1st = "#contentcell";
        private string waitSelector2nd = ".restaurant_details_tb";

        public AromeNMaximsCakesGrabber(PuppeteerConnection puppeteerConnection, IOptions<AromeNMaximsCakesOptions> options, IMemoryCache memoryCache, ILogger<AromeNMaximsCakesGrabber> logger)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            string enNumberOfShop = await _puppeteerConnection.PuppeteerGrabber<string>($"{_options.Value.EnUrl}1", shopNumber, waitSelector1st);
            var testResult =await Extract1stLevelData(EachPageNumberOfShops(enNumberOfShop));
            var enGrabResult = await Extract2stLevelData(testResult, _options.Value.EnSearchpath);
            var zhGrabResult = await Extract2stLevelData(testResult, _options.Value.ZhSearchpath);
            var mergeResult = MergeEnAndZh(enGrabResult, zhGrabResult);
            return mergeResult;
        }

        public decimal EachPageNumberOfShops(string number)
        {
            var _rgx = Regexs.ExtractInfo(AromeNMaximsCakesModel.NumberofShopRegex);
            decimal num = decimal.Parse(_rgx.Match(number).Groups[1].Value);
            decimal EachPageNum = Math.Ceiling((num / Convert.ToDecimal(_options.Value.EachPageNumber)));
            return EachPageNum;
            
        }

        public async Task<List<AromeNMaximsCakesModel>?> Extract1stLevelData(decimal page)
        {
            List<AromeNMaximsCakesModel> AromeNMaximsCakes1stList = new List<AromeNMaximsCakesModel>();
            for (int i =1; i<=page; i++)
            {
                var en1stData = await _puppeteerConnection.PuppeteerGrabber<AromeNMaximsCakesModel[]>($"{_options.Value.EnUrl}{i}", infoCode1stLevel, waitSelector1st);
                var en1stDataList = en1stData.ToList();
                AromeNMaximsCakes1stList = AromeNMaximsCakes1stList.Concat(en1stDataList).ToList();
            }
            return AromeNMaximsCakes1stList.Where(website => website != null).ToList();
        }

        public async Task<List<AromeNMaximsCakesModel>?> Extract2stLevelData(List<AromeNMaximsCakesModel>? pathList, string? searchPath)
        {
            var _pathRgx = Regexs.ExtractInfo(AromeNMaximsCakesModel.RestaurantPathRegex);
            var _idRgx = Regexs.ExtractInfo(AromeNMaximsCakesModel.IdRegex);
            List<AromeNMaximsCakesModel> AromeNMaximsCakesList = new List<AromeNMaximsCakesModel>();
            foreach (var path in pathList)
            {
                var extract2ndData = await _puppeteerConnection.PuppeteerGrabber<AromeNMaximsCakesModel>($"{searchPath}{_pathRgx.Match(path.Website!).Groups[1].Value}", infoCode2ndLevel, waitSelector2nd);
                extract2ndData.Id = _idRgx.Match(path.Website!).Groups[1].Value;
                AromeNMaximsCakesList.Add(extract2ndData);
            }
            return AromeNMaximsCakesList;
        }



        public  List<IGeoComGrabModel> MergeEnAndZh(List<AromeNMaximsCakesModel> enResult, List<AromeNMaximsCakesModel> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge Wellcome En and Zh");
                List<IGeoComGrabModel> AromeNMaximsCakesIGeoComList = new List<IGeoComGrabModel>();
                foreach (var shopEn in enResult)
                {
                    IGeoComGrabModel AromeNMaximsCakesIGeoCom = new IGeoComGrabModel();
                    AromeNMaximsCakesIGeoCom.GrabId = $"aromeNMaximsCakes_{shopEn.Id}";
                    AromeNMaximsCakesIGeoCom.EnglishName = shopEn.Name;
                    AromeNMaximsCakesIGeoCom.E_Address = shopEn.Address;
                    AromeNMaximsCakesIGeoCom.Tel_No = shopEn.Phone;
                    AromeNMaximsCakesIGeoCom.Fax_No = shopEn.Fax;
                    foreach (var shopZh in zhResult)
                    {
                        if (shopEn.Id == shopZh.Id)
                        {
                            AromeNMaximsCakesIGeoCom.ChineseName = shopZh.Name.Replace(" ", "");
                            AromeNMaximsCakesIGeoCom.C_Address = shopZh.Address;
                        }
                    }
                    AromeNMaximsCakesIGeoComList.Add(AromeNMaximsCakesIGeoCom);
                }
                return AromeNMaximsCakesIGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
