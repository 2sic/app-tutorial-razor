using System.Collections.Generic;

public class TreeParentPaths : Custom.DataSource.DataSource16
{
  public TreeParentPaths(MyServices services) : base(services, "My.Magic")
  {
    ProvideOut(() => {
      return new List<object> {
        // Root has ID 1 and points to 2 children
        CreateNode("/", "Root Node"),
        // This is a subnode, with 2 more children
        CreateNode("/101", "Sub Item 101", "/"),
        CreateNode("/102", "Sub Item 102", "/"),
        CreateNode("/101/1011", "Sub Item 1011", "/101"),
        CreateNode("/101/1012", "Sub Item 1012", "/101"),
      };
    });
  }


  private object CreateNode(string path, string title, string parent = null) {
    return new {
      Title = title,
      Path = path,
      // This says that the sub-items all use the key
      // of the current item because they point to the parent
      // so the child points to the parent, not the parent to the child
      SubItems = new { Relationships = path },
      RelationshipKeys = new [] { parent },
    };
  }
}
