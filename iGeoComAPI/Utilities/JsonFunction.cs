using Newtonsoft.Json;

namespace iGeoComAPI.Services
{

    public class JsonFunction
    {
       
        public T? Dserialize<T>(string message)
        {

            var resultArray = JsonConvert.DeserializeObject<T>(message);
            return resultArray;
        }

        public string Serialize<T>(T input)
        {
            var jsonResult = JsonConvert.SerializeObject(input);
            return jsonResult;
        }

    }
}
