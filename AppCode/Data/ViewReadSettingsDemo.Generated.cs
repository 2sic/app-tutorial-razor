// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "ViewReadSettingsDemo.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class ViewReadSettingsDemo
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   CSharpDataModelsGenerator v18.04.00
// App/Edition: Tutorial-Razor/
// User:        2sic Web-Developer
namespace AppCode.Data
{
  // This is a generated class for ViewReadSettingsDemo (scope: System.Configuration)
  // To extend/modify it, see instructions above.

  /// <summary>
  /// ViewReadSettingsDemo data. <br/>
  /// Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.CustomColor`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  /// <remarks>
  /// This Content-Type is NOT in the default scope, so you may not see it in the Admin UI. It's in the scope System.Configuration.
  /// </remarks>
  public partial class ViewReadSettingsDemo: AutoGenerated.ZAutoGenViewReadSettingsDemo
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for System.Configuration.ViewReadSettingsDemo in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZAutoGenViewReadSettingsDemo: Custom.Data.CustomItem
  {
    /// <summary>
    /// CustomColor as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("CustomColor", scrubHtml: true) etc.
    /// </summary>
    public string CustomColor => _item.String("CustomColor", fallback: "");

    /// <summary>
    /// PrimaryColor as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("PrimaryColor", scrubHtml: true) etc.
    /// </summary>
    public string PrimaryColor => _item.String("PrimaryColor", fallback: "");

    /// <summary>
    /// SecondaryColor as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("SecondaryColor", scrubHtml: true) etc.
    /// </summary>
    public string SecondaryColor => _item.String("SecondaryColor", fallback: "");

    /// <summary>
    /// Tiles as int. <br/>
    /// To get other types use methods such as .Decimal("Tiles")
    /// </summary>
    public int Tiles => _item.Int("Tiles");
  }
}