using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace TexasChicken.Helpers
{
    public static class GeoHelper
    {
        public static async Task<(double?, double?)> GetLatLngFromAddress(string address)
        {
            using var httpClient = new HttpClient();

            // Nominatim API (OpenStreetMap)
            var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json&limit=1";

            // Nên set User-Agent theo yêu cầu của Nominatim
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TexasChickenApp/1.0");

            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return (null, null);

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = JsonSerializer.Deserialize<NominatimResult[]>(content, options);

            if (results != null && results.Length > 0)
            {
                double lat = double.Parse(results[0].Lat);
                double lon = double.Parse(results[0].Lon);
                return (lat, lon);
            }

            return (null, null);
        }

        private class NominatimResult
        {
            public string Lat { get; set; }
            public string Lon { get; set; }
        }
    }
}
