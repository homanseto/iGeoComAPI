using iGeoComAPI.Models;
using System.Configuration;
using Microsoft.Extensions;
using Newtonsoft.Json;
using iGeoComAPI.Utilities;
using System.Text.RegularExpressions;

namespace iGeoComAPI.Services
{
    public partial class SevenElevenGrabber : IGrabberAPI
    {
        //Configuration? config = null;
        private readonly HttpClient _client;
        private Regex _rgx = Regexs.ExtractLagLong();
        private string _sql = "INSERT INTO igeocomtable VALUES (@GEONAMEID,@ENGLISHNAME,@CHINESENAME,@ClASS,@TYPE, @SUBCAT,@EASTING,@NORTHING,@SOURCE,@E_FLOOR,@C_FLOOR,@E_SITENAME,@C_SITENAME,@E_AREA,@C_AREA,@E_DISTRICT,@C_DISTRICT,@E_REGION,@C_REGION,@E_ADDRESS,@C_ADDRESS,@TEL_NO,@FAX_NO,@WEB_SITE,@REV_DATE,@GRAB_ID,@Latitude,@Longitude)";
        private DataAccess _dataAccess = new DataAccess();

        public SevenElevenGrabber(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<IGeoComModel>?> GetWebSiteItems()
        {
            try
            {
                var enConnectHttp = await new ConnectHttpClient(_client, "https://www.7-eleven.com.hk/en/api/store").SendAsync();
                var enSerializedResult = await new SerializeFunction(enConnectHttp).Diserialize<SevenElevenModel>();
                var zhConnectHttp = await new ConnectHttpClient(_client, "https://www.7-eleven.com.hk/zh/api/store").SendAsync();
                var zhSerializedResult = await new SerializeFunction(zhConnectHttp).Diserialize<SevenElevenModel>();
                var result = MergeEnAndZh(enSerializedResult, zhSerializedResult);

                return result;

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task SaveDataBase()
        {
            var result = await GetWebSiteItems();
            if (result != null)
            {
                _dataAccess.SaveGrabbedData(_sql, result, "Server=127.0.0.1;Port=3306;database=igeocom; user id=root; password=YouMeK100 ");
            }

        }

        public List<IGeoComModel> MergeEnAndZh(List<SevenElevenModel> enResult, List<SevenElevenModel> zhResult)
        {
            List<IGeoComModel> SevenElevenIGeoComList = new List<IGeoComModel>();
            if (enResult != null && zhResult != null)
            {
                foreach (SevenElevenModel shopEn in enResult)
                {
                    IGeoComModel sevenElevenIGeoCom = new IGeoComModel();
                    sevenElevenIGeoCom.E_Address = shopEn.Address;
                    sevenElevenIGeoCom.E_Region = shopEn.Region;
                    sevenElevenIGeoCom.E_District = shopEn.District;
                    var matchesEn = _rgx.Matches(shopEn.LatLng!);
                    sevenElevenIGeoCom.Latitude = matchesEn[0].Value;
                    sevenElevenIGeoCom.Longitude = matchesEn[2].Value;
                    if (shopEn.Opening_24 == "1")
                    {
                        sevenElevenIGeoCom.Subcat = "true";
                    }
                    else
                    {
                        sevenElevenIGeoCom.Subcat = "false";
                    }
                    sevenElevenIGeoCom.Grab_ID = $"sevenEleven{shopEn.LatLng}";

                    foreach (SevenElevenModel shopZh in zhResult)
                    {
                        var matchesZh = _rgx.Matches(shopZh.LatLng!);
                        if (matchesZh.Count > 0 && matchesZh != null)
                        {
                            if (sevenElevenIGeoCom.Latitude == matchesZh[0].Value && sevenElevenIGeoCom.Longitude == matchesZh[2].Value)
                            {
                                sevenElevenIGeoCom.C_Address = shopZh.Address;
                                sevenElevenIGeoCom.C_Region = shopZh.Region;
                                sevenElevenIGeoCom.C_District = shopZh.District;
                                continue;
                            }
                        }
                    }
                    SevenElevenIGeoComList.Add(sevenElevenIGeoCom);

                }
            }
            return SevenElevenIGeoComList;
        }
    }
}
