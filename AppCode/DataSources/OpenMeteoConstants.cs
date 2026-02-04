using System.Collections.Generic;

namespace AppCode.Extensions.OpenMeteo
{
  /// <summary>
  /// Contains constants and mappings for Open-Meteo weather API integration.
  /// </summary>
  internal class OpenMeteoConstants
  {
    /// <summary>
    /// Comma-separated list of fields requested from Open-Meteo API
    /// </summary>
    public const string ExpectedFields = "temperature_2m,wind_speed_10m,weather_code";

    /// <summary>
    /// Weather Codes mapping to readable descriptions
    /// </summary>
    public static Dictionary<int, string> WeatherInterpretationCodes = new Dictionary<int, string>
    {
      { 0, "Clear sky" },
      { 1, "Mainly clear" },
      { 2, "Partly cloudy" },
      { 3, "Overcast" },
      { 45, "Fog" },
      { 48, "Depositing rime fog" },
      { 51, "Light drizzle" },
      { 53, "Moderate drizzle" },
      { 55, "Dense drizzle" },
      { 56, "Light freezing drizzle" },
      { 57, "Dense freezing drizzle" },
      { 61, "Slight rain" },
      { 63, "Moderate rain" },
      { 65, "Heavy rain" },
      { 66, "Light freezing rain" },
      { 67, "Heavy freezing rain" },
      { 71, "Slight snowfall" },
      { 73, "Moderate snowfall" },
      { 75, "Heavy snowfall" },
      { 77, "Snow grains" },
      { 80, "Slight rain showers" },
      { 81, "Moderate rain showers" },
      { 82, "Violent rain showers" },
      { 85, "Slight snow showers" },
      { 86, "Heavy snow showers" },
      { 95, "Slight or moderate thunderstorm" },
      { 96, "Thunderstorm with hail" },
      { 99, "Thunderstorm with severe hail" }
    };

    /// <summary>
    /// Gets the weather description for a WMO code.
    /// </summary>
    /// <param name="code">WMO weather code (e.g., 0, 1, 61, 95)</param>
    /// <returns>Human-readable weather description, or "Unknown" if code not found</returns>
    public static string GetDescription(int code)
    {
      return WeatherInterpretationCodes.TryGetValue(code, out var description)
        ? description
        : "Unknown";
    }
  }
}