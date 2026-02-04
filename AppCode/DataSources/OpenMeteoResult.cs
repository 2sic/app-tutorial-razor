namespace AppCode.Extensions.OpenMeteo.Data
{
  public partial class OpenMeteoResult
  {
    /// <summary>
    /// Temperature in Fahrenheit, calculated from the Celsius value.
    /// </summary>
    public decimal TemperatureFahrenheit => (Temperature * 9 / 5) + 32;
  }
}
