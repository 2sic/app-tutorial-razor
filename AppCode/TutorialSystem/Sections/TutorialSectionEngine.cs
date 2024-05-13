using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using AppCode.Output;
using AppCode.TutorialSystem.Wrappers;
using AppCode.TutorialSystem.Tabs;
using AppCode.TutorialSystem.Source;
using AppCode.Data;
using static AppCode.TutorialSystem.Constants;

namespace AppCode.TutorialSystem.Sections
{
  /// <summary>
  /// Engine to generate all kinds of tutorial sections within wrappers.
  /// </summary>
  public class TutorialSectionEngine: AppCode.Services.ServiceBase
  {
    public TutorialSectionEngine Init(SourceCode sourceCode, TutorialSnippet item, List<TabSpecs> tabSpecs, string sourceFile = null) {
      Item = item ?? throw new Exception("Item should never be null");
      ScParent = sourceCode;
      BsTabs = GetService<BootstrapTabs>();
      SourceWrap = sourceCode.GetSourceWrap(this, item);
      TabHandler = new TabManager(sourceCode, item, tabSpecs, sourceWrap: SourceWrap);
      SourceFile = sourceFile;
      ViewConfig = GetService<ViewConfigurationSimulation>().Setup(TabHandler);
      return this;
    }

    internal SourceCode ScParent;

    private FancyboxService Fancybox => _fancybox ??= GetService<FancyboxService>();
    private FancyboxService _fancybox;

    private FileHandler FileHandler => _fileHandler ??= GetService<FileHandler>();
    private FileHandler _fileHandler;

    // internal so it can be used by wrappers
    internal SourceCodeFormulas Formulas => _formulas ??= GetService<SourceCodeFormulas>();
    private SourceCodeFormulas _formulas;

    // internal so it can be used by wrappers
    internal ToolbarHelpers ToolbarHelpers => _tlbHelpers ??= GetService<ToolbarHelpers>();
    private ToolbarHelpers _tlbHelpers;

    private BootstrapTabs BsTabs;
    public string TabPrefix {get; protected set;}
    protected Wrap SourceWrap;
    internal TabManager TabHandler;
    internal TutorialSnippet Item;
    internal string SourceFile;
    public ViewConfigurationSimulation ViewConfig;

    /// <summary>
    /// ...
    /// </summary>
    public ITag SnipStart() {
      // note: not sure if the snippet-count is even relevant, could be an old leftover
      var snippetCount = ScParent.SourceCodeTabCount++;
      // The unique key should be unique for each instance of this class
      var uniqueKey = Kit.Key.UniqueKeyWith(this);
      TabPrefix = $"tab-{uniqueKey}-{snippetCount}-auto-id";
      return TabsBeforeContent();
    }

    /// <summary>
    /// Public method to replace the tab contents from outside - typically used in img-tutorials
    /// </summary>
    /// <param name="tabs"></param>
    public void SetTabContents(params object[] tabs)
    {
      if (tabs == null || !tabs.Any()) return;
      Log.Add("Replace tab contents with (new): " + tabs.Count());
      TabHandler.ReplaceTabContents(tabs.ToList());
    }

    protected ITag TabsBeforeContent()
    {
      var tabs = TabHandler.CompleteTabs;
      var active = TabHandler.ActiveTab;
      var l = Log.Call<ITag>("tabs (" + tabs.Count() + "): " + TabHandler.TabNamesDebug + "; active: " + active);

      var outputOpen = SourceWrap?.OutputOpen();

      if (tabs.Count() <= 1)
        return l(outputOpen, "no tabs");

      var firstTab = tabs.FirstOrDefault();
      var firstIsActive = active.DisplayName == firstTab.DisplayName;
      var result = Tag.RawHtml(
        // Tab headers
        BsTabs.TabList(TabPrefix, tabs, active),
        // Tab bodies - must open the first one
        BsTabs.TabContentGroupOpen(),
        // Open the first tab-body item IF the snippet is right after this
        (firstTab.Type == TabType.Results || firstTab.Type == TabType.ResultsAndSource)
          ? BsTabs.TabContentOpen(TabPrefix, firstTab.DomId, firstIsActive)
          : null,
        // If we have a source-wrap, add it here
        outputOpen
      );
      return l(result, "ok");
    }

    /// <summary>
    /// End of Snippet - called by the accordion
    /// </summary>
    public ITag SnipEnd()
    {
      var tabs = TabHandler.CompleteTabs;
      // Logging
      var active = TabHandler.ActiveTab;
      var l = Log.Call<ITag>("tabPfx:" + TabPrefix 
        + "; TabNames: " + TabHandler.TabNamesDebug
        + "; results:" + tabs.Count()
        + "; results:" + TabHandler.TabContentsDebug);
      var nameCount = 0;

      // Close the tabs / header div section if it hasn't been closed yet
      var html = Tag.RawHtml();

      // If we have any results, add them here; Very often there are none left
      foreach (var tab in tabs) {
        string name = tab.DisplayName;
        // Find Name and Log Stuff

        nameCount++;
        var msg = "tab name: " + name + " (" + nameCount + ")";
        Log.Add(msg);
        html = html.Add("<!-- " + msg + "-->");

        // Special: if it's the ResultTab (usually the first) - close
        if (tab.Type == TabType.Results) // name == ResultTabName)
        {
          Log.Add("Contents of: " + ResultTabName);
          if (SourceWrap != null) html = html.Add(SourceWrap.OutputClose());
          html = html.Add(BsTabs.TabContentClose());
          continue;
        }

        // Special case: Source-and-Result Tab
        if (tab.Type == TabType.ResultsAndSource)
        {
          Log.Add("snippetInResultTab - SourceWrap: " + SourceWrap);
          html = html.Add(SourceWrap?.OutputClose(), SourceWrapped());
          // Reliably close the "Content" section IF it had been opened
          html = html.Add(BsTabs.TabContentClose());
          continue;
        }

        var nameId = tab.DomId;

        // Special case: Source Tab
        if (tab.Body as string == SourceTabName) {
          Log.Add("Contents of: " + SourceTabName);
          html = html.Add(BsTabs.TabContent(TabPrefix, nameId, SourceWrapped(), isActive: active.DomId == tab.DomId));
          continue;
        }

        // Other: Normal predefined content
        Log.Add("Contents of: " + name + "; nameId: " + nameId);
        var body = FlexibleResult(tab, Item);
        var isActive = active.DomId == tab.DomId;// active == nameId;
        html = html.Add(BsTabs.TabContent(TabPrefix, nameId, body, isActive: isActive));
      }

      html = html.Add(BsTabs.TabContentGroupClose());
      return l(html, "ok");
    }


    private ITag SourceWrapped() {
      var snippet = FileHandler.ShowSnippet(file: SourceFile);
      return SourceWrap == null
        ? snippet
        : Tag.RawHtml(SourceWrap.SourceOpen(), snippet, SourceWrap.SourceClose());
    }

    /// <summary>
    /// Take a result and if it has a special prefix, process that
    /// </summary>
    private object FlexibleResult(TabSpecs tab, TutorialSnippet item = null)
    {
      // Start with special cases like DataSource or Model
      if (tab.Type == TabType.DataSource)
        return FileHandler.GetTabFileContents($"/AppCode/DataSources/{tab.Value}.cs");

      // When showing a model, it can be either the model itself or the generated model or a mix
      // eg. CsvProduct is a manually created model, which doesn't have a generated part
      if (tab.Type == TabType.Model)
      {
        var extended = FileHandler.GetTabFileContents($"/AppCode/Data/{tab.Value}.cs", silent: true);
        var generated = FileHandler.GetTabFileContents($"/AppCode/Data/{tab.Value}.Generated.cs", silent: true);
        return Tag.RawHtml(extended, generated);
      }

      // If it's not a string, then it must be something prepared, typically IHtmlTags; return that
      if (!(tab.Body is string strResult))
        return tab.Body;

      // If it's a string such as "file:abc.cshtml" then resolve that first
      if (tab.Type == TabType.File || strResult.StartsWith("file:"))
        return FileHandler.GetTabFileContents(strResult.Replace("file:", ""));

      // Handle case "html-img:..."
      if (strResult.StartsWith("html-img:"))
        return FileHandler.ShowResultImg(strResult.Substring(9));

      // Optionally add tutorial links if defined in the item
      if (item == null)
        return tab.Body;

      // Handle case Tutorials
      if (tab.Type == TabType.TutorialReferences)
      {
        var liLinks = Item.Tutorials.Select(tutPage => $"\n    {TutLinks.TutPageLink(tutPage)}\n");
        return Tag.Ol(liLinks);
      }

      // Handle case Notes
      if (tab.Type == TabType.Notes)
      {
        if (item.IsEmpty(nameof(item.Notes)))
          return $"{nameof(item.Notes)} not found";

        var notesHtml = item.Notes.Select(tMd => Tag.RawHtml(
          "\n    ",
          Tag.Div().Class("alert alert-" + tMd.String("NoteType"))
            .Attr(Kit.Toolbar.Empty().Edit(tMd))
            .Wrap(
              Tag.H4(tMd.String("Title")),
              tMd.Html("Note"),
              Fancybox.Gallery(tMd, "Images")
            ),
          "\n"));
        return notesHtml;
      }

      // handle case In-Depth Explanations
      if (tab.Type == TabType.InDepth)
        return Tag.RawHtml(
          "\n",
          item.InDepthExplanation,
          Fancybox.Gallery(item, "InDepthImages"),
          "\n"
        );

      // Handle Case ViewConfig
      if (tab.Type == TabType.ViewConfig)
        return Tag.Div().Wrap(
          Tag.H4(tab.Label),
          Tag.P("This is how this view would be configured for this sample."),
          ViewConfig.TabContents()
        );

      // handle case Formulas
      if (tab.Type == TabType.Formulas)
        return Formulas.Show(item.Formula, false);

      // Other cases - just return original - could be the label or a prepared string
      return tab.Body;
    }
  }
}