// Add namespaces for security check in Oqtane & DNN despite differences in .net core/.net Framework
// If you only target one platform, you can remove the parts you don't need
#if NETCOREAPP
using Microsoft.AspNetCore.Authorization; // .net core [AllowAnonymous] & [Authorize]
using Microsoft.AspNetCore.Mvc;           // .net core [HttpGet] / [HttpPost] etc.
#else
using System.Web.Http;                    // .net 4.5 [AllowAnonymous] / [HttpGet]
using DotNetNuke.Web.Api;                 // [DnnModuleAuthorize] & [ValidateAntiForgeryToken]
#endif
using System.Collections.Generic;

[AllowAnonymous]                          // all commands can be accessed without a login
public class OrdersController : Custom.Hybrid.Api14 // see https://r.2sxc.org/CustomWebApi
{
  public List<Order> orders = new List<Order>{
    new Order{ Id = 1, Amount = 10 },
    new Order{ Id = 2, Amount = 37 }
  };

  [HttpGet]				// [HttpPost] says we're listening to POST requests
  public List<Order> Get()
  {
    return orders;
  }

  [HttpPost]				// [HttpPost] says we're listening to POST requests
  public Order Post([FromBody] Order order)
  {
    order.Id = orders[orders.Count - 1].Id + 1;
    orders.Add(order);
    return orders[orders.Count - 1];
  }

  [HttpPut]				// [HttpPut] says we're listening to PUT requests
  public Order Put([FromBody] Order order)
  {
    orders[0].Amount = order.Amount;
    return orders[0];
  }

  [HttpDelete]        // [HttpDelete] says we're listening to DELETE requests
  public string Delete()
  {
    orders.RemoveAt(0);
    return "";
  }
}

public class Order {
  public int Id;
  public int Amount;
}