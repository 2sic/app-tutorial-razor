using System.Net.Http;
using System.Linq;
using System.Text.Json.Serialization;   // For JsonPropertyName

public class DataFromWebService : Custom.DataSource.DataSource16
{
  public DataFromWebService(Dependencies services) : base(services)
  {
    ProvideOut(() => {
      var response = new HttpClient()
        .GetAsync("https://api.open-meteo.com/v1/forecast?latitude=52.52&longitude=13.41&current_weather=true")
        .GetAwaiter()
        .GetResult();
      response.EnsureSuccessStatusCode();
      
      var responseBody = response.Content.ReadAsStringAsync()
        .GetAwaiter()
        .GetResult();
      var result = Kit.Convert.Json.To<WeatherData>(responseBody);

      return new {
        Temperature = result.Current.Temperature,
        WindSpeed = result.Current.WindSpeed,
        WindDirection = result.Current.WindDirection,
      };
    });
  }
}

// Helper classes for JSON deserialization
// Note that we're not using most of the properties, but we have them for completeness of the tutorial
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
  public CurrentWeather Current { get; set; }
}

public class CurrentWeather
{
  public double Temperature { get; set; }
  public double WindSpeed { get; set; }
  public double WindDirection { get; set; }
  public int WeatherCode { get; set; }
  public int Is_Day { get; set; }
  public string Time { get; set; }
}
