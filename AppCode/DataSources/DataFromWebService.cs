// Best place this in /AppCode/DataSources so it will be pre-compiled together with all code.
using System.Net.Http;    // For HttpClient / WebAPI calls

namespace AppCode.DataSources
{
  public class DataFromWebService : Custom.DataSource.DataSource16
  {
    /// <summary>
    /// Constructor - just to define how the data is provided
    /// </summary>
    public DataFromWebService(Dependencies services) : base(services)
    {
      ProvideOut(GetWeather);
    }

    /// <summary>
    /// Fetches weather data from a public web service and returns an object with selected properties.
    /// </summary>
    private object GetWeather() {
      var response = new HttpClient()
        .GetAsync("https://api.open-meteo.com/v1/forecast?latitude=52.52&longitude=13.41&current_weather=true")
        .GetAwaiter()
        .GetResult();
      response.EnsureSuccessStatusCode();
      
      var responseBody = response.Content
        .ReadAsStringAsync()
        .GetAwaiter()
        .GetResult();
      var result = Kit.Convert.Json.To<WeatherData>(responseBody);

      // Return an anonymous object with selected properties
      // As of now, only one or lists of: anonymous objects, IEntity or IEntityRaw are supported.
      return new {
        result.Current.Temperature,
        result.Current.WindSpeed,
        result.Current.WindDirection,
      };
    }
  }
}