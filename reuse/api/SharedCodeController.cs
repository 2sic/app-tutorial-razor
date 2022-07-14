// Add namespaces for security check in Oqtane & DNN despite differences in .net core/.net Framework
// If you only target one platform, you can remove the parts you don't need
#if NETCOREAPP
using Microsoft.AspNetCore.Authorization; // .net core [AllowAnonymous] & [Authorize]
using Microsoft.AspNetCore.Mvc;           // .net core [HttpGet] / [HttpPost] etc.
#else
using System.Web.Http;                    // .net 4.5 [AllowAnonymous] / [HttpGet]
using DotNetNuke.Web.Api;                 // [DnnModuleAuthorize] & [ValidateAntiForgeryToken]
#endif

[AllowAnonymous]                          // all commands can be accessed without a login
[ValidateAntiForgeryToken]                // protects API from users not on your site (CSRF protection)
public class SharedCodeController : Custom.Hybrid.Api14 // see https://r.2sxc.org/CustomWebApi
{
	[HttpGet]
	[AllowAnonymous]
	public string Hello()
	{
		var shared = CreateInstance("../FunctionsBasic.cs");
		return shared.SayHello();
	}
}

// The next line is for 2sxc-internal quality checks, you can ignore this
// 2sxclint:disable:no-dnn-namespaces - 2sxclint:disable:no-web-namespace