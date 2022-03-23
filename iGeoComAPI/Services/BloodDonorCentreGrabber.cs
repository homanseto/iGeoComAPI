using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace iGeoComAPI.Services
{
    public class BloodDonorCentreGrabber
    {
        private PuppeteerConnection _puppeteerConnection;
        private IOptions<BloodDonorCentreOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<BloodDonorCentreGrabber> _logger;
        private string infoCode = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll(' .pg_content > .page_content > .ppb_render_row > .inner_wrapper > script'));
                                 return selectors[1].textContent.trim();
                                 }";
        private string waitSelector = ".loaded";

        BloodDonorCentreModel bloodDonorCentreModel = new BloodDonorCentreModel();

        public BloodDonorCentreGrabber(PuppeteerConnection puppeteerConnection, IOptions<BloodDonorCentreOptions> options, IMemoryCache memoryCache, ILogger<BloodDonorCentreGrabber> logger)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<string?> GetWebSiteItems()
        {
            var enScript = await _puppeteerConnection.PuppeteerGrabber<string>(_options.Value.EnUrl, infoCode, waitSelector);
            var zhScript = await _puppeteerConnection.PuppeteerGrabber<string>(_options.Value.ZhUrl, infoCode, waitSelector);
            var extractEnResult = Extract1stLevelData(enScript);
            var extractZhResult = Extract1stLevelData(zhScript);
            return enScript;
        }

        public List<BloodDonorCentreModel> Extract1stLevelData(string info)
        {
            try
            {
                var _listRgx = Regexs.ExtractInfo(bloodDonorCentreModel.ExtraList);
                var splitResult = _listRgx.Match(Regexs.TrimAllAndAdjustSpace(info!)).Groups[1].Value;
                var resultList = splitResult.Split("'],").ToList();
                var _lagLngRgx = Regexs.ExtractInfo(bloodDonorCentreModel.RegLagLngRegex);
                List<BloodDonorCentreModel> BloodDonorCentreList = new List<BloodDonorCentreModel>();
                foreach (var item in resultList)
                {
                    BloodDonorCentreModel bloodDonorCentre = new BloodDonorCentreModel();
                    List<string> replacedResult = Regex.Replace(item, bloodDonorCentreModel.ReplaceExtraInfo, "/split/").Split("/split/").ToList();
                    if(ReferenceEquals != null)
                    {
                        bloodDonorCentre.Name = replacedResult[2];
                        bloodDonorCentre.Address = replacedResult[3];
                        bloodDonorCentre.LatLng = replacedResult[1];
                        /*
                        var matchesEn = _rgx.Matches(shopEn.LatLng!);
                        bloodDonorCentre.Latitude = matchesEn[0].Value;
                        bloodDonorCentre.Longitude = matchesEn[2].Value;
                        */
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
    }
}
