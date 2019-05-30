using System.Web.Http;
using ToSic.SexyContent.WebApi;

public class BasicController : SxcApiController
{
	[HttpGet]
	[AllowAnonymous]
	// http://2sexycontent.2dm.2sic/feat-shared-code/DesktopModules/2sxc/api/app/FeatureSharedCode/api/Some/TestBasic
	public string Hello()
	{
		return "Hello from the basic controller";
	}

}
