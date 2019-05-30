// the using statements tell the compiler what you need
// the first one is needed so you can write [HttpGet] and [AllowAnonymous]
// The second one is needed so the compiler can find the SxcApiController
// The final one allows us to verify the AntiForgeryToken
using System.Web.Http;
using ToSic.SexyContent.WebApi;
using DotNetNuke.Web.Api;

// Tell the server that all commands can be accessed without a login
// But requires the user to be on your website
// the the API can't be used from other websites (CSRF protection)
[AllowAnonymous]
[ValidateAntiForgeryToken]
public class VerifiedController : SxcApiController
{
	// Tell the server that we're listening to GET requests
	[HttpGet]
	public string Hello()
	{
		return "Hello from the controller with ValidateAntiForgeryToken in /api";
	}

}
