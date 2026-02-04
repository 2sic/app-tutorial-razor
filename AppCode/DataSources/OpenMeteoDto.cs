using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace AppCode.Extensions.OpenMeteo.Dto
{
  /// <summary>
  /// Root DTO for Open-Meteo API responses containing location info and weather data.
  /// </summary>
  internal class OpenMeteoDto
  {
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("timezone")]
    public string Timezone { get; set; }

    [JsonPropertyName("current")]
    public SingleResult Current { get; set; }

    [JsonPropertyName("hourly")]
    public MultipleResults Hourly { get; set; }

    /// <summary>Raw JSON response from API (stored for reference)</summary>
    public string Json { get; set; }

    /// <summary>Transforms current weather data into a displayable model.</summary>
    public object ToCurrentModel() => new 
    {
      When = Current?.Time,
      Current?.Temperature,
      Current?.WindSpeed,
      Weather = OpenMeteoConstants.GetDescription(Current?.WeatherCode ?? 0),
      Current?.WeatherCode,
      Timezone,
      Latitude,
      Longitude,
      Json
    };

    /// <summary>
    /// Transforms hourly forecast arrays into a collection of displayable models.
    /// Each element represents one hour with time, temperature, wind speed, and weather description.
    /// </summary>
    public IEnumerable<object> ToForecastModels()
    {
      if (Hourly?.Time == null)
        return Array.Empty<object>();

      return Hourly.Time
        .Select((time, index) => CreateForecastModel(time, index))
        .ToArray();
    }

    /// <summary>
    /// Creates a single forecast model object for a given time index.
    /// </summary>
    private object CreateForecastModel(string time, int index) => new
    {
      When = time,
      Temperature = Hourly.Temperature?[index],
      WindSpeed = Hourly.WindSpeed?[index],
      Weather = OpenMeteoConstants.GetDescription(Hourly.WeatherCode?[index] ?? 0),
      WeatherCode = Hourly.WeatherCode?[index],
      Timezone,
      Latitude,
      Longitude,
      Json
    };
  }
}