// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "TutorialFeatureSet.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class TutorialFeatureSet
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
  // This is a generated class for TutorialFeatureSet 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// TutorialFeatureSet data. <br/>
  /// Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.Icon`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class TutorialFeatureSet: AutoGenerated.ZAutoGenTutorialFeatureSet
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.TutorialFeatureSet in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZAutoGenTutorialFeatureSet: Custom.Data.CustomItem
  {
    /// <summary>
    /// Icon as link (url). <br/>
    /// To get the underlying value like 'file:72' use String("Icon")
    /// </summary>
    public string Icon => _item.Url("Icon");

    /// <summary>
    /// Get the file object for Icon - or null if it's empty or not referencing a file.
    /// </summary>

    [JsonIgnore]
    public IFile IconFile => _item.File("Icon");

    /// <summary>
    /// Get the folder object for Icon.
    /// </summary>

    [JsonIgnore]
    public IFolder IconFolder => _item.Folder("Icon");

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
    /// LogoBanner as link (url). <br/>
    /// To get the underlying value like 'file:72' use String("LogoBanner")
    /// </summary>
    public string LogoBanner => _item.Url("LogoBanner");

    /// <summary>
    /// Get the file object for LogoBanner - or null if it's empty or not referencing a file.
    /// </summary>

    [JsonIgnore]
    public IFile LogoBannerFile => _item.File("LogoBanner");

    /// <summary>
    /// Get the folder object for LogoBanner.
    /// </summary>

    [JsonIgnore]
    public IFolder LogoBannerFolder => _item.Folder("LogoBanner");

    /// <summary>
    /// NameId as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("NameId", scrubHtml: true) etc.
    /// </summary>
    public string NameId => _item.String("NameId", fallback: "");
  }
}