using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class AeonGrabber : AbstractGrabber
    {
        private PuppeteerConnection _puppeteerConnection;
        private IOptions<AeonOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<AeonGrabber> _logger;
        private string infoCode = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll('.framebottom > .framecenter > .shopdetail > .shop > .shoplist'));
                                 return selectors.map(v => {return {Name: v.querySelector('input').getAttribute('data-name'), Address: v.querySelector('input').getAttribute('data-address'),
                                 LatLng: v.querySelector('.shopname > a').getAttribute('onclick'), Id: v.querySelector('input').getAttribute('data-id').trim(), 
                                 Phone: v.querySelectorAll('div > ul >li')[2].textContent.trim()
                                 }});
                                 }";
        private string waitSelector = ".framebottom";

        public AeonGrabber(PuppeteerConnection puppeteerConnection, IOptions<AeonOptions> options, IMemoryCache memoryCache, ILogger<AeonGrabber> logger, 
            IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json) : base(httpClient, absOptions, json)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        public async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enResult = await _puppeteerConnection.PuppeteerGrabber<AeonModel[]>(_options.Value.EnUrl, infoCode, waitSelector);
            var zhResult = await _puppeteerConnection.PuppeteerGrabber<AeonModel[]>(_options.Value.ZhUrl, infoCode, waitSelector);
            var enResultList = enResult.ToList();
            var zhResultList = zhResult.ToList();
            var mergeResult = await MergeEnAndZh(enResultList, zhResultList);
            //_memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
            return mergeResult;

        }

        public async Task<List<IGeoComGrabModel>> MergeEnAndZh(List<AeonModel> enResult, List<AeonModel> zhResult)
        {
            _logger.LogInformation("Merge Aeon En and Zh");
            var _rgxLat = Regexs.ExtractInfo(AeonModel.AeonLatRegex);
            var _rgxLng = Regexs.ExtractInfo(AeonModel.AeonLngRegex);
            List<IGeoComGrabModel> AeonIGeoComList = new List<IGeoComGrabModel>();
            foreach (var shopEn in enResult)
            {
                IGeoComGrabModel AeonIGeoCom = new IGeoComGrabModel();
                AeonIGeoCom.EnglishName = shopEn.Name;
                AeonIGeoCom.E_Address = shopEn.Address;
                AeonIGeoCom.Grab_ID = $"Aeon_{shopEn.Id}";
                AeonIGeoCom.Tel_No = shopEn.Phone!.Replace("Tel No.","").Replace(" ","");
                var matchLat = _rgxLat.Matches(shopEn.LatLng!);
                if (matchLat.Count > 0 && matchLat != null)
                {
                    AeonIGeoCom.Latitude = Convert.ToDouble(matchLat[0].Value.Replace("LatLng(","").Replace(",", ""));
                }
                var matchLng = _rgxLng.Matches(shopEn.LatLng!);
                if (matchLng.Count > 0 && matchLng != null)
                {
                    AeonIGeoCom.Longitude = Convert.ToDouble(matchLng[0].Value.Replace(", ","").Replace(")",""));
                }
                NorthEastModel eastNorth = await this.getNorthEastNorth(AeonIGeoCom.Latitude, AeonIGeoCom.Longitude);
                if (eastNorth != null)
                {
                    AeonIGeoCom.Easting = eastNorth.hkE;
                    AeonIGeoCom.Northing = eastNorth.hkN;
                }
                foreach (var shopZh in zhResult)
                {
                    if (shopEn.Id == shopZh.Id)
                    {
                        AeonIGeoCom.ChineseName = shopZh.Name;
                        AeonIGeoCom.C_Address = shopZh.Address;
                    }
                }
                AeonIGeoComList.Add(AeonIGeoCom);
            }
            return AeonIGeoComList;
        }
    }
}
