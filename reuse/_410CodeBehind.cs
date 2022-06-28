using ToSic.Razor.Blade;
using System.Linq;
using System;

public class _410CodeBehind: Custom.Hybrid.Code14
{
  public string Hello() {
    return "Hello from inner code";
  }

  // This is an example of a helper returning HTML which will be rendered directly
  // You could also do: return Html.Raw(...);
  public dynamic Message(string message) {
    return Tag.Div(message).Class("alert alert-success");
  }
}