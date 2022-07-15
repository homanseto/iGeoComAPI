using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;

namespace iGeoComAPI.Services
{

    public class SinopecGrabber : AbstractGrabber
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        private IOptions<SinopecOptions> _options;
        private ILogger<SinopecGrabber> _logger;
        private readonly IMemoryCache _memoryCache;

        private readonly string infoCode1 = @"() =>{" +
            @"const selectors = Array.from(document.querySelectorAll('.shareBody > .gasStationlist > li'));" +
            @"return selectors.map(v =>{return {id: v.querySelector('.box > b > a').getAttribute('onclick'), Name: v.querySelector('.box > b > a').textContent.trim()," +
            @"Address: v.querySelectorAll('.box > .fl > p')[0].textContent.trim(), Phone: v.querySelectorAll('.box > .fl > p')[1].textContent.trim()" +
            @"}})" +
            @"}";
        private string waitSelector1 = ".shareBody";

        private readonly string infoCode2 = @"() =>{" +
            @"const selectors = Array.from(document.querySelectorAll('script'));" +
            @"return selectors[1].textContent.trim()" +
            @"}";
        private string waitSelector2 = "script";

        public SinopecGrabber(PuppeteerConnection puppeteerConnection, IOptions<SinopecOptions> options, IMemoryCache memoryCache, ILogger<SinopecGrabber> logger,
            IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            List<SinopecModel> enWebList = new List<SinopecModel>();
            List<SinopecModel> zhWebList = new List<SinopecModel>();
            List<string> pageList = new List<string>();
            pageList.Add(_options.Value.page1);
            pageList.Add(_options.Value.page2);
            pageList.Add(_options.Value.page3);
            pageList.Add(_options.Value.page4);
            pageList.Add(_options.Value.page5);

            var EnCookie = new Dictionary<string, string>()
            {
                [_options.Value.CookieName] = _options.Value.EnCookie
            };
            List<IGeoComGrabModel> temp = new List<IGeoComGrabModel>();

            //foreach (var item in pageList)
            //{
            //    var enResult = await _puppeteerConnection.PuppeteerGrabber<SinopecModel[]>($"{_options.Value.Url}{item}", infoCode1, waitSelector1, EnCookie);
            //    enWebList.AddRange(enResult);
            //}
            //foreach (var item in pageList)
            //{
            //    var zhResult = await _puppeteerConnection.PuppeteerGrabber<SinopecModel[]>($"{_options.Value.Url}{item}", infoCode1, waitSelector1);
            //    zhWebList.AddRange(zhResult);
            //}
            //foreach (var eShop in enWebList)
            //{
            //    var _rgx = Regexs.ExtractInfo(SinopecModel.ExtractUrl);
            //    string urlString = _rgx.Match(eShop.id).Groups[1].Value;
            //    var shopEnString = await _puppeteerConnection.PuppeteerGrabber<string>($"{_options.Value.Url}{urlString}", infoCode2, waitSelector2, EnCookie);
            //}
            string[] test = new string[] { "http://www.sinopechk.com/gasmap/print/16493.aspx", "http://www.sinopechk.com/gasmap/print/16549.aspx", "http://www.sinopechk.com/gasmap/print/16500.aspx" };
            foreach (string testItem in test)
            {
                var shopEnResult = await _puppeteerConnection.PuppeteerGrabber<string>(testItem, infoCode2, waitSelector2, EnCookie);
                string text = Regexs.TrimAllAndAdjustSpace(shopEnResult);
                var rgxlat = Regexs.ExtractInfo(SinopecModel.ExtractLat);
                var rgxlng = Regexs.ExtractInfo(SinopecModel.ExtractLng);
                string lat = rgxlat.Match(text).Groups[1].Value;
                string lng = rgxlng.Match(text).Groups[1].Value;
                Console.WriteLine(text);
            }
            return temp;
        }
    }

}
