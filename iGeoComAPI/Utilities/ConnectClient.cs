namespace iGeoComAPI.Services
{
    public class ConnectClient
    {
        private readonly ILogger<ConnectClient> _logger;
        public ConnectClient(ILogger<ConnectClient> logger )
        {
            _logger = logger;
        }
        
        public async Task<HttpResponseMessage> SendAsync(string url)
        {
            try
            {
                _logger.LogInformation("HttpResponseMessage");
                using (var client = new HttpClient())
                {
                    HttpResponseMessage result = await client.GetAsync(url);
                    result.EnsureSuccessStatusCode();
                    return result;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            
        }
    }
}
