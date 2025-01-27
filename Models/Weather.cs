using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WeatherApp.Models
{
    public class Weather
    {
        [JsonPropertyName("description")]
        public required string Description { get; set; }

        [JsonPropertyName("icon")]
        public required string Icon { get; set; }
    }
}
