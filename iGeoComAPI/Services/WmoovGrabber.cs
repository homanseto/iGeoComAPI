using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class WmoovGrabber:AbstractGrabber
    {
        private PuppeteerConnection _puppeteerConnection;
        private IOptions<WmoovOptions> _options;
        private IMemoryCache _memoryCache;


        private string shopCode = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll('.section_cinema_list > .list > ul > li' ));
                                 return selectors.map(v=>{return {Name: v.querySelector('.gotomap').textContent.trim(),Latitude: v.querySelector('.gotomap').getAttribute('data-latitudes'),
                                 Longitude: v.querySelector('.gotomap').getAttribute('data-longitudes'), Website: v.querySelector('.gotomap').getAttribute('href')
                                 }})
                                 }";
        private string infoCode = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll('.left > .info > dd' ));
                                  return selectors.length === 3 ?
                                 {Tel_no: selectors[0] == null ? '': selectors[0].textContent.trim(), Web_Site: selectors[1] == null ? '':selectors[1].querySelector('a').getAttribute('href'), C_Address: selectors[2] == null ? '':selectors[2].textContent.trim()}
                                 : {Web_Site: selectors[0] == null ? '':selectors[0].querySelector('a').getAttribute('href'), C_Address: selectors[1] == null ? '':selectors[1].textContent.trim()}
                                 }";

        private string waitSelectorShop = ".section_cinema_list";

        private string waitSelectorInfo = ".left";

        private string test2Code = @"() =>{
                                 const selectors = Array.from(document.querySelectorAll('.section_cinema_list > .list' ));
                                 for (let i = 0; i < selectors.length; i++){ Array.from(selectors[i].querySelectorAll('ul > li')).
                                 map(v=>{return {Region: selectors[i].querySelector('h3').textContent.trim(),Name: v.querySelector('.gotomap').textContent.trim()}})} 
                                 }";



        public WmoovGrabber(PuppeteerConnection puppeteerConnection, IOptions<WmoovOptions> options, IMemoryCache memoryCache,
            IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json) : base(httpClient, absOptions, json)
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
        }

        public async Task<List<IGeoComGrabModel?>> GetWebSiteItems()
        {
            var shopResult = await _puppeteerConnection.PuppeteerGrabber<WmoovModel[]>(_options.Value.BaseUrl, shopCode, waitSelectorShop);
            var shopList = shopResult.ToList();
            var _rgx = Regexs.ExtractInfo(WmoovModel.WmoovIdRegex);
            List<IGeoComGrabModel> WmoovIGeoComList = new List<IGeoComGrabModel>();
            foreach (var shop in shopList)
            {
                var infoResult = await _puppeteerConnection.PuppeteerGrabber<IGeoComGrabModel>(@$"https://wmoov.com{shop.Website}", infoCode, waitSelectorInfo);
        
                infoResult.ChineseName = shop.Name;
                infoResult.Latitude = Convert.ToDouble(shop.Latitude);
                infoResult.Longitude = Convert.ToDouble(shop.Longitude);
                NorthEastModel eastNorth = await this.getNorthEastNorth(infoResult.Latitude, infoResult.Longitude);
                if (eastNorth != null)
                {
                    infoResult.Easting = eastNorth.hkE;
                    infoResult.Northing = eastNorth.hkN;
                }
                infoResult.C_Address = shop.Address.Replace(" ", "");
                infoResult.Class = "CUF";
                infoResult.Type = "TNC";
                var matchId = _rgx.Matches(shop.Website!);
                if (matchId.Count > 0 && matchId != null)
                {
                    infoResult.GrabId = $"wmoov_{matchId[0].Value}";
                }
                WmoovIGeoComList.Add(infoResult);
            }

            return WmoovIGeoComList;

        }

        //public List<IGeoComGrabModel> FindAdded(List<IGeoComGrabModel> newData, List<IGeoComRepository> previousData)
        //{
        //    int newDataLength = newData.Count;
        //    int previousDataLength = previousData.Count;
        //    List<IGeoComGrabModel> AddedWellcomeIGeoComList = new List<IGeoComGrabModel>();

        //    for (int i = 0; i < newDataLength; i++)
        //    {
        //        int j;
        //        for (j = 0; j < previousDataLength; j++) 
        //            if (newData[i].C_Address?.Replace(" ","") == previousData[j].C_Address?.Replace(" ", "") &&
        //               newData[i].Tel_No?.Replace(" ", "").Replace("-", "") == previousData[j].Tel_No?.Replace(" ", "").Replace("-", "") &&
        //               newData[i].Web_Site == previousData[j].Web_Site
        //                )
        //                break;
        //            if (j == previousDataLength)
        //            {
        //                AddedWellcomeIGeoComList.Add(newData[i]);
        //            }
        //    }
        //    return AddedWellcomeIGeoComList;
        //}
    }
}
