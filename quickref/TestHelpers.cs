using System;
using ToSic.Razor.Blade;
public class TestHelpers: Custom.Hybrid.CodeTyped
{
  public object TryGet(Func<object> func) {
    try {
      return func();
    } catch (Exception ex) {
      return "⚠️ Exception: " + ex.Message;
    }
  }
}
