// Best place this in /AppCode/DataSources so it will be pre-compiled together with all code.
namespace AppCode.DataSources
{
  // Inherit from DataSource16 which is the current base class for custom data sources
  public class Basic101 : Custom.DataSource.DataSource16
  {
    // The constructor defines what Out-streams will exist and how data will be generated.
    public Basic101(Dependencies services) : base(services)
    {
      // Define the "Default" output stream and just give it a single anonymous object with some data.
      ProvideOut(() => new {
        Title = "Hello from Basic101",
        TheAnswer = 42,
      });
    }
  }
}