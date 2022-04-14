﻿using iGeoComAPI.Models;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;
using iGeoComAPI.Options;
using Microsoft.Extensions.Caching.Memory;

namespace iGeoComAPI.Services
{
    public class SevenElevenGrabber : IGrabberAPI<SevenElevenModel>
    {
        //private readonly HttpClient _httpcClient;
        //private readonly IOptions<SevenElevenOptions> _options;
        private readonly ConnectClient _httpClient;
        private readonly JsonFunction _json;
        private readonly IOptions<SevenElevenOptions> _options;
        private readonly IMemoryCache _memoryCache;
        private readonly MyLogger _logger;

        /*
        public SevenElevenGrabber(HttpClient client, IOptions<SevenElevenOptions> options)
        {
            _client = client;
            _options = options;
        }
        */

        public SevenElevenGrabber(ConnectClient httpClient, JsonFunction json, IOptions<SevenElevenOptions> options, IMemoryCache memoryCache, MyLogger logger)
        {
            _httpClient = httpClient;
            _json = json;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        //HttpClient _HttpClient = new HttpClient();
        public async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
            try
            {
                _logger.LogStartGrabbing(nameof(SevenElevenGrabber));
                var enConnectHttp = await _httpClient.GetAsync(_options.Value.EnUrl);
                var enSerializedResult =  _json.Dserialize<List<SevenElevenModel>>(enConnectHttp);
                var zhConnectHttp = await _httpClient.GetAsync(_options.Value.ZhUrl);
                var zhSerializedResult = _json.Dserialize<List<SevenElevenModel>>(zhConnectHttp);
                var mergeResult = MergeEnAndZh(enSerializedResult, zhSerializedResult);
                // _memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
                return mergeResult;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public List<IGeoComGrabModel> MergeEnAndZh(List<SevenElevenModel>? enResult, List<SevenElevenModel>? zhResult)
        {
            var _rgx = Regexs.ExtractInfo(SevenElevenModel.RegLatLngRegex);
            List<IGeoComGrabModel> SevenElevenIGeoComList = new List<IGeoComGrabModel>();
            try
            {
                _logger.LogMergeEngAndZh(nameof(SevenElevenModel));
                if (enResult != null && zhResult != null)
                {
                    foreach (SevenElevenModel shopEn in enResult)
                    {
                        IGeoComGrabModel sevenElevenIGeoCom = new IGeoComGrabModel();
                        sevenElevenIGeoCom.E_Address = shopEn.Address;
                        if (shopEn.Region == "Kowloon")
                        {
                            sevenElevenIGeoCom.E_Region = "KLN";
                        }
                        else if (shopEn.Region == "New Territories")
                        {
                            sevenElevenIGeoCom.C_Region = "NT";
                        }
                        else
                        {
                            sevenElevenIGeoCom.C_Region = "HK";
                        }
                        sevenElevenIGeoCom.E_District = shopEn.District;
                        var matchesEn = _rgx.Matches(shopEn.LatLng!);
                        sevenElevenIGeoCom.Latitude = Convert.ToDouble(matchesEn[0].Value);
                        sevenElevenIGeoCom.Longitude = Convert.ToDouble(matchesEn[2].Value);
                        sevenElevenIGeoCom.Class = "CMF";
                        sevenElevenIGeoCom.Type = "CVS";
                        if (shopEn.Opening_24 == "1")
                        {
                            sevenElevenIGeoCom.Subcat = " ";
                        }
                        else
                        {
                            sevenElevenIGeoCom.Subcat = "NON24R";
                        }
                        sevenElevenIGeoCom.Grab_ID = $"seveneleven_{shopEn.Address?.Replace(" ", "").Replace("/", "").Replace(",", "").Replace(".", "")}";
                        sevenElevenIGeoCom.Web_Site = _options.Value.BaseUrl;

                        foreach (SevenElevenModel shopZh in zhResult)
                        {
                            var matchesZh = _rgx.Matches(shopZh.LatLng!);
                            if (matchesZh.Count > 0 && matchesZh != null)
                            {
                                if (matchesEn[0].Value == matchesZh[0].Value && matchesEn[2].Value == matchesZh[2].Value)
                                {
                                    sevenElevenIGeoCom.C_Address = shopZh.Address;
                                    if (shopZh.Region == "Kowloon")
                                    {
                                        sevenElevenIGeoCom.C_Region = "九龍";
                                    }
                                    else if (shopZh.Region == "New Territories")
                                    {
                                        sevenElevenIGeoCom.C_Region = "新界";
                                    }
                                    else
                                    {
                                        sevenElevenIGeoCom.C_Region = "香港";
                                    }
                                    sevenElevenIGeoCom.C_District = shopZh.District;
                                    continue;
                                }
                            }
                        }
                        SevenElevenIGeoComList.Add(sevenElevenIGeoCom);

                    }
                }
                return SevenElevenIGeoComList.Where(shop => shop.E_District != "Macau").ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        //public List<IGeoComGrabModel> FindAdded(List<IGeoComGrabModel> newData, List<IGeoComRepository> previousData)
        //{
        //    int newDataLength = newData.Count;
        //    int previousDataLength = previousData.Count;
        //    List<IGeoComGrabModel> AddedSevenElevenIGeoComList = new List<IGeoComGrabModel>();

        //    for (int i = 0; i < newDataLength; i++)
        //    {
        //        int j;
        //        for (j = 0; j < previousDataLength; j++)
        //            if (newData[i].E_Address?.Replace(",", "").Replace(" ", "") == previousData[j].E_Address?.Replace(",", "").Replace(" ", "") |
        //               newData[i].C_Address?.Replace(",", "").Replace(" ", "") == previousData[j].C_Address?.Replace(",", "").Replace(" ", "")
        //                )
        //                break;
        //        if (j == previousDataLength)
        //        {
        //            AddedSevenElevenIGeoComList.Add(newData[i]);
        //        }
        //    }
        //    return AddedSevenElevenIGeoComList;
        //}
    }
}
