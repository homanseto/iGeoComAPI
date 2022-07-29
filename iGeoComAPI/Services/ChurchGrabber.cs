using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace iGeoComAPI.Services
{
    public class ChurchGrabber : AbstractGrabber
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        private IOptions<ChurchOptions> _options;
        private ILogger<ChurchGrabber> _logger;
        private readonly IMemoryCache _memoryCache;


        private readonly string totalPageCode = @"() =>{const totalPage = document.querySelector('.tb_pages > tbody > tr > td > .red16').textContent; return totalPage}";
        private readonly string infoCode1 = @"() =>{" +
            @"const listA = Array.from(document.querySelectorAll('.tb_line_a')); const listB = Array.from(document.querySelectorAll('.tb_line_b'));" +
            @"const mergeList = [...listA, ...listB];" +
            @"return mergeList.map(v => {return { href: v.querySelector('a').getAttribute('href'), name: v.querySelector('a').textContent}})" +
            @"}";
        private string waitSelector1 = ".tb_line_a";
        private readonly string infoCode2 = @"() =>{" +
            @"return { latitude:document.querySelector('.map_guide').getAttribute('data-lat'), longitude: document.querySelector('.map_guide').getAttribute('data-lng')," +
            @" detail: document.querySelector('.church_detail').textContent.trim()}" +
            @"}";
        private string waitSelector2 = ".map_guide";


        public ChurchGrabber(PuppeteerConnection puppeteerConnection, IOptions<ChurchOptions> options, IMemoryCache memoryCache, ILogger<ChurchGrabber> logger,
            IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var shopResults = new List<ChurchModel>();
            var hkCookie = new Dictionary<string, string>()
            {
                [_options.Value.cookieName] = _options.Value.hkCookie
            };
            var totalPage = await _puppeteerConnection.PuppeteerGrabber<string>(_options.Value.ZhUrl, totalPageCode, waitSelector1, hkCookie);
            int value = 0;
            int num = 0;
            if (int.TryParse(totalPage, out value))
            {
                num = (value / _options.Value.perPageNum);
                num++;
            }
            for (int i = 1; i <= 2; i++)
            {
                var pageResult = await _puppeteerConnection.PuppeteerGrabber<List<ChurchModel>>($"{_options.Value.ZhUrl}&p={i}", infoCode1, waitSelector1, hkCookie);
                shopResults.AddRange(pageResult);
            }
            foreach (var item in shopResults.Select((value, i) => new { i, value }))
            {
                var shop = item.value;
                var index = item.i;
                var shopInfo = await _puppeteerConnection.PuppeteerGrabber<ChurchModel>($"{_options.Value.BaseUrl}{shop.href}", infoCode2, waitSelector2);
                shopResults[index].detail = shopInfo.detail;
                shopResults[index].latitude = shopInfo.latitude;
                shopResults[index].longitude = shopInfo.longitude;
                Console.WriteLine(shopResults[index]);
            }
            return new List<IGeoComGrabModel>();
        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<ChurchModel> enResult, List<ChurchModel> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge Church En and Zh");
                List<IGeoComGrabModel> ChurchIGeoComList = new List<IGeoComGrabModel>();
                foreach (var item in enResult.Select((value, i) => new { i, value }))
                {
                    var shopEn = item.value;
                    var index = item.i;
                    IGeoComGrabModel ChurchIGeoCom = new IGeoComGrabModel();


                    ChurchIGeoComList.Add(ChurchIGeoCom);
                }
                return ChurchIGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge Church En and Zh");
                throw;
            }
        }

    }
}
