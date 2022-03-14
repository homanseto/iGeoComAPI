using Newtonsoft.Json;

namespace iGeoComAPI.Services
{

    public class JsonFunction
    {
       
        public List<T>? Dserialize<T>(string message)
        {

            var resultArray = JsonConvert.DeserializeObject<List<T>>(message);
            return resultArray;
        }

        public T? DserializeSingle<T>(string message)
        {

            var resultArray = JsonConvert.DeserializeObject<T>(message);
            return resultArray;
        }


    }
}
