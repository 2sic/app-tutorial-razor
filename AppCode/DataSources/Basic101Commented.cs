// 1.1 The class must have the same name as the file
// 1.2 It must inherit from Custom.DataSource.DataSource16 
public class Basic101Commented : Custom.DataSource.DataSource16
{
  // 2.1 The constructor must have the same name as the class
  // 2.2 It must also have a parameter of type MyServices
  // 2.3 It must call the base constructor with the same parameter
  // 2.4 This ensures that any internal functionality is initialized
  public Basic101Commented(MyServices services) : base(services)
  {
    // 3.1 The ProvideOut method must be called to define the output
    // 3.2 It must be called with a lambda expression (the () => part)
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
