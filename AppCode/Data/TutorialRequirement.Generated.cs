// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "TutorialRequirement.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class TutorialRequirement
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   CSharpDataModelsGenerator v18.04.00
// App/Edition: Tutorial-Razor/
// User:        2sic Web-Developer
using System.Text.Json.Serialization;
using ToSic.Sxc.Adam;

namespace AppCode.Data
{
  // This is a generated class for TutorialRequirement 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// TutorialRequirement data. <br/>
  /// Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.Link`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class TutorialRequirement: AutoGenerated.ZAutoGenTutorialRequirement
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.TutorialRequirement in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZAutoGenTutorialRequirement: Custom.Data.CustomItem
  {
    /// <summary>
    /// Link as link (url). <br/>
    /// To get the underlying value like 'file:72' use String("Link")
    /// </summary>
    public string Link => _item.Url("Link");

    /// <summary>
    /// Get the file object for Link - or null if it's empty or not referencing a file.
    /// </summary>

    [JsonIgnore]
    public IFile LinkFile => _item.File("Link");

    /// <summary>
    /// Get the folder object for Link.
    /// </summary>

    [JsonIgnore]
    public IFolder LinkFolder => _item.Folder("Link");

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
  }
}