using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public abstract class AbstractGrabber
    {
        private readonly ConnectClient _httpClient;
        private IOptions<NorthEastOptions> _options;
        public AbstractGrabber(ConnectClient httpClient, IOptions<NorthEastOptions> options)
        {
            _httpClient = httpClient;
            _options = options;
        }


        public void getNorthEastNorth(double lat, double lng)
        {
           
        }
    }
}
