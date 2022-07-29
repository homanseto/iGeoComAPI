using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;

namespace iGeoComAPI.Services
{
    public class ConsularGrabber : AbstractGrabber
    {
        private ConnectClient _httpClient;
        private JsonFunction _json;
        private IOptions<ConsularOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<ConsularGrabber> _logger;
        private readonly IDataAccess dataAccess;

        public ConsularGrabber(ConnectClient httpClient, JsonFunction json, IOptions<ConsularOptions> options, IMemoryCache memoryCache, ILogger<ConsularGrabber> logger, IOptions<NorthEastOptions> absOptions, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _httpClient = httpClient;
            _json = json;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }


        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            _logger.LogInformation("start grabbing Consular rowdata");
            var consularList = new List<CountryInfo>();
            var countryList = await _httpClient.GetAsync(_options.Value.countriesUrl);
            var countrySerializedResult = _json.Dserialize<List<ConsularModel>>(countryList);
            if (countrySerializedResult != null && countrySerializedResult.Count > 0)
            {
                foreach (var country in countrySerializedResult)
                {
                    var consular = new CountryInfo();
                    var countryInfo = await _httpClient.GetAsync($"{_options.Value.countryUrl}{country.id}.json");
                    var serializedInfo = _json.Dserialize<CountryInfo>(countryInfo);
                    if (serializedInfo != null)
                    {
                        consular.Name_en = serializedInfo.Name_en;
                        consular.Name_tc = serializedInfo.Name_tc;
                        consular.Detail = serializedInfo.Detail;
                    }
                    consularList.Add(consular);
                }
            }
            var mergeResult = Parsing(consularList);
            var result = await this.GetShopInfo(mergeResult);
            return result;
        }

        public string ReplaceHtmlTag(string input)
        {
            return input.Replace("<p>", "").Replace("</p>", "").Replace("<br />", "");
        }

        public List<IGeoComGrabModel> Parsing(List<CountryInfo>? countries)
        {
            List<IGeoComGrabModel> ConsularIGeoComList = new List<IGeoComGrabModel>();
            try
            {
                _logger.LogInformation("Start merging Consular eng and Zh");
                if (countries != null)
                {
                    
                    foreach (var c in countries)
                    {
                        var ConsularIGeoCom = new IGeoComGrabModel();
                        ConsularIGeoCom.ChineseName = c.Name_tc;
                        ConsularIGeoCom.EnglishName = c.Name_en;
                        if(c.Detail != null)
                        {
                            ConsularIGeoCom.C_Address = ReplaceHtmlTag(c.Detail[0].address_tc);
                            ConsularIGeoCom.E_Address = ReplaceHtmlTag(c.Detail[0].address_en);
                            ConsularIGeoCom.Tel_No = ReplaceHtmlTag(c.Detail[0].telephone);
                            ConsularIGeoCom.Fax_No = ReplaceHtmlTag(c.Detail[0].fax);
                            ConsularIGeoCom.GrabId = $"Consular{c.Detail[0].id}";
                            ConsularIGeoCom.Class = "GOV";
                            ConsularIGeoCom.Type = "CST";
                        }
                        ConsularIGeoComList.Add(ConsularIGeoCom);
                    }
                }
                return ConsularIGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge Catlex Eng and Zh RawData");
                throw;
            }
        }
    }
}
