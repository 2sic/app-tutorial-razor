// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "Poets.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class Poets
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   CSharpDataModelsGenerator v17.06.02
// App/Edition: Tutorial-Razor/
// User:        2sic Web-Developer
// When:        2024-04-04 21:21:00Z
using System;

namespace AppCode.Data
{
  // This is a generated class for Poets 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// Poets data. <br/>
  /// Generated 2024-04-04 21:21:00Z. Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.BirthDate`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class Poets: AutoGenerated.ZagPoets
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.Poets in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZagPoets: Custom.Data.CustomItem
  {
    /// <summary>
    /// BirthDate as DateTime.
    /// </summary>
    public DateTime BirthDate => _item.DateTime("BirthDate");

    /// <summary>
    /// Name as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Name", scrubHtml: true) etc.
    /// </summary>
    public string Name => _item.String("Name", fallback: "");

    /// <summary>
    /// Poems as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Poems", scrubHtml: true) etc.
    /// </summary>
    public string Poems => _item.String("Poems", fallback: "");
  }
}