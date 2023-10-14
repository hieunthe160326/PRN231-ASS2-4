using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace eStoreWebMVC.Helper
{
    public static class APIHelper
    {
        public static async Task<T> GetApi<T>(this HttpClient client, string api)
        {
            HttpResponseMessage res = await client.GetAsync(api);
            var data = await res.Content.ReadAsStringAsync();

            var jsonData = JObject.Parse(data);
            string? valueData;

            if (jsonData.ContainsKey("value"))
                valueData = jsonData["value"]!.ToString();
            else
                valueData = jsonData.ToString();

            var opt = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var obj = JsonSerializer.Deserialize<T>(valueData, opt);
            return obj!;
        }
    }
}
