namespace iGeoComAPI.Services
{
    public class ConnectHttpClient
    {
        private readonly HttpClient _client;
        private string _url;
        public ConnectHttpClient(HttpClient client, string url)
        {
            _client = client;
            _url = url;
        }
        public async Task<HttpResponseMessage> SendAsync()
        {
            HttpResponseMessage result = await _client.GetAsync(_url);
            result.EnsureSuccessStatusCode();
            return result;
        }
    }
}
