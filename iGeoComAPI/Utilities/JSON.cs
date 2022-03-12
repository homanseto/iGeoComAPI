using Newtonsoft.Json;

namespace iGeoComAPI.Services
{

    public class JSON
    {
       
        public async Task<List<T>?> Diserialize<T>(HttpResponseMessage message)
        {
            string shops = await message.Content.ReadAsStringAsync();
            var resultArray = JsonConvert.DeserializeObject<List<T>>(shops);
            return resultArray;
        }
    }
}
