using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;

namespace iGeoComAPI.Services
{
    public class Store759Grabber : AbstractGrabber
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        private IOptions<Store759Options> _options;
        private ILogger<Store759Grabber> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly LatLngFunction _function;

        private readonly string infoCode = @"() =>{" +
            @"const hkList = Array.from(document.querySelector('#tabs-1').querySelectorAll('.clickable'));" +
            @"const klnList = Array.from(document.querySelector('#tabs-2').querySelectorAll('.clickable'));" +
            @"const ntList = Array.from(document.querySelector('#tabs-3').querySelectorAll('.clickable'));" +
            @"const hkListInfo = hkList.map(v =>{return {name: v.querySelector('td[width=""150""').textContent," +
            @"address: v.querySelector('td[width=""250""').textContent, number: v.querySelector('td[width=""80""').textContent," +
            @"id: v.getAttribute('data-href')}});" +
            @"const klnListInfo = klnList.map(v =>{return {name: v.querySelector('td[width=""150""').textContent," +
            @"address: v.querySelector('td[width=""250""').textContent, number: v.querySelector('td[width=""80""').textContent," +
            @"id: v.getAttribute('data-href')}});" +
            @"const ntListInfo = ntList.map(v =>{return {name: v.querySelector('td[width=""150""').textContent," +
            @"address: v.querySelector('td[width=""250""').textContent, number: v.querySelector('td[width=""80""').textContent," +
            @"id: v.getAttribute('data-href')}});" +
            @"const result = [...hkListInfo, ...klnListInfo, ...ntListInfo];"+
            @"return result;"+
            @"}";
        private string waitSelector = ".main";

        static Regex ExtractInfo(string input)
        {
            Regex reg = new Regex(input);

            return reg;
        }
        Regex rgxId = ExtractInfo(Store759Model.extractId);

        public Store759Grabber(PuppeteerConnection puppeteerConnection, IOptions<Store759Options> options, IMemoryCache memoryCache, ILogger<Store759Grabber> logger,
            IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess, LatLngFunction function) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _function = function;
            _logger = logger;
        }
        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enResult = await _puppeteerConnection.PuppeteerGrabber<List<Store759Model>>(_options.Value.EnUrl, infoCode, waitSelector);
            var zhResult = await _puppeteerConnection.PuppeteerGrabber<List<Store759Model>>(_options.Value.ZhUrl, infoCode, waitSelector);
            var mergeResult = MergeEnAndZh(enResult, zhResult);
            var latlngResult = await FindLatLng(mergeResult);
            var result = await this.GetShopInfo(latlngResult);
            return result;
        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<Store759Model> enResult, List<Store759Model> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge Store759 En and Zh");
                List<IGeoComGrabModel> Store759IGeoComList = new List<IGeoComGrabModel>();
                foreach (var item in enResult.Select((value, i) => new { i, value }))
                {
                    var shopEn = item.value;
                    var index = item.i;
                    IGeoComGrabModel Store759IGeoCom = new IGeoComGrabModel();
                    var regexEnId = rgxId.Match(shopEn.id);
                    if (regexEnId.Success)
                    {
                        if (!String.IsNullOrEmpty(regexEnId.Groups["id"].Value))
                        {
                            Store759IGeoCom.GrabId = $"Store759_{regexEnId.Groups["id"].Value}";
                        }
                    }
                    Store759IGeoCom.EnglishName = shopEn.name;
                    Store759IGeoCom.E_Address = shopEn.address;
                    Store759IGeoCom.Tel_No = shopEn.number;

                    foreach (var item2 in zhResult.Select((value2, i2) => new { i2, value2 }))
                    {
                        var shopZh = item2.value2;
                        var index2 = item2.i2;
                        if(shopEn.number.Replace(" ", "") == shopZh.number.Replace(" ", ""))
                        {
                            Store759IGeoCom.ChineseName = shopZh.name;
                            Store759IGeoCom.C_Address = shopZh.address;
                        }
                    }
                    Store759IGeoComList.Add(Store759IGeoCom);
                }
                return Store759IGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge Store759 En and Zh");
                throw;
            }
        }

        public async Task<List<IGeoComGrabModel>> FindLatLng(List<IGeoComGrabModel> input)
        {
            foreach (var inputItem in input)
            {
                var latlng = await _function.FindLatLngByAddress($"759阿信屋 { inputItem.C_Address}");
                inputItem.Latitude = latlng.Latitude;
                inputItem.Longitude = latlng.Longtitude;
            }
            return input;
        }

    }
}
