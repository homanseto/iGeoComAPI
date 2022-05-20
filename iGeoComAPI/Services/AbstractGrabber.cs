using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public abstract class AbstractGrabber
    {
        private readonly ConnectClient httpClient;
        private IOptions<NorthEastOptions> absOptions;
        private JsonFunction json;
        private readonly IDataAccess dataAccess;
        public AbstractGrabber(ConnectClient httpClient, IOptions<NorthEastOptions> absOptions, JsonFunction json, IDataAccess dataAccess)
        {
            this.httpClient = httpClient;
            this.absOptions = absOptions;
            this.json = json;
            this.dataAccess = dataAccess;
        }


        public abstract Task<List<IGeoComGrabModel>?> GetWebSiteItems();

        public virtual List<IGeoComGrabModel> MergeResult(List<SevenElevenModel> enInput, List<SevenElevenModel> zhInput)
        {
            List<IGeoComGrabModel> resultList = new List<IGeoComGrabModel>();
            return resultList;
        }


        public async Task<List<IGeoComGrabModel>?> GetNorthEastAndMapInfo(List<IGeoComGrabModel> shopList)
        {
            List<IGeoComGrabModel> resultList = new List<IGeoComGrabModel>();
            foreach (IGeoComGrabModel shop in shopList)
            {
                var northEast = await getNorthEast(shop.Latitude, shop.Longitude);
                var mapInfo = await MapInfoFunction(shop.Longitude, shop.Latitude);
                if(northEast != null)
                {
                    shop.Easting = northEast.hkE;
                    shop.Northing = northEast.hkN;
                }
                if(mapInfo != null)
                {
                    shop.E_area = mapInfo.Area2_Enam;
                    shop.C_area = mapInfo.Area2_Cnam;
                    shop.C_District = mapInfo.ChineseName;
                    shop.E_District = mapInfo.EnglishName;
                    shop.E_Region = mapInfo.E_Region;
                    shop.C_Region = mapInfo.C_Region;
                }
                resultList.Add(shop);
            }
            return resultList;
        }


        public async Task<NorthEastModel?> getNorthEast(double lat, double lng)
        {
                var query = new Dictionary<string, string>()
                {
                    ["inSys"] = this.absOptions.Value.InSys,
                    ["iutSys"] = this.absOptions.Value.IutSys,
                    ["lat"] = lat.ToString(),
                    ["long"] = lng.ToString()
                };

                var result = await this.httpClient.GetAsync(this.absOptions.Value.ConvertNE, query);
                return this.json.Dserialize<NorthEastModel>(result);
        }

        public async Task<HKMapInfo> MapInfoFunction(double lng, double lat)

        {
            string query = $"MAP_FUNCTION {lng}, {lat}";
            var result = await this.dataAccess.LoadSingleData<HKMapInfo>(query);
            return result;
        }

        //public async Task<bool> Execute()
        //{
        //    var step1Result = MergeEnAndZh();
        //    step1Result for->
        //}

        //public abstract async Task<List<IGeoComGrabModel>> MergeResult(List<T>? enResult, List<T>? zhResult = null)
        //{

        //}
        //public async Task<Ma>
    }
}
