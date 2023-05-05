public class DataFromWebService : Custom.DataSource.DataSource16
{
  public DataFromWebService(MyServices services) : base(services)
  {
    ProvideOut(() => new {
      Title = "Hello from Basic101",
      TheAnswer = 42,
    });
  }
}
