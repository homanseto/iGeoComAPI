﻿using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class WellcomeGrabber
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        private readonly IOptions<WellcomeOptions> _options;
        private readonly IMemoryCache _memoryCache;
        private readonly string infoCode = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll('.table-responsive > .table-striped > tbody > tr'));
                                 return selectors.map(v => {return {Address: v.querySelector('.views-field-field-address').textContent.trim(), Name: v.querySelector(
                                 '.views-field-title > .store-title',).textContent, LatLng: v.querySelector('.views-field-title > .store-title').getAttribute('data-latlng'),
                                 Phone: v.querySelector('.views-field-field-store-telephone').textContent.trim()
                                   }});
                                 }";

        public WellcomeGrabber(PuppeteerConnection puppeteerConnection, IOptions<WellcomeOptions> options, IMemoryCache memoryCache)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
        }

        public async Task<List<IGeoComModel>?> GetWebSiteItems()
        {
           var enResult = await _puppeteerConnection.PuppeteerGrabber<WellcomeModel>(_options.Value.EnUrl, infoCode);
           var zhResult = await _puppeteerConnection.PuppeteerGrabber<WellcomeModel>(_options.Value.ZhUrl, infoCode);
           var mergeResult = MergeEnAndZh(enResult, zhResult);
           //_memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
            return mergeResult;

        }

        public List<IGeoComModel> MergeEnAndZh(WellcomeModel[] enResult, WellcomeModel[] zhResult)
        {
            var _rgx = Regexs.ExtractLagLong();
            List<IGeoComModel> WellcomeIGeoComList = new List<IGeoComModel>();
            foreach (var shopEn in enResult)
            {
                IGeoComModel WellcomeIGeoCom = new IGeoComModel();
                WellcomeIGeoCom.E_Address = shopEn.Address?.Replace(",", "");
                WellcomeIGeoCom.EnglishName = $"Wellcome Supermarket-{shopEn.Name}";
                var matchesEn = _rgx.Matches(shopEn.LatLng!);
                WellcomeIGeoCom.Latitude = matchesEn[0].Value;
                WellcomeIGeoCom.Longitude = matchesEn[2].Value;
                WellcomeIGeoCom.Tel_No = shopEn.Phone;
                WellcomeIGeoCom.Web_Site = _options.Value.BaseUrl;
                WellcomeIGeoCom.Class = "CMF";
                WellcomeIGeoCom.Type = "SMK";
                WellcomeIGeoCom.Grab_ID = $"wellcome_{shopEn.LatLng}{shopEn.Phone}{shopEn.Name}".Replace(" ", "").Replace("|", "").Replace(".","");
                foreach (var shopZh in zhResult)
                {
                    var matchesZh = _rgx.Matches(shopZh.LatLng!);
                    if (matchesZh.Count > 0 && matchesZh != null)
                    {
                        if (WellcomeIGeoCom.Latitude == matchesZh[0].Value && WellcomeIGeoCom.Longitude == matchesZh[2].Value && WellcomeIGeoCom.Tel_No == shopZh.Phone)
                        {
                            WellcomeIGeoCom.C_Address = shopZh.Address?.Replace(",", "");
                            WellcomeIGeoCom.ChineseName = $"惠康超級市場-{shopZh.Name}";
                            continue;
                        }
                    }
                }
                WellcomeIGeoComList.Add(WellcomeIGeoCom);
            }
            return WellcomeIGeoComList;
        }

        /*
        public List<IGeoComModel> DeltaChange()
        {

        }
        */

    }
}
