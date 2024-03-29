﻿using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class USelectGrabber:AbstractGrabber
    {
        private ConnectClient _httpClient;
        private JsonFunction _json;
        private IOptions<USelectOptions> _options;
        private IMemoryCache _memoryCache;
        private ILogger<USelectGrabber> _logger;

        public USelectGrabber(ConnectClient httpClient, JsonFunction json, IOptions<USelectOptions> options, IMemoryCache memoryCache, ILogger<USelectGrabber> logger, IOptions<NorthEastOptions> absOptions) : base(httpClient, absOptions, json)
        {
            _httpClient = httpClient;
            _json = json;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            _logger.LogInformation("start grabbing Vango rowdata");
            var selectQuery = new Dictionary<string, string>()
            {
                ["regionID"] = _options.Value.select.ToString()
            };
            var selectFoodQuery = new Dictionary<string, string>()
            {
                ["regionID"] = _options.Value.selectFood.ToString()
            };
            var selectMiniQuery = new Dictionary<string, string>()
            {
                ["regionID"] = _options.Value.selectMini.ToString()
            };
            var selectConnectHttp = await _httpClient.GetAsync(_options.Value.Url, selectQuery);
            var selectFoodConnectHttp = await _httpClient.GetAsync(_options.Value.Url, selectFoodQuery);
            var selectMiniConnectHttp = await _httpClient.GetAsync(_options.Value.Url, selectMiniQuery);
            var selectResult = _json.Dserialize<List<USelectModel>>(selectConnectHttp);
            var selectFoodResult = _json.Dserialize<List<USelectModel>>(selectFoodConnectHttp);
            var selectMiniResult = _json.Dserialize<List<USelectModel>>(selectMiniConnectHttp);
            var parsingSelectResult = await Parsing(selectResult);
            var parsingSelectFoodResult = await Parsing(selectFoodResult);
            var parsingSelectMiniResult = await Parsing(selectMiniResult);
            List<IGeoComGrabModel> USelectResult = parsingSelectResult.Concat(parsingSelectFoodResult).Concat(parsingSelectMiniResult).ToList();
            return USelectResult;
            // _memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
        }

        public async  Task<List<IGeoComGrabModel>> Parsing(List<USelectModel>? grabResult)
        {
            try
            {
                List<IGeoComGrabModel> USelectIGeoComList = new List<IGeoComGrabModel>();
                if (grabResult != null)
                {
                    foreach (var shop in grabResult)
                    {
                        IGeoComGrabModel USelectIGeoCom = new IGeoComGrabModel();
                        USelectIGeoCom.ChineseName = $"{shop.store_number}-{shop.storename}";
                        USelectIGeoCom.EnglishName = $"{shop.store_number}-{shop.storename_en}";
                        USelectIGeoCom.C_Address = shop.address_description.Replace(" ", "");
                        var cFloor = Regexs.ExtractC_Floor().Matches(USelectIGeoCom.C_Address);
                        if (cFloor.Count > 0 && cFloor != null)
                        {
                            USelectIGeoCom.C_floor = cFloor[0].Value;
                        }
                        USelectIGeoCom.E_Address = shop.address_description_en;
                        USelectIGeoCom.Latitude = Convert.ToDouble(shop.address_geo_lat);
                        USelectIGeoCom.Longitude = Convert.ToDouble(shop.address_geo_lng);
                        NorthEastModel eastNorth = await this.getNorthEastNorth(USelectIGeoCom.Latitude, USelectIGeoCom.Longitude);
                        if (eastNorth != null)
                        {
                            USelectIGeoCom.Easting = eastNorth.hkE;
                            USelectIGeoCom.Northing = eastNorth.hkN;
                        }
                        USelectIGeoCom.Class = "CMF";
                        USelectIGeoCom.Type = "SMK";
                        USelectIGeoCom.Source = "27";
                        USelectIGeoCom.Web_Site = _options.Value.BaseUrl;
                        USelectIGeoCom.Grab_ID = $"{shop.store_number}_{shop.storename}{shop.address_geo_lat}";
                        USelectIGeoCom.Tel_No = $"{shop.telephone} {shop.telephone2} {shop.telephone3}";
                        USelectIGeoComList.Add(USelectIGeoCom);
                    }
                }
                return USelectIGeoComList;
            }catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            
        }
    }
}
