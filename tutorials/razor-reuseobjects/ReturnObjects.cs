using System.Collections.Generic;
using ToSic.Sxc.Data;

/// <summary>
/// Demonstrates how to return objects, for tutorials which show how to use them.
/// </summary>
public class ReturnObjects: Custom.Hybrid.CodeTyped {
  public object GetAnonymous() {
    return new {
      Name = "John",
      Age = 30
    };
  }

  public ITyped GetTyped() {
    return AsTyped(new {
      Name = "John",
      Age = 30
    });
  }

  public Person GetPerson() {
    return new Person() {
      Name = "John",
      Age = 30
    };
  }

  public Dictionary<string, string> GetDictionary() {
    return new Dictionary<string, string>() {
      { "Name", "John" },
      { "Age", "30" }
    };
  }

  public class Person {
    public string Name { get; set; }
    public int Age { get; set; }
  }  
}
