namespace iGeoComAPI.Services
{
    public class ConnectClient
    {
        
        public async Task<HttpResponseMessage> SendAsync(string url)
        {
            using(var client = new HttpClient())
            {
                HttpResponseMessage result = await client.GetAsync(url);
                result.EnsureSuccessStatusCode();
                return result;
            }  
        }
    }
}
