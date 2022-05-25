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


        public async Task<List<IGeoComGrabModel>?> GetShopInfo(List<IGeoComGrabModel> shopList)
        {
            List<IGeoComGrabModel> resultList = new List<IGeoComGrabModel>();
            foreach (IGeoComGrabModel shop in shopList)
            {
                var northEast = await getNorthEast(shop.Latitude, shop.Longitude);
                var mapInfo = await MapInfoFunction(shop.Longitude, shop.Latitude);
                if (northEast != null)
                {
                    shop.Easting = northEast.hkE;
                    shop.Northing = northEast.hkN;
                }
                shop.C_floor = getCFloor(shop.C_Address);
                shop.E_floor = getEFloor(shop.E_Address);
                if(shop.Source == "")
                {
                    shop.Source = "27";
                }
                if (mapInfo != null)
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

        public string getCFloor(string cAddress = "")
        {
            string cFloor = "";
            var cFloorObjects = Regexs.ExtractC_Floor().Matches(cAddress.Replace(" ",""));
            if (cFloorObjects.Count > 0 && cFloorObjects != null)
            {
                cFloor = cFloorObjects[0].Value.ToString();
            }
            return cFloor;
        }

        public string getEFloor(string eAddress = "")
        {
            string eFloor = "";
            var eFloorObjects = Regexs.ExtractE_Floor().Matches(eAddress.Replace(" ", ""));
            if (eFloorObjects.Count > 0 && eFloorObjects != null)
            {
                eFloor = eFloorObjects[0].Value.ToString();
            }
            return eFloor;
        }

    }
}
