using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class WellcomeGrabber
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        private readonly Regexs _regexs;
        private readonly IOptions<WellcomeOptions> _options;
        private readonly string infoCode = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll('.table-responsive > .table-striped > tbody > tr'));
                                 return selectors.map(v => {return {Address: v.querySelector('.views-field-field-address').textContent.trim(), Name: v.querySelector(
                                 '.views-field-title > .store-title',).textContent, LatLng: v.querySelector('.views-field-title > .store-title').getAttribute('data-latlng'),
                                 Phone: v.querySelector('.views-field-field-store-telephone').textContent.trim()
                                   }});
                                 }";

        public WellcomeGrabber(PuppeteerConnection puppeteerConnection, Regexs regexs, IOptions<WellcomeOptions> options)
        {
            _puppeteerConnection = puppeteerConnection;
            _regexs = regexs;
            _options = options;
        }

        public async Task<List<IGeoComModel>?> GetWebSiteItems()
        {
           var enResult = await _puppeteerConnection.PuppeteerGrabber<WellcomeModel>(_options.Value.EnUrl, infoCode);
           var zhResult = await _puppeteerConnection.PuppeteerGrabber<WellcomeModel>(_options.Value.ZhUrl, infoCode);
           return MergeEnAndZh(enResult, zhResult);
        }

        public List<IGeoComModel> MergeEnAndZh(WellcomeModel[] enResult, WellcomeModel[] zhResult)
        {
            var _rgx = _regexs.ExtractLagLong();
            List<IGeoComModel> WellcomeIGeoComList = new List<IGeoComModel>();
            foreach (var shopEn in enResult)
            {
                IGeoComModel WellcomeIGeoCom = new IGeoComModel();
                WellcomeIGeoCom.E_Address = shopEn.Address;
                WellcomeIGeoCom.EnglishName = shopEn.Name;
                var matchesEn = _rgx.Matches(shopEn.LatLng!);
                WellcomeIGeoCom.Latitude = matchesEn[0].Value;
                WellcomeIGeoCom.Longitude = matchesEn[2].Value;
                WellcomeIGeoCom.Tel_No = shopEn.Phone;
                foreach (var shopZh in zhResult)
                {
                    var matchesZh = _rgx.Matches(shopZh.LatLng!);
                    if (matchesZh.Count > 0 && matchesZh != null)
                    {
                        if (WellcomeIGeoCom.Latitude == matchesZh[0].Value && WellcomeIGeoCom.Longitude == matchesZh[2].Value && WellcomeIGeoCom.Tel_No == shopEn.Phone)
                        {
                            WellcomeIGeoCom.C_Address = shopZh.Address;
                            WellcomeIGeoCom.ChineseName = shopZh.Name;
                            continue;
                        }
                    }
                }
                WellcomeIGeoComList.Add(WellcomeIGeoCom);
            }
            return WellcomeIGeoComList;
        }


    }
}
