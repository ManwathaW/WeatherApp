using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WeatherApp.Models
{
    public class WeatherData
    {
        [JsonPropertyName("main")]
        public required MainData Main { get; set; }

        [JsonPropertyName("weather")]
        public required List<Weather> Weather { get; set; }

        [JsonPropertyName("name")]
        public required string CityName { get; set; }
    }
}
