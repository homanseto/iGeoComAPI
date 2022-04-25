using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public abstract class AbstractGrabber
    {
        private readonly ConnectClient httpClient;
        private IOptions<NorthEastOptions> absOptions;
        private JsonFunction json;
        public AbstractGrabber(ConnectClient httpClient, IOptions<NorthEastOptions> absOptions, JsonFunction json)
        {
            this.httpClient = httpClient;
            this.absOptions = absOptions;
            this.json = json;
        }


        public async Task<NorthEastModel?> getNorthEastNorth(double lat, double lng)
        {
                var query = new Dictionary<string, string>()
                {
                    ["inSys"] = this.absOptions.Value.InSys,
                    ["iutSys"] = this.absOptions.Value.IutSys,
                    ["lat"] = lat.ToString(),
                    ["long"] = lng.ToString()
                };

                var result = await this.httpClient.GetAsync(this.absOptions.Value.ConvertNE, query);
                return this.json.Dserialize<NorthEastModel>(result);

        }
    }
}
