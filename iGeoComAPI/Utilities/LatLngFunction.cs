using iGeoComAPI.Models;
using iGeoComAPI.Options;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Utilities
{
    public class LatLngFunction 
    {
        private readonly PuppeteerConnection puppeteerConnection;
        private readonly ILogger<LatLngFunction> logger;
        private readonly IOptions<GoogleMapOptions> options;
        private string infoCode = @"()=>{
                                 const selectors = Array.from(document.querySelectorAll('div.Nv2PK > a.hfpxzc'));
                                 if(selectors.length != 0){
                                    return selectors[0].getAttribute('href');
                                 } else{
                                    return '';
                                 }
                                 }";
        private string waitSelector = "#pane";
        public LatLngFunction(PuppeteerConnection puppeteerConnection, ILogger<LatLngFunction> logger, IOptions<GoogleMapOptions> options)
        {
            this.puppeteerConnection = puppeteerConnection;
            this.logger = logger;
            this.options = options;
        }
        public async Task<LatLngModel>FindLatLngByAddress(string address)
        {
           LatLngModel latlng = new LatLngModel();
            //string newUrl = await _puppeteerConnection.GetUrl(url);
            string link = $"{this.options.Value.SearchUrl}{address}";
            string newUrl;
            newUrl = await this.puppeteerConnection.PuppeteerGrabber<string>(link, infoCode, waitSelector);
            if (newUrl == "")
                newUrl = await this.puppeteerConnection.GetUrl(link);
            var latRgx = Regexs.ExtractInfo(LatLngModel.getLat);
            var lngRgx = Regexs.ExtractInfo(LatLngModel.getLng);
            var lngExClamation = Regexs.ExtractInfo(LatLngModel.getLngWithExClamation);
            var latMatch = latRgx.Matches(newUrl);
            var lngMatch = lngRgx.Matches(newUrl);
            if (latMatch.Count > 0 && latMatch != null)
            {
                latlng.Latitude = Convert.ToDouble(latMatch[0].Value);
            }
            if (lngMatch[0].Value.Contains("!"))
            {
                var num = lngMatch[0].Value;
                var drawExClamation = lngExClamation.Matches(num);
                latlng.Longtitude = Convert.ToDouble(drawExClamation[0].Value);
            }else
            {
                latlng.Longtitude = Convert.ToDouble(lngMatch[0].Value);
            }
            return latlng;

        }
    }
}
