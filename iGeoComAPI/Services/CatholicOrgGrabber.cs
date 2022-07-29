using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;

namespace iGeoComAPI.Services
{
    public class CatholicOrgGrabber : AbstractGrabber
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        private IOptions<CatholicOrgOptions> _options;
        private ILogger<CatholicOrgGrabber> _logger;
        private readonly IMemoryCache _memoryCache;

        private readonly string infoCode = @"() =>{" +
            @"const selectors = Array.from(document.querySelectorAll('.menu-churches-hki-container > .mip-drop-nav> option'));" +
            @"return selectors.map(v =>{return {name: v.textContent, href: v.getAttribute('value')" +
            @"}})" +
            @"}";
        private string waitSelector = ".menu-churches-hki-container";


        public CatholicOrgGrabber(PuppeteerConnection puppeteerConnection, IOptions<CatholicOrgOptions> options, IMemoryCache memoryCache, ILogger<CatholicOrgGrabber> logger,
            IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enHrefResult = await _puppeteerConnection.PuppeteerGrabber<List<CatholicOrgModel>>(_options.Value.EnUrl, infoCode, waitSelector);
            var zhHrefResult = await _puppeteerConnection.PuppeteerGrabber<List<CatholicOrgModel>>(_options.Value.ZhUrl, infoCode, waitSelector);
            return new List<IGeoComGrabModel>();
        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<CatholicOrgModel> enResult, List<CatholicOrgModel> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge CatholicOrg En and Zh");
                List<IGeoComGrabModel> CatholicOrgIGeoComList = new List<IGeoComGrabModel>();
                foreach (var item in enResult.Select((value, i) => new { i, value }))
                {
                    var shopEn = item.value;
                    var index = item.i;
                    IGeoComGrabModel CatholicOrgIGeoCom = new IGeoComGrabModel();
                    foreach (var item2 in zhResult.Select((value2, i2) => new { i2, value2 }))
                    {
                        var shopZh = item2.value2;
                        var index2 = item2.i2;
                        if (shopEn.id == shopZh.id)
                        {
                            continue;
                        }

                    }
                    CatholicOrgIGeoComList.Add(CatholicOrgIGeoCom);
                }
                return CatholicOrgIGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge CatholicOrg En and Zh");
                throw;
            }
        }

    }
}
