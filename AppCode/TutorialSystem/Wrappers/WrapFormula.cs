using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Data;
using System;
using System.Collections.Generic;
using static AppCode.TutorialSystem.Constants;
using AppCode.TutorialSystem.Sections;


namespace AppCode.TutorialSystem.Wrappers
{
  internal class WrapFormula: Wrap
    {
      public WrapFormula(TutorialSectionEngine sb, ITypedItem specs = null) : base(sb, "WrapFormula") {
        FormulaSpecs = specs ?? Section.Item.Child("Formula");
        if (FormulaSpecs == null) throw new Exception("Formula section needs a Formula item");
        Tabs = new List<string> { ResultTabName, FormulasTabName };
        TabSelected = ResultTabName;
      }
      public ITypedItem FormulaSpecs;

      public override ITag OutputOpen() {
        // Activate toolbar for anonymous so it will always work in demo-mode
        Section.ScParent.Sys.ToolbarHelpers.EnableEditForAll();
        return Tag.RawHtml(
          Section.ScParent.Formulas.Intro(FormulaSpecs)
        );
      }
    }
}