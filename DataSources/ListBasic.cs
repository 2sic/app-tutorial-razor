// This sample uses some LINQ
using System.Linq;

public class ListBasic : Custom.DataSource.DataSource15
{
  public ListBasic(MyServices services) : base(services)
  {
    ProvideOut(() => Enumerable
      // For demo, create a few of these items
      .Range(1, 5)
      .Select(i => new {
        // Property with name "Id" is automatically used for the EntityId
        Id = i,
        guid = System.Guid.NewGuid(),
        Title = "Hello from ListBasic",
        FavoriteNumber = 2742,
      })
    );
  }
}
