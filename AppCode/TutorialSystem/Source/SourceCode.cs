using ToSic.Eav.Data;
using ToSic.Razor.Blade;
using System.Linq;
using AppCode.TutorialSystem.Wrappers;
using AppCode.TutorialSystem.Sections;
using AppCode.Data;
using AppCode.TutorialSystem.Tabs;

namespace AppCode.TutorialSystem.Source
{
  public class SourceCode: AppCode.Services.ServiceBase
  {
    #region Init / Dependencies

    private TabSpecsFactory TabSpecsFactory => _tsFactory ??= GetService<TabSpecsFactory>();
    private TabSpecsFactory _tsFactory;

    #endregion

    #region All Available Entry Points to Show Snippets in Various Ways

    /// <summary>
    /// Create a Snip/Section object from a TutorialSnippet.
    /// Only used in Accordion
    /// </summary>
    /// <param name="item">The configuration item</param>
    /// <param name="file">The file from which it will be relative to</param>
    /// <returns></returns>
    public TutorialSectionEngine SnipFromItem(TutorialSnippet item, string codeFile)
    {
      var l = Log.Call<TutorialSectionEngine>($"{nameof(codeFile)}: {codeFile}");
      // If we have a file, we should try to look up the tabs
      Log.Add("tabs before:" + codeFile);
      var tabCsv = TabSpecsFactory.TryToGetTabsFromSource(codeFile);
      foreach (var tab in tabCsv)
        Log.Add("tabs: '" + tab + "'");
      Log.Add("tabs: '" + tabCsv + "'");
      var result = GetService<TutorialSectionEngine>().Init(this, item, tabCsv, sourceFile: codeFile);
      return l(result, "ok - count: " + tabCsv.Count());
    }

    #endregion


    #region Counter / Identifiers

    /// <summary>
    /// Count of source code snippets - used to create unique IDs
    /// </summary>
    public int SourceCodeTabCount = 0;

    #endregion

    #region Wrap: Source Wrappers like Wrap, WrapOutOverSrc, WrapOutOnly, WrapSrcOnly, WrapOutSplitSrc, WrapFormula

    internal Wrap GetSourceWrap(TutorialSectionEngine section, TutorialSnippet item) {

      // No item - unsure how this could happen, probably never possible?
      if (item == null)
        return new WrapOutOverSrc(section);

      if (item.TutorialType == "formula")
        return new WrapFormula(section);

      // Figure out the type based on the item or it's parent
      string code = null;
      if (item.IsNotEmpty(nameof(item.OutputAndSourceDisplay)))
        code = item.OutputAndSourceDisplay;
      else {
        var parent = item.Parent<TutorialGroup>();
        if (parent?.IsNotEmpty(nameof(parent.OutputAndSourceDisplay)) == true)
          code = parent.OutputAndSourceDisplay;
      }

      // Default Output over Source
      if (!code.Has() || code == "out-over-src")
        return new WrapOutOverSrc(section);

      // Basic src / out only
      if (code == "src") return new WrapSrcOnly(section);
      if (code == "out") return new WrapOutOnly(section);

      // Split - either a real split, or if width == 0, then 2 tabs
      if (code == "split") {
        if (item.Int("OutputWidth") != 0)
          return new WrapOutSplitSrc(section);
        return new Wrap(section, "WrapInsteadOfSplit", selectSkip: 1);
      }
      
      // SourceWrapIntro
      // SourceWrapIntroWithSource
      return new Wrap(section, "UnknownWrapperAutoDefault");
    }

    #endregion

  }
}