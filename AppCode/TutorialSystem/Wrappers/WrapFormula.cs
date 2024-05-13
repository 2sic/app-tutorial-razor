using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using System;
using System.Collections.Generic;
using AppCode.TutorialSystem.Sections;
using AppCode.Data;
using AppCode.TutorialSystem.Tabs;


namespace AppCode.TutorialSystem.Wrappers
{
  internal class WrapFormula: Wrap
    {
      public WrapFormula(TutorialSectionEngine sb) : base(sb, "WrapFormula",
        tabSpecs: new List<TabSpecs> { TabSpecsFactory.Results(), TabSpecsFactory.Formulas() }
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