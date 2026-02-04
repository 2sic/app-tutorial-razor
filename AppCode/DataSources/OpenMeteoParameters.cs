using ToSic.Eav.DataSource;

namespace AppCode.Extensions.OpenMeteo
{
  /// <summary>
  /// Parameters for Open-Meteo Data Sources.
  /// </summary>
  public class OpenMeteoParameters: IDataSourceParameters
  {
    /// <summary>
    /// Latitude of the location to get the weather for.
    /// </summary>
    public double Latitude;

    /// <summary>
    /// Longitude of the location to get the weather for.
    /// </summary>
    public double Longitude;

    /// <summary>
    /// Timezone for the weather data. Use "auto" to use the timezone of the specified coordinates.
    /// </summary>
    public string Timezone = "auto";

    /// <summary>
    /// Number of days to forecast (1 day = 24 hours).
    /// </summary>
    public int ForecastDays = 1;
  }
}