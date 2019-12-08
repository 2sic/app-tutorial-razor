using System.Web.Http;		// this enables [HttpGet] and [AllowAnonymous]
using System.Linq;
using DotNetNuke.Web.Api;	// this is to verify the AntiForgeryToken
using Dynlist = System.Collections.Generic.IEnumerable<dynamic>;

[AllowAnonymous]			// define that all commands can be accessed without a login
[ValidateAntiForgeryToken]	// protects the API from users not on your site (CSRF protection)
// Inherit from ToSic...ApiController to get features like App, Data or Dnn - see https://r.2sxc.org/CustomWebApi
public class BooksController : ToSic.Sxc.Dnn.ApiController
{

	[HttpGet]				// [HttpGet] says we're listening to GET requests
	public dynamic Persons()
	{
		return App.Data["Persons"];
	}

	[HttpGet]				// [HttpGet] says we're listening to GET requests
	public dynamic Books()
	{
		return App.Data["Books"];
	}

	[HttpGet]				// [HttpGet] says we're listening to GET requests
	public dynamic BookAuthors()
	{
		var firstBook = AsList(App.Data["Books"])
			.First();
		return (firstBook.Authors as Dynlist)
			.Select(a => a.FirstName + " " + a.LastName);
	}

}
