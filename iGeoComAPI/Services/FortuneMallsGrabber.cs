using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;

namespace iGeoComAPI.Services
{
    public class FortuneMallsGrabber : AbstractGrabber
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        private IOptions<FortuneMallsOptions> _options;
        private ILogger<FortuneMallsGrabber> _logger;
        private readonly IMemoryCache _memoryCache;

        private readonly string infoCode1 = @"() =>{" +
            @"const selectors = Array.from(document.querySelectorAll('.common-menu-list > li'));" +
            @"return selectors.map(v =>{return {href: v.getAttribute('onClick') }})" +
            @"}";
        private string waitSelector1 = ".common-menu-list";

        private readonly string iframeInfoCode1 = @"#parking-content > div > iframe";
        private readonly string iframeWaitSelector = @".google-maps-link ";
        private readonly string iframeInfoCode2 = @"()=>{return document.querySelector('.google-maps-link > a')? document.querySelector('.google-maps-link > a').getAttribute('href'):'' }";
        private readonly string iframeChick = @"#parking";

        static Regex ExtractInfo(string input)
        {
            Regex reg = new Regex(input);
            return reg;
        }


        public FortuneMallsGrabber(PuppeteerConnection puppeteerConnection, IOptions<FortuneMallsOptions> options, IMemoryCache memoryCache, ILogger<FortuneMallsGrabber> logger,
            IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var hrefEnResult = await _puppeteerConnection.PuppeteerGrabber<List<FortuneMallsModel>>(_options.Value.EnUrl, infoCode1, waitSelector1);
            var hrefZhResult = await _puppeteerConnection.PuppeteerGrabber<List<FortuneMallsModel>>(_options.Value.ZhUrl, infoCode1, waitSelector1);
            var rgxId = ExtractInfo(FortuneMallsModel.extractId);
            foreach (var en in hrefEnResult)
            {
                var shop = en;
                var extractId = rgxId.Match(en.href);
                if (extractId.Success)
                {
                    if (!String.IsNullOrEmpty(extractId.Groups["id"].Value))
                    {
                        shop.id = extractId.Groups["id"].Value;
                    }
                }
                var iframe = await _puppeteerConnection.GetIframaContent($"{_options.Value.EnUrl}{shop.id}", iframeInfoCode1, iframeWaitSelector,iframeInfoCode2, iframeChick);
                Console.WriteLine(iframe);
            }

            return new List<IGeoComGrabModel>();
        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<FortuneMallsModel> enResult, List<FortuneMallsModel> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge FortuneMalls En and Zh");
                List<IGeoComGrabModel> FortuneMallsGeoComList = new List<IGeoComGrabModel>();
                foreach (var item in enResult.Select((value, i) => new { i, value }))
                {
                    var shopEn = item.value;
                    var index = item.i;
                    IGeoComGrabModel FortuneMallsGeoCom = new IGeoComGrabModel();
                    foreach (var item2 in zhResult.Select((value2, i2) => new { i2, value2 }))
                    {
                        var shopZh = item2.value2;
                        var index2 = item2.i2;

                    }
                    FortuneMallsGeoComList.Add(FortuneMallsGeoCom);
                }
                return FortuneMallsGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge FortuneMalls En and Zh");
                throw;
            }
        }

    }
}
