using System.Text.Json.Serialization;

namespace AppCode.Extensions.OpenMeteo.Dto
{
  /// <summary>
  /// Hourly forecast data arrays from Open-Meteo API.
  /// All arrays have matching lengths, with one element per hour.
  /// </summary>
  internal class MultipleResults
  {
    /// <summary>ISO 8601 timestamps (one per hour)</summary>
    [JsonPropertyName("time")]
    public string[] Time { get; set; }

    /// <summary>Temperature in celsius at 2m height (nullable for missing data)</summary>
    [JsonPropertyName("temperature_2m")]
    public double?[] Temperature { get; set; }

    /// <summary>Wind speed in km/h at 10m height (nullable for missing data)</summary>
    [JsonPropertyName("wind_speed_10m")]
    public double?[] WindSpeed { get; set; }

    /// <summary>WMO weather code (see OpenMeteoConstants for code interpretation)</summary>
    [JsonPropertyName("weather_code")]
    public int?[] WeatherCode { get; set; }
  }
}