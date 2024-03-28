using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using System;
using System.Collections.Generic;
using static AppCode.TutorialSystem.Constants;
using AppCode.TutorialSystem.Sections;
using AppCode.Data;


namespace AppCode.TutorialSystem.Wrappers
{
  internal class WrapFormula: Wrap
    {
      public WrapFormula(TutorialSectionEngine sb) /*, TutorialSnippet specs = null) */ : base(sb, "WrapFormula") {
        FormulaSpecs = /* specs ?? */ Section.Item.Formula; //.Child<TutorialSnippet>("Formula");
        if (FormulaSpecs == null) throw new Exception("Formula section needs a Formula item");
        Tabs = new List<string> { ResultTabName, FormulasTabName };
        TabSelected = ResultTabName;
      }
      public TutFormulaSample FormulaSpecs;

      public override ITag OutputOpen() {
        // Activate toolbar for anonymous so it will always work in demo-mode
        Section.ToolbarHelpers.EnableEditForAll();
        return Tag.RawHtml(
          Section.Formulas.Intro(FormulaSpecs)
        );
      }
    }
}