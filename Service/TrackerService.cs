using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ReportingService.Service
{
    public class TrackerService
    {

        //private static readonly string apiBasicUri = Environment.GetEnvironmentVariable("");
        private static readonly string apiBasicUri = "https://trackingsvcapp.azurewebsites.net/";
        public static async Task<ResponseWrapper> Get<ResponseWrapper>(string url)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBasicUri);
                var result = await client.GetAsync(url);
                result.EnsureSuccessStatusCode();
                string resultContentString = await result.Content.ReadAsStringAsync();

                ResponseWrapper resultContent = JsonConvert.DeserializeObject<ResponseWrapper>(resultContentString);
                return resultContent;
            }
        }
    }
}
