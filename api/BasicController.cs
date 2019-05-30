// the using statements tell the compiler what you need
// the first one is needed so you can write [HttpGet] and [AllowAnonymous]
// The second one is needed so the compiler can find the SxcApiController
using System.Web.Http;
using ToSic.SexyContent.WebApi;

// Tell the server that all commands can be accessed without a login 
[AllowAnonymous]
public class BasicController : SxcApiController
{
	// Tell the server that we're listening to GET requests
	[HttpGet]
	public string Hello()
	{
		return "Hello from the basic controller in /api";
	}

}
