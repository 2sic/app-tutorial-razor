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
