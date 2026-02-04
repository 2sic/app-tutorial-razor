using System.Collections.Generic;
using AppCode.Extensions.OpenMeteo.Data;
using Custom.DataSource;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;

namespace AppCode.Extensions.OpenMeteo
{
  /// <summary>
  /// DataSource which loads an hourly forecast from the Open-Meteo API.
  /// <br/>
  /// Returns one record per hour containing time, temperature, wind speed and weather code
  /// for the configured location.
  /// <br/>
  /// Uses the strongly typed OpenMeteoResult model.
  /// </summary>
  [VisualQuery(
    NiceName = "OpenMeteo Forecast",
    UiHint = "Loads hourly forecast data from Open-Meteo",
    Icon = "schedule",
    ConfigurationType = nameof(OpenMeteoConfiguration)
  )]
  public class OpenMeteoForecast : DataSource16
  {
    public OpenMeteoForecast(Dependencies services) : base(services)
    {
      ProvideOut(GetForecast);
    }

    private IEnumerable<object> GetForecast()
    {
      var result = OpenMeteoHelpers.Download(Kit, Latitude, Longitude, Timezone,
        $"&hourly={OpenMeteoConstants.ExpectedFields}&forecast_days={ForecastDays}"
      );

      return result.ToForecastModels();
    }

    [Configuration(Fallback = "47.1674")]
    public double Latitude => Configuration.GetThis(47.1674);

    [Configuration(Fallback = "9.4779")]
    public double Longitude => Configuration.GetThis(9.4779);

    [Configuration(Fallback = "auto")]
    public string Timezone => Configuration.GetThis<string>("auto");

    [Configuration(Fallback = "2")]
    public int ForecastDays => Configuration.GetThis(2);
  }
}
