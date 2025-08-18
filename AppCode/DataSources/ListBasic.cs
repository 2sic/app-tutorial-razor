// This sample uses some LINQ
using System.Linq;

public class ListBasic : Custom.DataSource.DataSource16
{
  public ListBasic(Dependencies services) : base(services)
  {
    ProvideOut(() =>
      // For demo, create a few of these items using numbers 1 to 5
      Enumerable.Range(1, 5).Select(i => new {
        // Property with name "Id" is automatically used for the EntityId
        Id = i,
        // Property with name "Guid" is automatically used for the EntityGuid
        Guid = System.Guid.NewGuid(),
        Title = "Hello from ListBasic",
        FavoriteNumber = 2742,
      })
    );
  }
}
