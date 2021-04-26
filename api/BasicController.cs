using System.Web.Http;		// this enables [HttpGet] and [AllowAnonymous]

// All commands can be accessed without security checks
[AllowAnonymous]
// Inherit from ToSic...Api12 to get features like App, Data or Dnn - see https://r.2sxc.org/CustomWebApi
public class BasicController : Custom.Hybrid.Api12
{

  [HttpGet]				// [HttpGet] says we're listening to GET requests
  public string Hello()
  {
    return "Hello from the basic controller in /api";
  }


  [HttpGet]				// [HttpGet] says we're listening to GET requests
  public int Square(int number)
  {
    return number * number;
  }


}
