using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;

namespace iGeoComAPI.Services
{
    public class PetroChinaGrabber : AbstractGrabber
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        private IOptions<PetroChinaOptions> _options;
        private ILogger<PetroChinaGrabber> _logger;
        private readonly IMemoryCache _memoryCache;

        private readonly string infoCode = @"() =>{" +
            @"const selectors = Array.from(document.querySelectorAll('.station-table > table> tbody > tr'));" +
            @"return selectors.map(v =>{return {id: v.getAttribute('data-id'), Name: v.querySelectorAll('td')[0].textContent.trim()," +
            @"Address: v.querySelectorAll('td')[1].textContent.trim(), Number: v.querySelectorAll('td')[2].textContent.trim()," +
            @"Latitude: v.getAttribute('data-lat'), Longitude: v.getAttribute('data-lng')" +
            @"}})" +
            @"}";
        private string waitSelector = ".station-table";


        public PetroChinaGrabber(PuppeteerConnection puppeteerConnection, IOptions<PetroChinaOptions> options, IMemoryCache memoryCache, ILogger<PetroChinaGrabber> logger,
            IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enResult = await _puppeteerConnection.PuppeteerGrabber<List<PetroChinaModel>>(_options.Value.EnUrl, infoCode, waitSelector);
            var zhResult = await _puppeteerConnection.PuppeteerGrabber<List<PetroChinaModel>>(_options.Value.ZhUrl, infoCode, waitSelector);
            var mergeResult = MergeEnAndZh(enResult, zhResult);
            var result = await this.GetShopInfo(mergeResult);
            return result;
        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<PetroChinaModel> enResult, List<PetroChinaModel> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge PetroChina En and Zh");
                List<IGeoComGrabModel> PetroChinaIGeoComList = new List<IGeoComGrabModel>();
                foreach (var item in enResult.Select((value, i) => new { i, value }))
                {
                    var shopEn = item.value;
                    var index = item.i;
                    IGeoComGrabModel PetroChinaIGeoCom = new IGeoComGrabModel();
                    PetroChinaIGeoCom.E_Address = shopEn.Address!;
                    PetroChinaIGeoCom.EnglishName = $"PetroChina {shopEn.Name}";
                    PetroChinaIGeoCom.Latitude = shopEn.Latitude;
                    PetroChinaIGeoCom.Longitude = shopEn.Longitude;
                    PetroChinaIGeoCom.Tel_No = shopEn.Number;
                    PetroChinaIGeoCom.Web_Site = _options.Value.BaseUrl!;
                    PetroChinaIGeoCom.Class = "UTI";
                    PetroChinaIGeoCom.Type = "PFS";
                    PetroChinaIGeoCom.Shop = 14;
                    PetroChinaIGeoCom.GrabId = $"PetroChina_{shopEn.id}";
                    foreach (var item2 in zhResult.Select((value2, i2) => new { i2, value2 }))
                    {
                        var shopZh = item2.value2;
                        var index2 = item2.i2;
                        if (shopEn.id == shopZh.id)
                        {
                            PetroChinaIGeoCom.C_Address = shopZh.Address!.Replace(" ", "");
                            PetroChinaIGeoCom.ChineseName = $"中國石油-{shopZh.Name}";
                            break;
                        }

                    }
                    PetroChinaIGeoComList.Add(PetroChinaIGeoCom);
                }
                return PetroChinaIGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge PetroChina En and Zh");
                throw;
            }
        }

    }
}
