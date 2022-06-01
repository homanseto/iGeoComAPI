using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Repository;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace iGeoComAPI.Services
{
    public class BloodDonorCentreGrabber: AbstractGrabber
    {
        private readonly ConnectClient _httpClient;
        private PuppeteerConnection _puppeteerConnection;
        private IOptions<BloodDonorCentreOptions> _options;
        private IMemoryCache _memoryCache;
        private readonly IGeoComGrabRepository _iGeoComGrabRepository;
        private readonly IDataAccess dataAccess;

        private ILogger<BloodDonorCentreGrabber> _logger;
        private string infoCode = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll(' .pg_content > .page_content > .ppb_render_row > .inner_wrapper > script'));
                                 return selectors[1].textContent.trim();
                                 }";
        private string waitSelector = ".loaded";

        public BloodDonorCentreGrabber(PuppeteerConnection puppeteerConnection, IOptions<BloodDonorCentreOptions> options, IMemoryCache memoryCache, ILogger<BloodDonorCentreGrabber> logger,
    IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json, IDataAccess dataAccess) : base(httpClient, absOptions, json, dataAccess)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public override async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enScript = await _puppeteerConnection.PuppeteerGrabber<string>(_options.Value.EnUrl, infoCode, waitSelector);
            var zhScript = await _puppeteerConnection.PuppeteerGrabber<string>(_options.Value.ZhUrl, infoCode, waitSelector);
            var extractEnResult = Extract1stLevelData(enScript);
            var extractZhResult = Extract1stLevelData(zhScript);
            var mergeResult = MergeEnAndZh(extractEnResult, extractZhResult);
            var result = await this.GetShopInfo(mergeResult);
            return result;
        }

        public List<BloodDonorCentreModel> Extract1stLevelData(string info)
        {
            try
            {
                var _listRgx = Regexs.ExtractInfo(BloodDonorCentreModel.ExtraList);
                var splitResult = _listRgx.Match(Regexs.TrimAllAndAdjustSpace(info!)).Groups[1].Value;
                var resultList = splitResult.Split("'],").ToList();
                var _lagLngRgx = Regexs.ExtractInfo(BloodDonorCentreModel.RegLagLngRegex);
                List<BloodDonorCentreModel> BloodDonorCentreList = new List<BloodDonorCentreModel>();
                foreach (var item in resultList)
                {
                    BloodDonorCentreModel bloodDonorCentre = new BloodDonorCentreModel();
                    List<string> replacedResult = Regex.Replace(item, BloodDonorCentreModel.ReplaceExtraInfo, "/split/").Split("/split/").ToList();
                    if(!String.IsNullOrEmpty(item))
                    {
                        bloodDonorCentre.Name = replacedResult[2];
                        bloodDonorCentre.Address = replacedResult[3];
                        bloodDonorCentre.LatLng = replacedResult[1];
                        
                        BloodDonorCentreList.Add(bloodDonorCentre);
                    }
                    else
                    {
                        continue;
                    }
                    
                }
                return BloodDonorCentreList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<BloodDonorCentreModel>? enResult, List<BloodDonorCentreModel>? zhResult)
        {
            List<IGeoComGrabModel> BloodDonorCentreIGeoComList = new List<IGeoComGrabModel>();
            var _lagLngRgx = Regexs.ExtractInfo(BloodDonorCentreModel.RegLagLngRegex);
            try
            {
                if (enResult != null && zhResult != null)
                {
                    //Parallel.ForEach(enResult, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, (shopEn) =>
                    foreach (var item in enResult.Select((value, i) => new { i, value }))
                    {
                        var shopEn = item.value;
                        var index = item.i;
                        IGeoComGrabModel BloodDonorCentreIGeoCom = new IGeoComGrabModel();
                        BloodDonorCentreIGeoCom.E_Address = shopEn.Address;
                        var matchesEn = _lagLngRgx.Matches(shopEn.LatLng!);
                        BloodDonorCentreIGeoCom.Latitude = Convert.ToDouble(matchesEn[0].Value);
                        BloodDonorCentreIGeoCom.Longitude = Convert.ToDouble(matchesEn[2].Value);
                        BloodDonorCentreIGeoCom.Type = "BDC";
                        BloodDonorCentreIGeoCom.Class = "HNC";
                        BloodDonorCentreIGeoCom.Shop = 12;
                        BloodDonorCentreIGeoCom.ChineseName = "香港紅十字會";
                        BloodDonorCentreIGeoCom.EnglishName = "Blood Transfusion";
                        BloodDonorCentreIGeoCom.Web_Site = _options.Value.BaseUrl;
                        BloodDonorCentreIGeoCom.GrabId = $"BloodTransfusion{BloodDonorCentreIGeoCom.Latitude}{BloodDonorCentreIGeoCom.Longitude}".Replace(".","").Trim();

                        foreach (BloodDonorCentreModel shopZh in zhResult)
                        {
                            
                            if (shopEn.LatLng == shopZh.LatLng)
                            {
                                BloodDonorCentreIGeoCom.C_Address = shopZh.Address.Replace(" ", "");

                                continue;
                            }
                        }
                        BloodDonorCentreIGeoComList.Add(BloodDonorCentreIGeoCom);
                    }
                }
                return BloodDonorCentreIGeoComList;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
