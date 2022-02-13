namespace iGeoComAPI.Services
{
    public class ConnectClient
    {
        HttpClient client = new HttpClient();
    
        public async Task<HttpResponseMessage> SendAsync(string url)
        {
            HttpResponseMessage result = await client.GetAsync(url);
            result.EnsureSuccessStatusCode();
            return result;
        }
    }
}
