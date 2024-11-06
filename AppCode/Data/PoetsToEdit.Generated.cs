// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "PoetsToEdit.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class PoetsToEdit
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   CSharpDataModelsGenerator v18.04.00
// App/Edition: Tutorial-Razor/
// User:        2sic Web-Developer
using System;

namespace AppCode.Data
{
  // This is a generated class for PoetsToEdit 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// PoetsToEdit data. <br/>
  /// Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.BirthDate`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class PoetsToEdit: AutoGenerated.ZAutoGenPoetsToEdit
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.PoetsToEdit in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZAutoGenPoetsToEdit: Custom.Data.CustomItem
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
    /// Poems as int. <br/>
    /// To get other types use methods such as .Decimal("Poems")
    /// </summary>
    public int Poems => _item.Int("Poems");
  }
}