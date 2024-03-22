// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "TutorialGroup.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class TutorialGroup
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   CSharpDataModelsGenerator v17.05.00
// App/Edition: Tutorial-Razor/
// User:        2sic Web-Developer
// When:        2024-03-22 17:00:25Z
using System.Collections.Generic;
using ToSic.Sxc.Adam;

namespace AppCode.Data
{
  // This is a generated class for TutorialGroup 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// TutorialGroup data. <br/>
  /// Generated 2024-03-22 17:00:25Z. Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.Accordions`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class TutorialGroup: AutoGenerated.ZagTutorialGroup
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.TutorialGroup in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZagTutorialGroup: Custom.Data.CustomItem
  {
    /// <summary>
    /// Accordions as list of TutorialGroup.
    /// </summary>
    /// <remarks>
    /// Generated to return child-list child because field settings had Multi-Value=true. The type TutorialGroup was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// An IEnumerable of specified type, but can be empty.
    /// </returns>
    public IEnumerable<TutorialGroup> Accordions => _accordions ??= _item.Children<TutorialGroup>("Accordions");
    private IEnumerable<TutorialGroup> _accordions;

    /// <summary>
    /// DefaultStateIsOpen as bool. <br/>
    /// To get nullable use .Get("DefaultStateIsOpen") as bool?;
    /// </summary>
    public bool DefaultStateIsOpen => _item.Bool("DefaultStateIsOpen");

    /// <summary>
    /// HasVariants as bool. <br/>
    /// To get nullable use .Get("HasVariants") as bool?;
    /// </summary>
    public bool HasVariants => _item.Bool("HasVariants");

    /// <summary>
    /// Intro as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Intro", scrubHtml: true) etc.
    /// </summary>
    public string Intro => _item.String("Intro", fallback: "");

    /// <summary>
    /// IntroMoreDyn as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("IntroMoreDyn", scrubHtml: true) etc.
    /// </summary>
    public string IntroMoreDyn => _item.String("IntroMoreDyn", fallback: "");

    /// <summary>
    /// IntroMoreTyped as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("IntroMoreTyped", scrubHtml: true) etc.
    /// </summary>
    public string IntroMoreTyped => _item.String("IntroMoreTyped", fallback: "");

    /// <summary>
    /// IsPage as bool. <br/>
    /// To get nullable use .Get("IsPage") as bool?;
    /// </summary>
    public bool IsPage => _item.Bool("IsPage");

    /// <summary>
    /// LinkEmphasis as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("LinkEmphasis", scrubHtml: true) etc.
    /// </summary>
    public string LinkEmphasis => _item.String("LinkEmphasis", fallback: "");

    /// <summary>
    /// LinkTeaser as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("LinkTeaser", scrubHtml: true) etc.
    /// </summary>
    public string LinkTeaser => _item.String("LinkTeaser", fallback: "");

    /// <summary>
    /// LinkTitle as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("LinkTitle", scrubHtml: true) etc.
    /// </summary>
    public string LinkTitle => _item.String("LinkTitle", fallback: "");

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
    /// Notes as list of TutorialNote.
    /// </summary>
    /// <remarks>
    /// Generated to return child-list child because field settings had Multi-Value=true. The type TutorialNote was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// An IEnumerable of specified type, but can be empty.
    /// </returns>
    public IEnumerable<TutorialNote> Notes => _notes ??= _item.Children<TutorialNote>("Notes");
    private IEnumerable<TutorialNote> _notes;

    /// <summary>
    /// OutputAndSourceDisplay as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("OutputAndSourceDisplay", scrubHtml: true) etc.
    /// </summary>
    public string OutputAndSourceDisplay => _item.String("OutputAndSourceDisplay", fallback: "");

    /// <summary>
    /// RefSharedSpecs as single item of TutorialSpecs.
    /// </summary>
    /// <remarks>
    /// Generated to only return 1 child because field settings had Multi-Value=false. The type TutorialSpecs was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// A single item OR null if nothing found, so you can use ?? to provide alternate objects.
    /// </returns>
    public TutorialSpecs RefSharedSpecs => _refSharedSpecs ??= _item.Child<TutorialSpecs>("RefSharedSpecs");
    private TutorialSpecs _refSharedSpecs;

    /// <summary>
    /// RefTutorials as list of TutorialGroup.
    /// </summary>
    /// <remarks>
    /// Generated to return child-list child because field settings had Multi-Value=true. The type TutorialGroup was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// An IEnumerable of specified type, but can be empty.
    /// </returns>
    public IEnumerable<TutorialGroup> RefTutorials => _refTutorials ??= _item.Children<TutorialGroup>("RefTutorials");
    private IEnumerable<TutorialGroup> _refTutorials;

    /// <summary>
    /// Sections as list of TutorialSnippet.
    /// </summary>
    /// <remarks>
    /// Generated to return child-list child because field settings had Multi-Value=true. The type TutorialSnippet was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// An IEnumerable of specified type, but can be empty.
    /// </returns>
    public IEnumerable<TutorialSnippet> Sections => _sections ??= _item.Children<TutorialSnippet>("Sections");
    private IEnumerable<TutorialSnippet> _sections;

    /// <summary>
    /// ShareImage as link (url). <br/>
    /// To get the underlying value like 'file:72' use String("ShareImage")
    /// </summary>
    public string ShareImage => _item.Url("ShareImage");

    /// <summary>
    /// Get the file object for ShareImage - or null if it's empty or not referencing a file.
    /// </summary>
    public IFile ShareImageFile => _item.File("ShareImage");

    /// <summary>
    /// Get the folder object for ShareImage.
    /// </summary>
    public IFolder ShareImageFolder => _item.Folder("ShareImage");

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

    /// <summary>
    /// Variants as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Variants", scrubHtml: true) etc.
    /// </summary>
    public string Variants => _item.String("Variants", fallback: "");

    /// <summary>
    /// XxxLogo as link (url). <br/>
    /// To get the underlying value like 'file:72' use String("XxxLogo")
    /// </summary>
    public string XxxLogo => _item.Url("XxxLogo");

    /// <summary>
    /// Get the file object for XxxLogo - or null if it's empty or not referencing a file.
    /// </summary>
    public IFile XxxLogoFile => _item.File("XxxLogo");

    /// <summary>
    /// Get the folder object for XxxLogo.
    /// </summary>
    public IFolder XxxLogoFolder => _item.Folder("XxxLogo");
  }
}