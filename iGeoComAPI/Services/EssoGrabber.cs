using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using iGeoComAPI.Repository;

namespace iGeoComAPI.Services
{
    public class EssoGrabber: AbstractGrabber
    {
        private readonly ConnectClient _httpClient;
        private readonly JsonFunction _json;
        private readonly IOptions<EssoOptions> _options;
        private readonly MyLogger _logger;
        private readonly IGeoComGrabRepository _iGeoComGrabRepository;
        private readonly IDataAccess dataAccess;

        public EssoGrabber(ConnectClient httpClient, JsonFunction json, IOptions<EssoOptions> options, MyLogger logger, IOptions<NorthEastOptions> absOptions, IGeoComGrabRepository iGeoComGrabRepository, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _httpClient = httpClient;
            _json = json;
            _options = options;
            _logger = logger;
            _iGeoComGrabRepository = iGeoComGrabRepository;
        }

        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            try
            {
                var param = new Dictionary<string, string>()
                {
                    ["Latitude1"] = _options.Value.Latitude1,
                    ["Latitude2"] = _options.Value.Latitude2,
                    ["Longitude1"] = _options.Value.Longitude1,
                    ["Longitude2"] = _options.Value.Longitude2,
                    ["DataSource"] = _options.Value.DataSource,
                    ["Country"] = _options.Value.Country,
                    ["ResultLimit"] = _options.Value.ResultLimit
                };
                var enGrabResult = await _httpClient.GetAsync(_options.Value.EnUrl, param);
                var zhGrabResult = await _httpClient.GetAsync(_options.Value.ZhUrl, param);
                var dseEnResult = _json.Dserialize<EssoLocations>(enGrabResult);
                var dseZhResult = _json.Dserialize<EssoLocations>(zhGrabResult);
                var mergeresult = MergeEnAndZh(dseEnResult.locations, dseZhResult.locations);
                var result = await this.GetShopInfo(mergeresult);

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<EssoModel>? enResult, List<EssoModel>? zhResult)
        {
                _logger.LogMergeEngAndZh(nameof(EssoModel));
            List<IGeoComGrabModel> EssoIGeoComList = new List<IGeoComGrabModel>();
            if (enResult != null && zhResult != null)
                {
                //Parallel.ForEach(enResult, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, (shopEn) =>
                foreach (var item in enResult.Select((value, i) => new { i, value }))
                 {
                        var shopEn = item.value;
                        var index = item.i;
                        IGeoComGrabModel EssoIGeoCom = new IGeoComGrabModel();
                        EssoIGeoCom.E_Address = shopEn.AddressLine1;
                        EssoIGeoCom.Latitude = shopEn.Latitude;
                        EssoIGeoCom.Longitude = shopEn.Longitude;
                        EssoIGeoCom.Type = "PFS";
                        EssoIGeoCom.Class = "UTI";
                        EssoIGeoCom.ChineseName = $"{shopEn.brandName}-{shopEn.locationName}";
                        if (shopEn.Open24Hours == true)
                        {
                            EssoIGeoCom.Subcat = " ";
                        }
                        else
                        {
                            EssoIGeoCom.Subcat = "NON24R";
                        }

                        EssoIGeoCom.Web_Site = _options.Value.BaseUrl;
                    string phoneList = "";
                        foreach (string num in shopEn.Telephone)
                        {
                        phoneList += num;
                        }
                        EssoIGeoCom.Tel_No = phoneList;
                        EssoIGeoCom.GrabId = $"esso-{shopEn.locationID}";
                        foreach (EssoModel shopZh in zhResult)
                        {

                            if (shopEn.locationID == shopZh.locationID)
                            {
                                EssoIGeoCom.C_Address = shopZh.AddressLine1.Replace(" ", "");
                            EssoIGeoCom.EnglishName = $"{shopZh.brandName}-{shopZh.locationName}";

                            continue;
                            }
                        }
                    EssoIGeoComList.Add(EssoIGeoCom);
                }
                }
            return EssoIGeoComList;
        }
    }
}
