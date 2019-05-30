using System.Web.Http;
using ToSic.SexyContent.WebApi;

public class DemoController : SxcApiController
{
	[HttpGet]
	[AllowAnonymous]
	public string Hello()
	{
		return "Hello from the live controller";
	}

}
