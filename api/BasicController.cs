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
public class BasicController : Custom.Hybrid.Api14 // see https://r.2sxc.org/CustomWebApi
{
  [HttpGet]                               // [HttpGet] says we're listening to GET requests
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

// The next line is for 2sxc-internal quality checks, you can ignore this
// 2sxclint:disable:no-dnn-namespaces - 2sxclint:disable:no-web-namespace