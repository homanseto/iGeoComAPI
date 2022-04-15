using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public abstract class AbstractGrabber
    {
        private readonly ConnectClient _httpClient;
        private IOptions<NorthEastOptions> _absOptions;
        private JsonFunction _json;
        public AbstractGrabber(ConnectClient httpClient, IOptions<NorthEastOptions> absOptions, JsonFunction json)
        {
            _httpClient = httpClient;
            _absOptions = absOptions;
            _json = json;
        }


        public async Task<NorthEastModel?> getNorthEastNorth(double lat, double lng)
        {
            if (lat < 0 || lng < 0)
            {
                var query = new Dictionary<string, string>()
                {
                    ["inSys"] = _absOptions.Value.InSys,
                    ["iutSys"] = _absOptions.Value.IutSys,
                    ["lat"] = lat.ToString(),
                    ["long"] = lng.ToString()
                };

                var result = await _httpClient.GetAsync(_absOptions.Value.ConvertNE, query);
                return _json.Dserialize<NorthEastModel>(result);
            }else
                throw new Exception("lat and lng cannot be empty or zero");

        }
    }
}
