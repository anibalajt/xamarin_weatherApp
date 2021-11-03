using System;
using System.Net.Http;
using System.Threading.Tasks;
using Weather.Models;

namespace Weather.Services
{
    public class ApiService
    {
        const string apiKey = "45a2fd01d7fd7f558c59a65e81a1dede";
        public ApiService()
        {
        }
        //api.openweathermap.org/data/2.5/weather?lat=-31.9464646&lon=115.8614118&appid=45a2fd01d7fd7f558c59a65e81a1dede
        public async Task<string> GetWeatherpByLonLat(string lat,string lon)
        {
            string content = await GetData($"{Constants.OpenWeatherEndpoint}lat={lat}&lon={lon}&appid={apiKey}");
            return content;
        }
        public async Task<string> GetWeatherBycity(string city)
        {
            string content = await GetData($"{Constants.OpenWeatherEndpoint}q={city}&appid={apiKey}");
            return content;
        }
        public async Task<string> GetDataCity(string city)
        {
            string content = await GetData($"{Constants.OpenStreetMapEndpoint}q={city}");
            return city;
        }
        public async Task<string> GetData(string endpoint)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(endpoint);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string content = await response.Content.ReadAsStringAsync();
                
                return content;
            }
            return null;
        }
    }
}
