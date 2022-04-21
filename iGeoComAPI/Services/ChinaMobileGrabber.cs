using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class ChinaMobileGrabber:AbstractGrabber
    {
        private PuppeteerConnection _puppeteerConnection;
        private IOptions<ChinaMobileOptions> _options;
        private ILogger<ChinaMobileGrabber> _logger;
        private string idCode = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll('.toggle-list-content '));
                                 const hkList = Array.from(selectors[0].querySelectorAll('.map-icon'));
                                 const klnList = Array.from(selectors[1].querySelectorAll('.map-icon'));
                                 const ntList = Array.from(selectors[2].querySelectorAll('.map-icon'));
                                 const hkArray = hkList.map(v =>{return {Region: 'HK', Id: v.querySelector('a').getAttribute('href')}});
                                 const klnArray = klnList.map(v =>{return {Region: 'KLN', Id: v.querySelector('a').getAttribute('href')}});
                                 const ntArray = ntList.map(v =>{return {Region: 'NT', Id: v.querySelector('a').getAttribute('href')}});
                                 return hkArray.concat(klnArray, ntArray);
                                 }";
        private string infoCode = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll('script'));
                                 return selectors[1].textContent.trim();
                                 }";
        private string waitSelectorId = ".innerpage-content";
        private string waitSelectorInfo = "head";

        public ChinaMobileGrabber(PuppeteerConnection puppeteerConnection, IOptions<ChinaMobileOptions> options, ILogger<ChinaMobileGrabber> logger,
            IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json) : base(httpClient, absOptions, json)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _logger = logger;
        }

        public async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            var enIdLink = await _puppeteerConnection.PuppeteerGrabber<ChinaMobileModel[]>(_options.Value.EnUrl, idCode, waitSelectorId);
            var zhIdLink = await _puppeteerConnection.PuppeteerGrabber<ChinaMobileModel[]>(_options.Value.ZhUrl, idCode, waitSelectorId);
            var enResult = await grabResultByID(enIdLink);
            var zhResult = await grabResultByID(zhIdLink);
            var result = await MergeEnAndZh(enResult, zhResult);
            return result;
        }

        public async Task<List<ChinaMobileModel>?> grabResultByID(ChinaMobileModel[] idResult)
        {
            var _linkRgx = Regexs.ExtractInfo(ChinaMobileModel.ExtractLink);
            var _addressAndHourRgx = Regexs.ExtractInfo(ChinaMobileModel.ExtractAddressAndOpeningHour);
            var _latLngRgx = Regexs.ExtractInfo(ChinaMobileModel.ExtractLatLng);
            var _IdRgx = Regexs.ExtractInfo(ChinaMobileModel.ExtractId);
            List<ChinaMobileModel> ChinaMobileList = new List<ChinaMobileModel>();
            foreach (ChinaMobileModel id in idResult)
            {
                ChinaMobileModel ChinaMobile = new ChinaMobileModel();
                var extractLink = _linkRgx.Match(id.Id!).Groups[1].Value;
                var link = $"{_options.Value.BaseUrl}{extractLink}";
                var shopScript = await _puppeteerConnection.PuppeteerGrabber<string>(link, infoCode, waitSelectorInfo);
                var trimed = Regexs.TrimAllAndAdjustSpace(shopScript).Replace("\n", "").Replace("\t", "");
                ChinaMobile.Address = _addressAndHourRgx.Match(trimed).Groups[1].Value;
                ChinaMobile.LatLng = _latLngRgx.Match(trimed).Groups[1].Value;
                ChinaMobile.Id = _IdRgx.Match(link).Groups[1].Value;
                ChinaMobile.Region = id.Region;
                ChinaMobileList.Add(ChinaMobile);
            }
            return ChinaMobileList;
        }

        public async Task<List<IGeoComGrabModel>> MergeEnAndZh(List<ChinaMobileModel>? enResult, List<ChinaMobileModel>? zhResult)
        {
            try
            {
                _logger.LogInformation("Start merging ChinaMobile eng and Zh");
                var _LatLngrgx = Regexs.ExtractInfo(ChinaMobileModel.RegLatLngRegex);
                List<IGeoComGrabModel> ChinaMobileIGeoComList = new List<IGeoComGrabModel>();
                if (enResult != null && zhResult != null)
                {
                    foreach (ChinaMobileModel shopEn in enResult)
                    {
                        IGeoComGrabModel ChinaMobileIGeoCom = new IGeoComGrabModel();
                        ChinaMobileIGeoCom.E_Address = shopEn.Address;
                        ChinaMobileIGeoCom.E_Region = shopEn.Region;
                        var matchesEn = _LatLngrgx.Matches(shopEn.LatLng!);
                        ChinaMobileIGeoCom.Latitude = Convert.ToDouble(matchesEn[0].Value);
                        ChinaMobileIGeoCom.Longitude = Convert.ToDouble(matchesEn[2].Value);
                        NorthEastModel eastNorth = await this.getNorthEastNorth(ChinaMobileIGeoCom.Latitude, ChinaMobileIGeoCom.Longitude);
                        if (eastNorth != null)
                        {
                            ChinaMobileIGeoCom.Easting = eastNorth.hkE;
                            ChinaMobileIGeoCom.Northing = eastNorth.hkN;
                        }
                        ChinaMobileIGeoCom.GeoNameId = $"chinamobile_{shopEn.Id}";
                        foreach (ChinaMobileModel shopZh in zhResult)
                        {
                            if (shopEn.Id == shopZh.Id)
                            {
                                ChinaMobileIGeoCom.C_Address = shopZh.Address.Replace(" ", "");
                            }
                            continue;
                        }
                        ChinaMobileIGeoComList.Add(ChinaMobileIGeoCom);
                    }
                }
                return ChinaMobileIGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge ChinaMobile Eng and Zh RawData");
                throw;
            }
        }
    }

}
