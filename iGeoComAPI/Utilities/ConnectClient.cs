﻿using Microsoft.AspNetCore.WebUtilities;

namespace iGeoComAPI.Services
{
    public class ConnectClient
    {
        private readonly ILogger<ConnectClient> _logger;
        public ConnectClient(ILogger<ConnectClient> logger )
        {
            _logger = logger;
        }

        public async Task<string> GetAsync(string? url, Dictionary<string, string>? parameter = null)
        {
            try
            {
                _logger.LogInformation("HttpResponseMessage");
                //if (parameter == null) parameter = new Dictionary<string, string>();
                if (url != null && parameter != null)
                {
                    url = QueryHelpers.AddQueryString(url, parameter);
                }
                    using (var client = new HttpClient())
                    {
                        HttpResponseMessage resultMessage = await client.GetAsync(url);
                        resultMessage.EnsureSuccessStatusCode();
                        string result = await resultMessage.Content.ReadAsStringAsync();
                        return result;
                    }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }
    }
}

//public async Task<string> GetAsync(string? url, string? parameter = "")
//{
//    try
//    {
//        _logger.LogInformation("HttpResponseMessage");
//        if (url != null)
//        {
//            UriBuilder uriBuilder = new UriBuilder(url!);
//            uriBuilder.Query = parameter;

//            using (var client = new HttpClient())
//            {
//                HttpResponseMessage resultMessage = await client.GetAsync(uriBuilder.Uri);
//                resultMessage.EnsureSuccessStatusCode();
//                string result = await resultMessage.Content.ReadAsStringAsync();
//                return result;
//            }
//        }
//        else
//            throw new Exception("url cannot be empty or null");
//    }
//    catch (Exception ex)
//    {
//        _logger.LogError(ex.Message);
//        throw;
//    }

//}