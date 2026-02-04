using System.Text.Json.Serialization;

namespace AppCode.Extensions.OpenMeteo.Dto
{
  /// <summary>
  /// Current weather data for a specific point in time from Open-Meteo API.
  /// </summary>
  internal class SingleResult
  {
    /// <summary>ISO 8601 timestamp of the current observation</summary>
    [JsonPropertyName("time")]
    public string Time { get; set; }

    /// <summary>Temperature in celsius at 2m height</summary>
    [JsonPropertyName("temperature_2m")]
    public double? Temperature { get; set; }

    /// <summary>Wind speed in km/h at 10m height</summary>
    [JsonPropertyName("wind_speed_10m")]
    public double? WindSpeed { get; set; }

    /// <summary>WMO weather code (see OpenMeteoConstants for code interpretation)</summary>
    [JsonPropertyName("weather_code")]
    public int? WeatherCode { get; set; }
  }
}