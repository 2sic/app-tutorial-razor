using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using System;
using System.Collections.Generic;
using AppCode.TutorialSystem.Sections;
using AppCode.Data;
using AppCode.TutorialSystem.Tabs;
using static AppCode.TutorialSystem.Constants;
using System.Linq;


namespace AppCode.TutorialSystem.Wrappers
{
  internal class WrapFormula: Wrap
    {
      public WrapFormula(TutorialSectionEngine sb) : base(sb, "WrapFormula",
        tabSpecs: new List<TabSpecs> { new TabSpecs(TabType.Results, ResultTabName), new TabSpecs(TabType.Formulas, FormulasTabName) }
      )
      {
        FormulaSpecs = Section.Item.Formula ?? throw new Exception("Formula section needs a Formula item");
      }
      public TutorialEditUiFormula FormulaSpecs;

      public override ITag OutputOpen() {
        // Activate toolbar for anonymous so it will always work in demo-mode
        Section.ToolbarHelpers.EnableEditForAll();
        return Tag.RawHtml(
          Section.Formulas.Intro(FormulaSpecs)
        );
      }
    }
}