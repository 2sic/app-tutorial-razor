using System.Linq;
using ToSic.Eav.DataSource; // This namespace is for the [Configuration] attribute

public class WithConfig : Custom.DataSource.DataSource16
{
  public WithConfig(Dependencies services) : base(services, "My.Magic")
  {
    ProvideOut(() => {
      var result = Enumerable.Range(1, AmountOfItems).Select(i => new {
        Title = "Hello from WithConfig #" + i,
        FavoriteColor,
      });
      return result;
    });
  }

  // This attribute [Configuration] creates a configuration "FavoriteColor"
  // In this example [Configuration] already knows about the fallback.
  // * The property getter calls Configuration.GetThis()
  // * GetThis() will automatically use the property name "FavoriteColor" to look up the config
  // * Calling an empty GetThis() will always return a string,
  //   so it's ideal for string-properties where the Fallback was specified before
  [Configuration(Fallback = "magenta")]
  public string FavoriteColor { get { return Configuration.GetThis(); } }

  // This attribute [Configuration] creates configuration "AmountOfItems"
  // * The property getter calls Configuration.GetThis()
  // * GetThis() will automatically use the property name "AmountOfItems" to look up the config
  //   and will use the default value of 1 if it was not specified
  [Configuration]
  public int AmountOfItems { get { return Configuration.GetThis(1); } }
}
