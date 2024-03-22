// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// All the classes are partial, so you can extend them in a separate file.

// Generator:   CSharpServicesGenerator v17.05.00
// App/Edition: Tutorial-Razor/
// User:        2sic Web-Developer
// When:        2024-03-22 17:04:47Z

using AppCode.Data;
using ToSic.Sxc.Apps;

namespace AppCode.Services
{
  /// <summary>
  /// Base Class for Services which have a typed App.
  /// </summary>
  public abstract partial class ServiceBase: Custom.Hybrid.CodeTyped
  {

    /// <summary>
    /// Typed App with typed Settings & Resources
    /// </summary>
    public new IAppTyped<AppSettings, AppResources> App => _app ??= Customize.App<AppSettings, AppResources>();
    private IAppTyped<AppSettings, AppResources> _app;
  }
}
