﻿using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class WmoovGrabber
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        private readonly IOptions<WmoovOptions> _options;
        private readonly IMemoryCache _memoryCache;


        private readonly string shopCode = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll('.section_cinema_list > .list > ul > li' ));
                                 return selectors.map(v=>{return {Name: v.querySelector('.gotomap').textContent.trim(),Latitude: v.querySelector('.gotomap').getAttribute('data-latitudes'),
                                 Longitude: v.querySelector('.gotomap').getAttribute('data-longitudes'), Website: v.querySelector('.gotomap').getAttribute('href')
                                 }})
                                 }";
        private readonly string infoCode = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll('.left > .info > dd' ));
                                  return selectors.length === 3 ?
                                 {Tel_no: selectors[0] == null ? '': selectors[0].textContent.trim(), Web_Site: selectors[1] == null ? '':selectors[1].querySelector('a').getAttribute('href'), C_Address: selectors[2] == null ? '':selectors[2].textContent.trim()}
                                 : {Web_Site: selectors[0] == null ? '':selectors[0].querySelector('a').getAttribute('href'), C_Address: selectors[1] == null ? '':selectors[1].textContent.trim()}
                                 }";

        private readonly string test2Code = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll('.section_cinema_list > .list' ));
                                 for (let i = 0; i < selectors.length; i++){ Array.from(selectors[i].querySelectorAll('ul > li')).
                                 map(v=>{return {Region: selectors[i].querySelector('h3').textContent.trim(),Name: v.querySelector('.gotomap').textContent.trim()}})} 
                                 }";



        public WmoovGrabber(PuppeteerConnection puppeteerConnection, IOptions<WmoovOptions> options, IMemoryCache memoryCache)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
        }

        public async Task<List<IGeoComGrabModel?>> GetWebSiteItems()
        {
            var shopResult = await _puppeteerConnection.PuppeteerGrabber<WmoovModel>(_options.Value.BaseUrl, shopCode);
            var shopList = shopResult.ToList();
            var _rgx = Regexs.ExtractWmoovId();
            List<IGeoComGrabModel> WmoovIGeoComList = new List<IGeoComGrabModel>();
            foreach (var shop in shopList)
            {
                var infoResult = await _puppeteerConnection.PuppeteerSignalGrabber<IGeoComGrabModel>(@$"https://wmoov.com{shop.Website}", infoCode);
        
                infoResult.ChineseName = shop.Name;
                infoResult.Latitude = shop.Latitude;
                infoResult.Longitude = shop.Longitude;
                infoResult.Class = "CUF";
                infoResult.Type = "TNC";
                var matchId = _rgx.Matches(shop.Website!);
                if (matchId.Count > 0 && matchId != null)
                {
                    infoResult.Grab_ID = $"wmoov_{matchId[0].Value}";
                }
                WmoovIGeoComList.Add(infoResult);
            }

            return WmoovIGeoComList;

        }

        public List<IGeoComGrabModel> FindAdded(List<IGeoComGrabModel> newData, List<IGeoComModel> previousData)
        {
            int newDataLength = newData.Count;
            int previousDataLength = previousData.Count;
            List<IGeoComGrabModel> AddedWellcomeIGeoComList = new List<IGeoComGrabModel>();

            for (int i = 0; i < newDataLength; i++)
            {
                int j;
                for (j = 0; j < previousDataLength; j++) 
                    if (newData[i].C_Address?.Replace(",", "").Replace(" ","") == previousData[j].C_Address?.Replace(",", "").Replace(" ", "") &&
                       newData[i].Tel_No?.Replace(" ", "").Replace("-", "") == previousData[j].Tel_No?.Replace(" ", "").Replace("-", "") &&
                       newData[i].Web_Site == previousData[j].Web_Site
                        )
                        break;
                    if (j == previousDataLength)
                    {
                        AddedWellcomeIGeoComList.Add(newData[i]);
                    }
            }
            return AddedWellcomeIGeoComList;
        }
    }
}