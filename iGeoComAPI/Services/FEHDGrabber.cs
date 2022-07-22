using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;

namespace iGeoComAPI.Services
{
    public class FEHDGrabber : AbstractGrabber
    {
        private ConnectClient _httpClient;
        private JsonFunction _json;
        private IOptions<FEHDOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<FEHDGrabber> _logger;
        private readonly IDataAccess dataAccess;

        public FEHDGrabber(ConnectClient httpClient, JsonFunction json, IOptions<FEHDOptions> options, IMemoryCache memoryCache, ILogger<FEHDGrabber> logger, IOptions<NorthEastOptions> absOptions, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _httpClient = httpClient;
            _json = json;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            List<FEHDModel> fehdList = new List<FEHDModel>();
            _logger.LogInformation("start grabbing FEHD rowdata");
            foreach (string item in _options.Value.type)
            {
                var query = new Dictionary<string, string>()
                {
                    ["type"] = item
                };
                var connectHttp = await _httpClient.GetAsync(_options.Value.getMapData, query);
                var cerializedResult = _json.Dserialize<List<FEHDModel>>(connectHttp);
                if (cerializedResult != null)
                {
                    cerializedResult.ForEach(v => v.type = item);
                    fehdList.AddRange(cerializedResult);
                }
            }
            var convertResult = ConvertIGeoCom(fehdList);
            var result = await this.GetShopInfo(convertResult);
            return result;
        }

        public List<IGeoComGrabModel> ConvertIGeoCom(List<FEHDModel> grabResult)
        {
            var _rgx = Regexs.ExtractInfo(FEHDModel.RegLatLng);
            List<IGeoComGrabModel> FEHDIGeoComList = new List<IGeoComGrabModel>();
            if (grabResult != null)
            {
                foreach (var item in grabResult)
                {
                    IGeoComGrabModel FEHDIGeoCom = new IGeoComGrabModel();
                    FEHDIGeoCom.GrabId = $"FEHD {item.mapID}";
                    FEHDIGeoCom.E_Address = item.addressEN;
                    FEHDIGeoCom.C_Address = item.addressTC;
                    var matchesEn = _rgx.Matches(item.latitude!);
                    if (matchesEn.Count > 0 && matchesEn != null)
                    {
                        FEHDIGeoCom.Latitude = Convert.ToDouble(matchesEn[0].Value);
                        FEHDIGeoCom.Longitude = Convert.ToDouble(matchesEn[2].Value);
                    }
                    FEHDIGeoCom.Web_Site = _options.Value.BaseUrl;
                    FEHDIGeoCom.EnglishName = item.nameEN;
                    FEHDIGeoCom.ChineseName = item.nameTC;
                    if (item.type == "market")
                    {
                        FEHDIGeoCom.Class = "MUF";
                        FEHDIGeoCom.Type = "MKT";
                    }
                    else if (item.type == "toilet")
                    {
                        FEHDIGeoCom.Class = "MUF";
                        FEHDIGeoCom.Type = "TOI";
                        if (item.openHourEN == "24 hours")
                        {
                            FEHDIGeoCom.Subcat = "";
                        }
                        else
                        {
                            FEHDIGeoCom.Subcat = "NON24R";
                        }

                    }
                    else if (item.type == "refuse")
                    {
                        FEHDIGeoCom.Class = "MUF";
                        FEHDIGeoCom.Type = "RCP";
                    }
                    else
                    {
                        FEHDIGeoCom.Class = "MUF";
                        FEHDIGeoCom.Type = "BAZ";
                    }

                    FEHDIGeoComList.Add(FEHDIGeoCom);
                }
            }
            return FEHDIGeoComList;
        }
    }
}