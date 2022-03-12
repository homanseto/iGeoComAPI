namespace iGeoComAPI.Services
{
    public class ConnectClient
    {
        private readonly ILogger<ConnectClient> _logger;
        public ConnectClient(ILogger<ConnectClient> logger )
        {
            _logger = logger;
        }
        
        public async Task<HttpResponseMessage> GetAsync(string? url, string? parameter = ""  )
        {
            try
            {
                _logger.LogInformation("HttpResponseMessage");
                if (url != null)
                {
                    UriBuilder uriBuilder = new UriBuilder(url!);
                    uriBuilder.Query = parameter;

                    using (var client = new HttpClient())
                    {
                        HttpResponseMessage result = await client.GetAsync(uriBuilder.Uri);
                        result.EnsureSuccessStatusCode();
                        return result;
                    }
                }
                _logger.LogError("url cannot be empty or null");
                throw new Exception("url cannot be empty or null");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            
        }
    }
}
