using System.Collections.Generic;

public class TreeChildPaths : Custom.DataSource.DataSource16
{
  public TreeChildPaths(MyServices services) : base(services, "My.Magic")
  {
    ProvideOut(() => {
      return new List<object> {
        // Root has path "/" and points to 2 children
        CreateNode("/", "Root Node", new [] { "/101", "/102" }),
        // This is a subnode, with 2 more children
        CreateNode("/101", "Sub Item 101", new [] { "/101/1011", "/101/1012" }),
        CreateNode("/102", "Sub Item 102", null),
        CreateNode("/101/1011", "Sub Item 1011", null),
        CreateNode("/101/1012", "Sub Item 1012", null),
      };
    });
  }


  private object CreateNode(string path, string title, string[] relationships = null) {
    return new {
      Title = title,
      Path = path,
      // To reference another item in the same list,
      // create an anonymous object with "Relationships" as an Enumerable, Array or List
      SubItems = new { Relationships = relationships },
      RelationshipKeys = new [] { path },
    };
  }
}
