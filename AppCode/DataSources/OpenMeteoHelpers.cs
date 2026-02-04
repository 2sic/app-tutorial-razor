using System;
using System.Net.Http;
using AppCode.Extensions.OpenMeteo.Dto;
using ToSic.Sxc.Services;

namespace AppCode.Extensions.OpenMeteo
{
  /// <summary>
  /// Helper methods for downloading and processing Open-Meteo weather API data.
  /// </summary>
  internal static class OpenMeteoHelpers
  {
    private const string BaseUrl = "https://api.open-meteo.com/v1/forecast";

    /// <summary>
    /// Downloads weather data from the Open-Meteo API and deserializes the JSON response.
    /// </summary>
    /// <param name="kit">Service kit for conversion utilities</param>
    /// <param name="latitude">Location latitude (e.g., 47.1674)</param>
    /// <param name="longitude">Location longitude (e.g., 9.4779)</param>
    /// <param name="timezone">Timezone identifier or "auto" (e.g., "Europe/Zurich")</param>
    /// <param name="extraQuery">Additional query parameters (e.g., "&current=..." or "&hourly=...")</param>
    /// <returns>Deserialized API response as OpenMeteoDto object; Json property contains raw JSON</returns>
    internal static OpenMeteoDto Download(
      ServiceKitLight16 kit,
      double latitude,
      double longitude,
      string timezone,
      string extraQuery
    )
    {
      var url = BuildUrl(kit, latitude, longitude, timezone, extraQuery);
      var json = DownloadJson(url);
      var result = kit.Convert.Json.To<OpenMeteoDto>(json);
      result.Json = json;
      return result;
    }

    /// <summary>
    /// Builds the complete API URL with all query parameters.
    /// </summary>
    private static string BuildUrl(ServiceKitLight16 kit, double latitude, double longitude, string timezone, string extraQuery)
      => BaseUrl +
        $"?latitude={kit.Convert.ForCode(latitude)}" +
        $"&longitude={kit.Convert.ForCode(longitude)}" +
        $"&timezone={Uri.EscapeDataString(timezone)}" +
        extraQuery;

    /// <summary>
    /// Downloads JSON string from the specified URL with appropriate headers.
    /// </summary>
    private static string DownloadJson(string url)
    {
      using var httpClient = new HttpClient
      {
        DefaultRequestHeaders = { { "User-Agent", "2sxc OpenMeteo DataSource" } }
      };
      var response = httpClient.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
      response.EnsureSuccessStatusCode();
      return response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
    }
  }
}