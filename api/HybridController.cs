// Add namespaces to enable [HttpGet] and [AllowAnonymous]
// In Oqtane it's on a different .net namespace than in Dnn
#if OQTANE
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#endif
#if !OQTANE
using System.Web.Http;		
#endif

// All commands can be accessed without security checks
[AllowAnonymous]
public class HybridController : ToSic.Custom.Api12
{
  [HttpGet] public string Hello()
  {
    return "Hello from the basic controller in /api";
  }

  [HttpGet] public int Square(int number)
  {
    return number * number;
  }
}
