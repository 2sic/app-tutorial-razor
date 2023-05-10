using System.Collections.Generic;

public class TreeBasic : Custom.DataSource.DataSource16
{
  public TreeBasic(MyServices services) : base(services, "My.Magic")
  {
    ProvideOut(() => {     
      return new List<object> {
        // Root has ID 1 and points to 2 children
        CreateNode(1, "Root Node", new [] { 101, 102 }),
        // This is a subnode, with 2 more children
        CreateNode(101, "Sub Item 101", new [] { 1011, 1012 }),
        CreateNode(102, "Sub Item 102"),
        CreateNode(1011, "Sub Item 1011"),
        CreateNode(1012, "Sub Item 1012"),
      };
    });
  }

  private object CreateNode(int id, string title, int[] relationships = null) {
    return new {
      Id = id,
      Title = title,
      // To reference another item in the same list,
      // create an anonymous object with "Relationships" as an Enumerable, Array or List
      SubItems = new { Relationships = relationships }
    };
  }
}
