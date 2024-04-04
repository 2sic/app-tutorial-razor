// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "TutorialSection.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class TutorialSection
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   CSharpDataModelsGenerator v17.06.02
// App/Edition: Tutorial-Razor/
// User:        2sic Web-Developer
// When:        2024-04-04 20:28:06Z
using System.Collections.Generic;
using ToSic.Sxc.Data;

namespace AppCode.Data
{
  // This is a generated class for TutorialSection 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// TutorialSection data. <br/>
  /// Generated 2024-04-04 20:28:06Z. Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.Category`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class TutorialSection: AutoGenerated.ZagTutorialSection
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.TutorialSection in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZagTutorialSection: Custom.Data.CustomItem
  {
    /// <summary>
    /// Category as single item of TutorialCategory.
    /// </summary>
    /// <remarks>
    /// Generated to only return 1 child because field settings had Multi-Value=false. The type TutorialCategory was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// A single item OR null if nothing found, so you can use ?? to provide alternate objects.
    /// </returns>
    public TutorialCategory Category => _category ??= _item.Child<TutorialCategory>("Category");
    private TutorialCategory _category;

    /// <summary>
    /// FeatureSet as single item of TutorialFeatureSet.
    /// </summary>
    /// <remarks>
    /// Generated to only return 1 child because field settings had Multi-Value=false. The type TutorialFeatureSet was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// A single item OR null if nothing found, so you can use ?? to provide alternate objects.
    /// </returns>
    public TutorialFeatureSet FeatureSet => _featureSet ??= _item.Child<TutorialFeatureSet>("FeatureSet");
    private TutorialFeatureSet _featureSet;

    /// <summary>
    /// Introduction as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Introduction", scrubHtml: true) etc.
    /// </summary>
    public string Introduction => _item.String("Introduction", fallback: "");

    /// <summary>
    /// NameId as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("NameId", scrubHtml: true) etc.
    /// </summary>
    public string NameId => _item.String("NameId", fallback: "");

    /// <summary>
    /// RefRequirements as list of TutorialRequirement.
    /// </summary>
    /// <remarks>
    /// Generated to return child-list child because field settings had Multi-Value=true. The type TutorialRequirement was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// An IEnumerable of specified type, but can be empty.
    /// </returns>
    public IEnumerable<TutorialRequirement> RefRequirements => _refRequirements ??= _item.Children<TutorialRequirement>("RefRequirements");
    private IEnumerable<TutorialRequirement> _refRequirements;

    /// <summary>
    /// RefResources as list of TutorialResource.
    /// </summary>
    /// <remarks>
    /// Generated to return child-list child because field settings had Multi-Value=true. The type TutorialResource was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// An IEnumerable of specified type, but can be empty.
    /// </returns>
    public IEnumerable<TutorialResource> RefResources => _refResources ??= _item.Children<TutorialResource>("RefResources");
    private IEnumerable<TutorialResource> _refResources;

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
    /// SegmentName as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("SegmentName", scrubHtml: true) etc.
    /// </summary>
    public string SegmentName => _item.String("SegmentName", fallback: "");

    /// <summary>
    /// SegmentTitle as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("SegmentTitle", scrubHtml: true) etc.
    /// </summary>
    public string SegmentTitle => _item.String("SegmentTitle", fallback: "");

    /// <summary>
    /// Subsection as bool. <br/>
    /// To get nullable use .Get("Subsection") as bool?;
    /// </summary>
    public bool Subsection => _item.Bool("Subsection");

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
    /// TutorialGroups as list of TutorialGroup.
    /// </summary>
    /// <remarks>
    /// Generated to return child-list child because field settings had Multi-Value=true. The type TutorialGroup was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// An IEnumerable of specified type, but can be empty.
    /// </returns>
    public IEnumerable<TutorialGroup> TutorialGroups => _tutorialGroups ??= _item.Children<TutorialGroup>("TutorialGroups");
    private IEnumerable<TutorialGroup> _tutorialGroups;

    /// <summary>
    /// Views as list of ITypedItem.
    /// </summary>
    /// <remarks>
    /// Generated to return child-list child because field settings had Multi-Value=true. 
    /// </remarks>
    /// <returns>
    /// An IEnumerable of specified type, but can be empty.
    /// </returns>
    public IEnumerable<ITypedItem> Views => _views ??= _item.Children("Views");
    private IEnumerable<ITypedItem> _views;
  }
}