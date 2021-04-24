// Add namespaces to enable security in Oqtane & DNN despite the differences
#if OQTANE
using Microsoft.AspNetCore.Authorization; // .net core [AllowAnonymous] & [Authorize]
using Microsoft.AspNetCore.Mvc;           // .net core [HttpGet] / [HttpPost] etc.
using Oqtane.Shared;        // Oqtane role names
#else
using System.Web.Http;      // .net 4.5 [AllowAnonymous] / [HttpGet]
using DotNetNuke.Web.Api;   // [DnnModuleAuthorize] & [ValidateAntiForgeryToken]
using DotNetNuke.Security;  // SecurityAccessLevel.Xyz
#endif

#if OQTANE
[Produces("application/json")]  // Ensures that strings are JSON and have quotes around them in .net 5
#endif
[ValidateAntiForgeryToken]      // This has the same name, but comes from a different namespace
public class HybridController : ToSic.Custom.Api12
{
  [AllowAnonymous]  // Works in Oqtane and DNN
  [HttpGet]         // Works in Oqtane and DNN
  public string Hello()
  {
    return "Hello from the basic controller in /api";
  }

  #if OQTANE
  [Authorize(Roles = RoleNames.Everyone)]
  #else
  [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
  #endif
  [HttpGet]
  public int Square(int number)
  {
    return number * number;
  }
}
