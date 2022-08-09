using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;

namespace iGeoComAPI.Services
{
    public class  CorrectionalInstitutionGrabber : AbstractGrabber
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        private IOptions< CorrectionalInstitutionOptions> _options;
        private ILogger< CorrectionalInstitutionGrabber> _logger;
        private readonly IMemoryCache _memoryCache;

        private readonly string infoCode = @"() =>{" +
            @"const selectors = Array.from(document.querySelectorAll('.table-bordered')[2].querySelectorAll('tbody > tr')).slice(1);" +
            @"return selectors.map(v =>{return {name: v.querySelectorAll('td')[1]? v.querySelectorAll('td')[1].textContent:''," +
            @"number: v.querySelectorAll('td')[2]? v.querySelectorAll('td')[2].textContent:''," +
            @"address: v.querySelectorAll('td')[3]? v.querySelectorAll('td')[3].textContent:''" +
            @"}});" +
            @"}";
        private string waitSelector = ".table-bordered";


        public  CorrectionalInstitutionGrabber(PuppeteerConnection puppeteerConnection, IOptions< CorrectionalInstitutionOptions> options, IMemoryCache memoryCache, ILogger< CorrectionalInstitutionGrabber> logger,
            IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enResult = await _puppeteerConnection.PuppeteerGrabber<List< CorrectionalInstitutionModel>>(_options.Value.EnUrl, infoCode, waitSelector);
            var zhResult = await _puppeteerConnection.PuppeteerGrabber<List< CorrectionalInstitutionModel>>(_options.Value.ZhUrl, infoCode, waitSelector);
            var mergeResult = MergeEnAndZh(enResult, zhResult);
            var result = await this.GetShopInfo(mergeResult);
            return result;
        }

        public List<IGeoComGrabModel> MergeEnAndZh(List< CorrectionalInstitutionModel> enResult, List< CorrectionalInstitutionModel> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge  CorrectionalInstitution En and Zh");
                List<IGeoComGrabModel>  CorrectionalInstitutionIGeoComList = new List<IGeoComGrabModel>();
                foreach (var item in enResult.Select((value, i) => new { i, value }))
                {
                    var shopEn = item.value;
                    var index = item.i;
                    IGeoComGrabModel  CorrectionalInstitutionIGeoCom = new IGeoComGrabModel();
                    CorrectionalInstitutionIGeoCom.EnglishName = shopEn.name;
                    CorrectionalInstitutionIGeoCom.E_Address = shopEn.address;
                    CorrectionalInstitutionIGeoCom.Tel_No = shopEn.number;
                    foreach (var item2 in zhResult.Select((value2, i2) => new { i2, value2 }))
                    {
                        var shopZh = item2.value2;
                        var index2 = item2.i2;
                        if (shopEn.number == shopZh.number)
                        {
                            CorrectionalInstitutionIGeoCom.ChineseName = shopZh.name;
                            CorrectionalInstitutionIGeoCom.C_Address = shopZh.address;
                            break;
                        }

                    }
                     CorrectionalInstitutionIGeoComList.Add( CorrectionalInstitutionIGeoCom);
                }
                return  CorrectionalInstitutionIGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge  CorrectionalInstitution En and Zh");
                throw;
            }
        }

        public List<IGeoComGrabModel> ExtractClinic(List<IGeoComGrabModel> mergeList)
        {
            var newList = mergeList;
            foreach(var item in mergeList)
            {
                if(item.E_Address.Contains("(") && item.E_Address.Contains("(") && item.C_Address.Contains("(") && item.C_Address.Contains("("))
                {
                    var addressList = item.E_Address.Split(")").ToList();
                    if (item.EnglishName.Contains("/"))
                    {

                    }
                    foreach(var address in addressList)
                    {
                        var newItem = new IGeoComGrabModel();
                    }
                }else if (item.E_Address.Contains("Clinic:") && item.E_Address.Contains("Day hospital:"))
                {

                }
            }
            return new List<IGeoComGrabModel>();
        }
    }
}
