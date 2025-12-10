// This is just a quick, short first sample
// Check out the next sample which shows more detailed comments as to what is happening.
namespace AppCode.DataSources
{
  public class Basic101 : Custom.DataSource.DataSource16
  {
    public Basic101(Dependencies services) : base(services)
    {
      ProvideOut(() => new {
        Title = "Hello from Basic101",
        TheAnswer = 42,
      });
    }
  }
}