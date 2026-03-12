using VozdelEste.Models.Monedas;
using VozdelEste.Models.Weather;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Net;

namespace VozdelEste.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12; 
            _httpClient = new HttpClient();
        }

        public async Task<Weather> GetWeatherDataAsync()
        {
            string apiKey = "xxxxxxxxxxxxx";
            string url = $"https://api.openweathermap.org/data/2.5/weather?lat=-34.6667&lon=-54.9167&appid={apiKey}&units=metric&lang=es";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return Weather.FromJson(json);
        }
        public async Task<JObject> GetForecastDataAsync()
        {
            const string key = "xxxxxxxxxxxxx";
            string url = "https://api.openweathermap.org/data/2.5/forecast"
                       + "?lat=-34.6667&lon=-54.9167&units=metric&lang=es&appid=" + key;

            var json = await _httpClient.GetStringAsync(url);
            return JObject.Parse(json);             
        }

        public async Task<Monedas> GetCurrencyDataAsync()
        {
            string apiKey = "xxxxxxxxxxxxx";
            string url = $"http://api.currencylayer.com/live?access_key={apiKey}&currencies=USD,EUR,BRL&source=UYU&format=1";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("🔁 JSON Monedas:");
            Debug.WriteLine(json);

            if (json.Contains("\"success\":false"))
            {
                Debug.WriteLine("❌ Error en la API de cotizaciones");
                return null;
            }

            return Monedas.FromJson(json);
        }
    }
}

