using System.Linq;            // This is for the Enumerable.Range()
using ToSic.Eav.Data.Build;   // This is for the FactoryOptions

public class FactoryOptions : Custom.DataSource.DataSource16
{
  public FactoryOptions(MyServices services) : base(services)
  {
    ProvideOut(() =>
      // For demo, create a few of these items
      Enumerable.Range(0, 4).Select(i => new {
        // We don't provide an ID, which will result in an id=0 which will auto-id
        // Id = i,
        Guid = System.Guid.NewGuid(),
        // No more Title, now it's Greeting - which wouldn't auto-map to EntityTitle
        Greeting = "Greeting from FactoryOptions",
      }),
      // Set various options for how the factory will generate the Entities
      options: () => new DataFactoryOptions(/* typeName: "MyContentType", titleField: "Greeting", idSeed: 1000 */) {
        TypeName = "MyContentType",
        TitleField = "Greeting",
        IdSeed = 1000,
      }
    );
  }
}
