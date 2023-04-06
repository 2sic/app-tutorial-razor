public class Basic101 : Custom.DataSource.DataSource15
{
  public Basic101(MyServices services) : base(services)
  {
    ProvideOut(() => new {
      Title = "Hello from Basic101",
      TheAnswer = 42,
    });
  }
}
