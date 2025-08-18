// This sample uses Enumerable.Repeat, which needs this namespace
using System.Linq;

public class ListMultiStream : Custom.DataSource.DataSource16
{
  public ListMultiStream(Dependencies services) : base(services)
  {
    // First stream "Settings" containing just one item
    ProvideOut(() => new {
      Title = "Settings Entity",
      PageSize = 50,
      ShowStuff = true,
    }, name: "Settings");

    // A second stream on the normal (unnamed) "Default"
    ProvideOut(() => Enumerable.Range(1, 5).Select(i => new {
        Id = i,
        guid = System.Guid.NewGuid(),
        Title = "Hello from ListMultiStream",
        FavoriteNumber = 2742,
      })
    );
  }
}
