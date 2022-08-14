using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class AeonGrabber:AbstractGrabber
    {
        private PuppeteerConnection _puppeteerConnection;
        private IOptions<AeonOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<AeonGrabber> _logger;
        private string infoCode = @"() =>{" +
            @"const selectors = Array.from(document.querySelectorAll('.framebottom > .framecenter > .shopdetail > .shop > .shoplist'));" +
            @"return selectors.map(v => {return {Name: v.querySelector('input').getAttribute('data-name'), Address: v.querySelector('input').getAttribute('data-address')," +
            @"LatLng: v.querySelector('.shopname > a').getAttribute('onclick'), Id: v.querySelector('input').getAttribute('data-id').trim()," +
            @"Phone: v.querySelectorAll('div > ul >li')[2].textContent.trim()" +
            @"}});" +
            @"}";
        private string waitSelector = ".framebottom";
        public AeonGrabber(PuppeteerConnection puppeteerConnection, IOptions<AeonOptions> options, IMemoryCache memoryCache, ILogger<AeonGrabber> logger,
    IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        StringComparison comp = StringComparison.OrdinalIgnoreCase;
        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enResult = await _puppeteerConnection.PuppeteerGrabber<List<AeonModel>>(_options.Value.EnUrl, infoCode, waitSelector);
            var zhResult = await _puppeteerConnection.PuppeteerGrabber<List<AeonModel>>(_options.Value.ZhUrl, infoCode, waitSelector);
            var filterEnResult = enResult.Where(v => !(v.Name.Contains("Living PLAZA", comp) || v.Name.Contains("Daiso", comp) || v.Name.Contains("bento express", comp) || v.Name.Contains("Mono Mono", comp))).ToList();
            var filterZhResult = zhResult.Where(v => !(v.Name.Contains("Living PLAZA", comp) || v.Name.Contains("Daiso", comp) || v.Name.Contains("bento express", comp) || v.Name.Contains("ものもの", comp))).ToList();
            var mergeResult = CreateIGeoCom(filterEnResult, filterZhResult);
            //var result = await this.GetShopInfo(mergeResult);
            //_memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
            return mergeResult;
        }

        public List<IGeoComGrabModel> CreateIGeoCom(List<AeonModel> enResult, List<AeonModel> zhResult)
        {
            List<IGeoComGrabModel> AeonIGeoComList = new List<IGeoComGrabModel>();
            foreach (var shopEn in enResult)
            {
                if (shopEn.Name.Contains("SUPERMARKET", comp))
                {
                    var supermaketShop = MergeEnAndZh(shopEn, zhResult, "supermarket");
                    AeonIGeoComList.Add(supermaketShop);
                }
                else
                {

                    var supermaketShop = MergeEnAndZh(shopEn, zhResult, "supermarket");
                    var departmentStore = MergeEnAndZh(shopEn, zhResult);
                    AeonIGeoComList.Add(supermaketShop);
                    AeonIGeoComList.Add(departmentStore);
                }
            }
            return AeonIGeoComList;
        }

        public IGeoComGrabModel MergeEnAndZh(AeonModel shopEn, List<AeonModel> zhResult, string? type = "")
        {
            _logger.LogInformation("Merge Aeon En and Zh");
            var _rgxLat = Regexs.ExtractInfo(AeonModel.AeonLatRegex);
            var _rgxLng = Regexs.ExtractInfo(AeonModel.AeonLngRegex);
            IGeoComGrabModel AeonIGeoCom = new IGeoComGrabModel();
            if (type == "supermarket")
            {
                if(shopEn.Name.Contains("SUPERMARKET", comp))
                {
                    AeonIGeoCom.EnglishName = shopEn.Name;
                }
                else
                {
                    AeonIGeoCom.EnglishName = $"{shopEn.Name} Supermarket";
                }
                AeonIGeoCom.Class = "CMF";
                AeonIGeoCom.Type = "SMK";
                AeonIGeoCom.ShopId = "smk5";
            }
            else
            {
                AeonIGeoCom.EnglishName = shopEn.Name;
                AeonIGeoCom.Class = "CMF";
                AeonIGeoCom.Type = "ROI";
                AeonIGeoCom.ShopId = "roi1";
            }
            AeonIGeoCom.E_Address = shopEn.Address;
            AeonIGeoCom.GrabId = $"Aeon_{shopEn.Id}";
            AeonIGeoCom.Tel_No = shopEn.Phone!.Replace("Tel No.", "").Replace(" ", "");
            AeonIGeoCom.Web_Site = _options.Value.BaseUrl;
            var matchLat = _rgxLat.Matches(shopEn.LatLng!);
            if (matchLat.Count > 0 && matchLat != null)
            {
                AeonIGeoCom.Latitude = Convert.ToDouble(matchLat[0].Value.Replace("LatLng(", "").Replace(",", ""));
            }
            var matchLng = _rgxLng.Matches(shopEn.LatLng!);
            if (matchLng.Count > 0 && matchLng != null)
            {
                AeonIGeoCom.Longitude = Convert.ToDouble(matchLng[0].Value.Replace(", ", "").Replace(")", ""));
            }
            foreach (var shopZh in zhResult)
            {
                if (shopEn.Id == shopZh.Id)
                {
                    if (type == "supermarket")
                    {
                        if (shopEn.Name.Contains("SUPERMARKET", comp))
                        {
                            AeonIGeoCom.ChineseName = shopZh.Name;
                        }
                        else
                        {
                            AeonIGeoCom.ChineseName = $"{shopZh.Name}超級市場";
                        }
                    }
                    else
                    {
                        AeonIGeoCom.ChineseName = shopZh.Name;
                    }
                    AeonIGeoCom.C_Address = shopZh.Address;
                }
            }
            return AeonIGeoCom;
        }
    }
}
