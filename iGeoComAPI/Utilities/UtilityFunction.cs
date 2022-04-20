using iGeoComAPI.Models;

namespace iGeoComAPI.Utilities
{
    public class UtilityFunction 
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        public UtilityFunction(PuppeteerConnection puppeteerConnection)
        {
            _puppeteerConnection = puppeteerConnection;
        }
        public async Task FindLatLngByAddress(string url)
        {
           LatLngModel latlng = new LatLngModel();
           string newUrl = await _puppeteerConnection.GetUrl(url);
            Console.WriteLine(newUrl);
        }
    }
}
