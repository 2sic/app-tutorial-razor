// Add namespaces for security check in Oqtane & DNN despite differences in .net core/.net Framework
// If you only target one platform, you can remove the parts you don't need
#if NETCOREAPP
using Microsoft.AspNetCore.Authorization; // .net core [AllowAnonymous] & [Authorize]
using Microsoft.AspNetCore.Mvc;           // .net core [HttpGet] / [HttpPost] etc.
using Oqtane.Shared;                      // Oqtane role names for [Authorize]
#else
using System.Web.Http;                    // .net 4.5 [AllowAnonymous] / [HttpGet]
using DotNetNuke.Web.Api;                 // [DnnModuleAuthorize] & [ValidateAntiForgeryToken]
using DotNetNuke.Security;                // DnnRoles for SecurityAccessLevel.Anonymous
#endif

#if NETCOREAPP
[Produces("application/json")]  // Ensures that strings are JSON and have quotes around them in .net 5
#endif
[AllowAnonymous]                          // all commands can be accessed without a login
public class HybridController : Custom.Hybrid.Api14 // see https://r.2sxc.org/CustomWebApi
{
  [AllowAnonymous]                        // Explicitly set for this method
  [HttpGet]                               // [HttpGet] says we're listening to GET requests
  public string Hello()
  {
    return "Hello from the basic controller in /api";
  }

  #if OQTANE
  [Authorize(Roles = RoleNames.Everyone)] // Oqtane authorize everyone / anonymous
  #else
  [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)] // Dnn equivalent
  #endif
  [HttpGet]                               // [HttpGet] says we're listening to GET requests
  public int Square(int number)
  {
    return number * number;
  }
}

// The next line is for 2sxc-internal quality checks, you can ignore this
// 2sxclint:disable:no-dnn-namespaces - 2sxclint:disable:no-web-namespace