using System.Collections.Generic;
using System.Linq;

public class WithConfig : Custom.DataSource.DataSource15
{
  public WithConfig(MyServices services) : base(services, "My.Magic")
  {
    ProvideOut(() => {
      var newItem = {
        Id = 27,
        Title = "Hello from ListMultiStream",
        FavoriteNumber = 42,
      };
      return Enumerable.Repeat(newItem, AmountOfItems).ToList();
    });
  }

  [Configuration(Fallback = 3)]
  public int AmountOfItems { get { return Configuration.GetThis(3); } }

  [Configuration(Fallback = 42)]
  public int FavoriteNumber { get { return Configuration.GetThis(42); } }
}
