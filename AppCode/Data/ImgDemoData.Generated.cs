// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "ImgDemoData.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class ImgDemoData
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   CSharpDataModelsGenerator v17.08.00
// App/Edition: Tutorial-Razor/
// User:        2sic Web-Developer
// When:        2024-05-21 19:46:38Z
using System.Text.Json.Serialization;
using ToSic.Sxc.Adam;

namespace AppCode.Data
{
  // This is a generated class for ImgDemoData 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// ImgDemoData data. <br/>
  /// Generated 2024-05-21 19:46:38Z. Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.Image`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class ImgDemoData: AutoGenerated.ZAutoGenImgDemoData
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.ImgDemoData in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZAutoGenImgDemoData: Custom.Data.CustomItem
  {
    /// <summary>
    /// Image as link (url). <br/>
    /// To get the underlying value like 'file:72' use String("Image")
    /// </summary>
    public string Image => _item.Url("Image");

    /// <summary>
    /// Get the file object for Image - or null if it's empty or not referencing a file.
    /// </summary>

    [JsonIgnore]
    public IFile ImageFile => _item.File("Image");

    /// <summary>
    /// Get the folder object for Image.
    /// </summary>

    [JsonIgnore]
    public IFolder ImageFolder => _item.Folder("Image");

    /// <summary>
    /// NameId as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("NameId", scrubHtml: true) etc.
    /// </summary>
    public string NameId => _item.String("NameId", fallback: "");
  }
}