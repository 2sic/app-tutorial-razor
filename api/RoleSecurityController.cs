

todo: create an example using Roles

#if NETCOREAPP
using Microsoft.AspNetCore.Authorization; // .net core [AllowAnonymous] & [Authorize]
using Microsoft.AspNetCore.Mvc;           // .net core [HttpGet] / [HttpPost] etc.
using Oqtane.Shared;        // Oqtane role names
#else
// 2sxclint:disable:no-dnn-namespaces - 2sxclint:disable:no-web-namespace
using System.Web.Http;      // .net 4.5 [AllowAnonymous] / [HttpGet]
using DotNetNuke.Web.Api;   // [DnnModuleAuthorize] & [ValidateAntiForgeryToken]
using DotNetNuke.Security;  // SecurityAccessLevel.Xyz
#endif