using Newtonsoft.Json;
using System.Text;

namespace RestaurantAPI.IntegrationTests.Helpers
{
    public static class HttpContentHelper
    {
        public static HttpContent ToJsonHttpContent(this object obj)
        {
            var json =  JsonConvert.SerializeObject(obj);
            return new StringContent(json,UnicodeEncoding.UTF8,"application/json");
        }
    }
}
