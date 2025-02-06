using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherApp.Models;

namespace WeatherApp.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = "811dde43fb83489a3cbff4d769d73b9a";
    private readonly string _baseUrl = "https://api.openweathermap.org/data/2.5/weather";

    public WeatherService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<WeatherData> GetWeatherDataAsync(double latitude, double longitude)
    {
        try
        {
            var url = $"{_baseUrl}?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric";
            Console.WriteLine($"Requesting URL: {url}");

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Response Status: {response.StatusCode}");
            Console.WriteLine($"Response Content: {content}");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var weatherData = JsonSerializer.Deserialize<WeatherData>(content, options);

                if (weatherData == null)
                {
                    throw new Exception("Failed to deserialize weather data");
                }

                return weatherData;
            }

            throw new Exception($"API returned {response.StatusCode}: {content}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching weather data: {ex}");
            throw new Exception($"Failed to fetch weather data: {ex.Message}");
        }
    }
}