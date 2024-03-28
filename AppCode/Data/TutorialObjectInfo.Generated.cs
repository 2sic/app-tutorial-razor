// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "TutorialObjectInfo.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class TutorialObjectInfo
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   CSharpDataModelsGenerator v17.06.00
// App/Edition: Tutorial-Razor/
// User:        2sic Web-Developer
// When:        2024-03-28 18:17:21Z
using ToSic.Sxc.Adam;

namespace AppCode.Data
{
  // This is a generated class for TutorialObjectInfo 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// TutorialObjectInfo data. <br/>
  /// Generated 2024-03-28 18:17:21Z. Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.Description`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class TutorialObjectInfo: AutoGenerated.ZagTutorialObjectInfo
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.TutorialObjectInfo in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZagTutorialObjectInfo: Custom.Data.CustomItem
  {
    /// <summary>
    /// Description as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Description", scrubHtml: true) etc.
    /// </summary>
    public string Description => _item.String("Description", fallback: "");

    /// <summary>
    /// Images as link (url). <br/>
    /// To get the underlying value like 'file:72' use String("Images")
    /// </summary>
    public string Images => _item.Url("Images");

    /// <summary>
    /// Get the file object for Images - or null if it's empty or not referencing a file.
    /// </summary>
    public IFile ImagesFile => _item.File("Images");

    /// <summary>
    /// Get the folder object for Images.
    /// </summary>
    public IFolder ImagesFolder => _item.Folder("Images");

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
  }
}