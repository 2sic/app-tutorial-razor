// Add namespaces for security check in Oqtane & DNN despite differences in .net core/.net Framework
// If you only target one platform, you can remove the parts you don't need
#if NETCOREAPP
using Microsoft.AspNetCore.Authorization; // .net core [AllowAnonymous] & [Authorize]
using Microsoft.AspNetCore.Mvc;           // .net core [HttpGet] / [HttpPost] etc.
#else
using System.Web.Http;                    // .net 4.5 [AllowAnonymous] / [HttpGet]
using DotNetNuke.Web.Api;                 // [DnnModuleAuthorize] & [ValidateAntiForgeryToken]
#endif
using System.Linq;        // this enables .Select(x => ...)
using ToSic.Eav.DataFormats.EavLight; // For Auto-Conversion (see below)

[AllowAnonymous]                          // all commands can be accessed without a login
[ValidateAntiForgeryToken]                // protects API from users not on your site (CSRF protection)
public class BooksController : Custom.Hybrid.Api14 // see https://r.2sxc.org/CustomWebApi
{
  [HttpGet]                               // [HttpGet] says we're listening to GET requests
  public dynamic Persons()
  {
    // we could do: return App.Data["Persons"];
    // but the raw data is difficult to use, so we'll improve it

    // instead we'll create nice objects for our use
    return AsList(App.Data["Persons"])
      .Select(p => new {
        Id = p.EntityId,
        p.FirstName,
        p.LastName,
        Picture = p.Mugshot,
      });
  }

  [HttpGet]                               // [HttpGet] says we're listening to GET requests
  public dynamic PersonsAuto()
  {
    // 2sxclint:disable:v14-no-getservice
    var json = GetService<IConvertToEavLight>();
    return json.Convert(App.Data["Persons"]);
  }

  [HttpGet]                               // [HttpGet] says we're listening to GET requests
  public dynamic Books()
  {
    return AsList(App.Data["Books"])
      .Select(b => new {
        Id = b.EntityId,
        b.Title,
        b.Cover,
        Awards = AsList(b.Awards as object)
          .Select(a => a.EntityTitle)
      });
  }
}

// The next line is for 2sxc-internal quality checks, you can ignore this
// 2sxclint:disable:no-dnn-namespaces - 2sxclint:disable:no-web-namespace