// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "TutorialNote.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class TutorialNote
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
  // This is a generated class for TutorialNote 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// TutorialNote data. <br/>
  /// Generated 2024-05-21 19:46:38Z. Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.CshtmlFile`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class TutorialNote: AutoGenerated.ZAutoGenTutorialNote
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.TutorialNote in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZAutoGenTutorialNote: Custom.Data.CustomItem
  {
    /// <summary>
    /// CshtmlFile as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("CshtmlFile", scrubHtml: true) etc.
    /// </summary>
    public string CshtmlFile => _item.String("CshtmlFile", fallback: "");

    /// <summary>
    /// Images as link (url). <br/>
    /// To get the underlying value like 'file:72' use String("Images")
    /// </summary>
    public string Images => _item.Url("Images");

    /// <summary>
    /// Get the file object for Images - or null if it's empty or not referencing a file.
    /// </summary>

    [JsonIgnore]
    public IFile ImagesFile => _item.File("Images");

    /// <summary>
    /// Get the folder object for Images.
    /// </summary>

    [JsonIgnore]
    public IFolder ImagesFolder => _item.Folder("Images");

    /// <summary>
    /// NameId as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("NameId", scrubHtml: true) etc.
    /// </summary>
    public string NameId => _item.String("NameId", fallback: "");

    /// <summary>
    /// Note as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Note", scrubHtml: true) etc.
    /// </summary>
    public string Note => _item.String("Note", fallback: "");

    /// <summary>
    /// NoteSource as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("NoteSource", scrubHtml: true) etc.
    /// </summary>
    public string NoteSource => _item.String("NoteSource", fallback: "");

    /// <summary>
    /// NoteType as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("NoteType", scrubHtml: true) etc.
    /// </summary>
    public string NoteType => _item.String("NoteType", fallback: "");

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