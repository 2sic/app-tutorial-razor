// Best place this in /AppCode/DataSources so it will be pre-compiled together with all code.
using System.Text.Json.Serialization; // For JsonPropertyName

namespace AppCode.DataSources
{
  /// <summary>
  /// Helper classes for JSON deserialization of weather data.
  /// Note that we're not using most of the properties, but we have them for completeness of the tutorial
  /// </summary>
  public class WeatherData
  {
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    [JsonPropertyName("generationtime_ms")]
    public double GenerationTimeMs { get; set; }
    [JsonPropertyName("utc_offset_seconds")]
    public int UtcOffsetSeconds { get; set; }
    public string Timezone { get; set; }
    [JsonPropertyName("timezone_abbreviation")]
    public string TimezoneAbbreviation { get; set; }
    public double Elevation { get; set; }
    [JsonPropertyName("current_weather")]
    public WeatherDataCurrent Current { get; set; }
  }

  public class WeatherDataCurrent
  {
    public double Temperature { get; set; }
    public double WindSpeed { get; set; }
    public double WindDirection { get; set; }
    public int WeatherCode { get; set; }
    public int Is_Day { get; set; }
    public string Time { get; set; }
  }
}