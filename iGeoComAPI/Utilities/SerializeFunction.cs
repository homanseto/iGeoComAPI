using Newtonsoft.Json;

namespace iGeoComAPI.Services
{
    public partial class SevenElevenGrabber
    {
        public class SerializeFunction
        {
            private readonly HttpResponseMessage _message;
            public SerializeFunction(HttpResponseMessage message)
            {
                _message = message;
            }
            public async Task<List<T>?> Diserialize<T>()
            {
                string shops = await _message.Content.ReadAsStringAsync();
                var resultArray = JsonConvert.DeserializeObject<List<T>>(shops);
                return resultArray;
            }
        }
    }}
