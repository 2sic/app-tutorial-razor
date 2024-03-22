// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "TutorialCredits.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class TutorialCredits
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   CSharpDataModelsGenerator v17.05.00
// App/Edition: Tutorial-Razor/
// User:        2sic Web-Developer
// When:        2024-03-22 17:00:25Z
namespace AppCode.Data
{
  // This is a generated class for TutorialCredits 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// TutorialCredits data. <br/>
  /// Generated 2024-03-22 17:00:25Z. Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.Credits`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class TutorialCredits: AutoGenerated.ZagTutorialCredits
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.TutorialCredits in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZagTutorialCredits: Custom.Data.CustomItem
  {
    /// <summary>
    /// Credits as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Credits", scrubHtml: true) etc.
    /// </summary>
    public string Credits => _item.String("Credits", fallback: "");

    /// <summary>
    /// NameId as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("NameId", scrubHtml: true) etc.
    /// </summary>
    public string NameId => _item.String("NameId", fallback: "");

    /// <summary>
    /// Title as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Title", scrubHtml: true) etc.
    /// </summary>
    /// <remarks>
    /// This hides base property Title.
    /// To access original, convert using AsItem(...) or cast to ITypedItem.
    /// Consider renaming this field in the underlying content-type.
    /// </remarks>
    public new string Title => _item.String("Title", fallback: "");

    /// <summary>
    /// TitleForPicker as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("TitleForPicker", scrubHtml: true) etc.
    /// </summary>
    public string TitleForPicker => _item.String("TitleForPicker", fallback: "");
  }
}