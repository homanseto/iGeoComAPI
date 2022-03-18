using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

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
        private string _extraInfoRegex = @"(^.+LatLng\(| \),.*pop_title fs_20"" >|< a class=""marker_detail_link fs_12"".*<div class=""location_addr"">|<\/div><div class=""location_openhour"">|<\/div\><div class=""location_addtocalander""\>.*)";

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
            Console.WriteLine(_extraInfoRegex);
            return enScript;
        }
    }
}
