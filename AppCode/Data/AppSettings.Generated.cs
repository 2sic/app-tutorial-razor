// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "AppSettings.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class AppSettings
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   CSharpDataModelsGenerator v18.03.00
// App/Edition: Tutorial-Razor/
// User:        2sic Web-Developer
using System.Text.Json.Serialization;
using ToSic.Sxc.Adam;

namespace AppCode.Data
{
  // This is a generated class for AppSettings (scope: System.App)
  // To extend/modify it, see instructions above.

  /// <summary>
  /// AppSettings data. <br/>
  /// Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.CsvFile`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  /// <remarks>
  /// This Content-Type is NOT in the default scope, so you may not see it in the Admin UI. It's in the scope System.App.
  /// </remarks>
  public partial class AppSettings: AutoGenerated.ZAutoGenAppSettings
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for System.App.AppSettings in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZAutoGenAppSettings: Custom.Data.CustomItem
  {
    /// <summary>
    /// CsvFile as link (url). <br/>
    /// To get the underlying value like 'file:72' use String("CsvFile")
    /// </summary>
    public string CsvFile => _item.Url("CsvFile");

    /// <summary>
    /// Get the file object for CsvFile - or null if it's empty or not referencing a file.
    /// </summary>

    [JsonIgnore]
    public IFile CsvFileFile => _item.File("CsvFile");

    /// <summary>
    /// Get the folder object for CsvFile.
    /// </summary>

    [JsonIgnore]
    public IFolder CsvFileFolder => _item.Folder("CsvFile");

    /// <summary>
    /// CustomColor as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("CustomColor", scrubHtml: true) etc.
    /// </summary>
    public string CustomColor => _item.String("CustomColor", fallback: "");

    /// <summary>
    /// FormulaDemoShowAdvanced as bool. <br/>
    /// To get nullable use .Get("FormulaDemoShowAdvanced") as bool?;
    /// </summary>
    public bool FormulaDemoShowAdvanced => _item.Bool("FormulaDemoShowAdvanced");

    /// <summary>
    /// PrimaryColor as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("PrimaryColor", scrubHtml: true) etc.
    /// </summary>
    public string PrimaryColor => _item.String("PrimaryColor", fallback: "");

    /// <summary>
    /// QrBackgroundColor as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("QrBackgroundColor", scrubHtml: true) etc.
    /// </summary>
    public string QrBackgroundColor => _item.String("QrBackgroundColor", fallback: "");

    /// <summary>
    /// QrDimension as int. <br/>
    /// To get other types use methods such as .Decimal("QrDimension")
    /// </summary>
    public int QrDimension => _item.Int("QrDimension");

    /// <summary>
    /// QrEcc as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("QrEcc", scrubHtml: true) etc.
    /// </summary>
    public string QrEcc => _item.String("QrEcc", fallback: "");

    /// <summary>
    /// QrForegroundColor as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("QrForegroundColor", scrubHtml: true) etc.
    /// </summary>
    public string QrForegroundColor => _item.String("QrForegroundColor", fallback: "");

    /// <summary>
    /// SecondaryColor as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("SecondaryColor", scrubHtml: true) etc.
    /// </summary>
    public string SecondaryColor => _item.String("SecondaryColor", fallback: "");
  }
}