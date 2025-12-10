// Best place this in /AppCode/DataSources so it will be pre-compiled together with all code.
namespace AppCode.DataSources
{
  // The class must inherit from Custom.DataSource.DataSource16 
  // which is the current base class for custom data sources
  public class Basic101Commented : Custom.DataSource.DataSource16
  {
    // The constructor must have the same name as the class
    // It must also have a parameter of type Dependencies which it passes to the base constructor
    // This ensures that any internal functionality is initialized
    public Basic101Commented(Dependencies services) : base(services)
    {
      // 3.1 The ProvideOut method must be called to define the output
      // 3.2 It must receive a function or a lambda expression (the () => part)
      // 3.3 In this example we're just returning a single object
      // 3.4 The object can be anonymous, as shown here
      ProvideOut(() => new {
        // Our object has a Title property with a hello-message
        // and another property with the answer to life, the universe and everything
        Title = "Hello from Basic101-Commented",
        TheAnswer = 42,
      });
    }
  }
}