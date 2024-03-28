using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using static AppCode.TutorialSystem.Constants;
using AppCode.Output;
using AppCode.TutorialSystem.Wrappers;
using AppCode.TutorialSystem.Tabs;
using AppCode.TutorialSystem.Source;

namespace AppCode.TutorialSystem.Sections
{
  /// <summary>
  /// Base class for all code sections with snippets etc.
  /// </summary>
  public class TutorialSection
  {
    public TutorialSection(SourceCode sourceCode, ITypedItem item, Dictionary<string, string> tabs, string sourceFile = null) {
      if (item == null) throw new Exception("Item should never be null");
      ScParent = sourceCode;
      BsTabs = ScParent.BsTabs;
      Log = ScParent.Log;
      Item = item;
      SourceWrap = sourceCode.GetSourceWrap(this, item);
      TabHandler = new TabManager(sourceCode, item, tabs, sourceWrap: SourceWrap);
      SourceFile = sourceFile;
      ViewConfig = ScParent.GetService<ViewConfigurationSimulation>().Setup(TabHandler);
    }
    internal SourceCode ScParent;

    public FancyboxService Fancybox => _fancybox ??= ScParent.GetService<FancyboxService>();
    private FancyboxService _fancybox;

    private BootstrapTabs BsTabs;
    internal ICodeLog Log;
    protected int SnippetCount;
    internal string SnippetId {get; private set;}
    public string TabPrefix {get; protected set;}
    protected Wrap SourceWrap;
    internal TabManager TabHandler;
    internal ITypedItem Item;
    internal string SourceFile;
    public ViewConfigurationSimulation ViewConfig;

    /// <summary>
    /// The SnippetId must be provided here, so it can be found in the source code later on
    /// </summary>
    public ITag SnipStart(string snippetId = null) {
      InitSnippetAndTabId(snippetId);
      return TabsBeforeContent();
    }

    /// <summary>
    /// Helper which is usually called by implementations of SnipStart
    /// </summary>
    protected void InitSnippetAndTabId(string snippetId = null) {
      SnippetCount = ScParent.SourceCodeTabCount++;
      SnippetId = snippetId ?? "" + SnippetCount;
      TabPrefix = "tab-" + ScParent.UniqueKey + "-" + SnippetCount + "-" + (snippetId ?? "auto-id");
    }

    /// <summary>
    /// Public method to replace the tab contents from outside
    /// </summary>
    /// <param name="tabs"></param>
    public void SetTabContents(params object[] tabs) {
      if (tabs != null & tabs.Any()) {
        Log.Add("Replace tab contents with (new): " + tabs.Count());
        TabHandler.ReplaceTabContents(tabs.ToList());
      }
    }

    /// <summary>
    /// End of Snippet, so the source-analyzer can find it.
    /// Usually overridden by the implementation.
    /// </summary>
    public virtual ITag SnipEnd()  { return SnipEndFinal(); }

    protected ITag TabsBeforeContent() {
      var tabNames = TabHandler.TabNames;
      var active = TabHandler.ActiveTabName;
      var l = Log.Call<ITag>("tabs (" + tabNames.Count() + "): " + TabHandler.TabNamesDebug + "; active: " + active);

      var outputOpen = SourceWrap != null ? SourceWrap.OutputOpen() : null;

      if (tabNames.Count() <= 1) return l(outputOpen, "no tabs");

      var firstName = tabNames.FirstOrDefault();
      var firstIsActive = active == null || active == firstName;
      var result = Tag.RawHtml(
        // Tab headers
        BsTabs.TabList(TabPrefix, tabNames, active),
        // Tab bodies - must open the first one
        BsTabs.TabContentGroupOpen(),
        // Open the first tab-body item IF the snippet is right after this
        (firstName == ResultTabName || firstName == ResultAndSourceTabName)
          ? BsTabs.TabContentOpen(TabPrefix, TabHelpers.Name2TabId(firstName), firstIsActive)
          : null,
        // If we have a source-wrap, add it here
        outputOpen
      );
      return l(result, "ok");
    }

    protected ITag SnipEndFinal() {
      var results = TabHandler.TabContents;
      var names = TabHandler.TabNames;
      // Logging
      var active = TabHandler.ActiveTabName;
      var l = Log.Call<ITag>("snippetId: " + SnippetId 
        + "; tabPfx:" + TabPrefix 
        + "; TabNames: " + TabHandler.TabNamesDebug
        + "; results:" + results.Count()
        + "; results:" + TabHandler.TabContentsDebug);
      var nameCount = 0;

      // Close the tabs / header div section if it hasn't been closed yet
      var html = Tag.RawHtml();

      // If we have any results, add them here; Very often there are none left
      foreach (var m in results) {
        string name;
        // Find Name and Log Stuff

        name = names.ElementAt(nameCount);
        nameCount++;
        var msg = "tab name: " + name + " (" + nameCount + ")";
        Log.Add(msg);
        html = html.Add("<!-- " + msg + "-->");

        // Special: if it's the ResultTab (usually the first) - close
        if (name == ResultTabName) {
          Log.Add("Contents of: " + ResultTabName);
          if (SourceWrap != null) html = html.Add(SourceWrap.OutputClose());
          html = html.Add(BsTabs.TabContentClose());
          continue;
        }

        // Special case: Source-and-Result Tab
        if (name == ResultAndSourceTabName) {
          Log.Add("snippetInResultTab - SourceWrap: " + SourceWrap);
          html = html.Add(SourceWrap?.OutputClose(), SourceWrapped());
          // Reliably close the "Content" section IF it had been opened
          html = html.Add(BsTabs.TabContentClose());
          continue;
        }

        var nameId = TabHelpers.Name2TabId(name);

        // Special case: Source Tab
        if (m as string == SourceTabName) {
          Log.Add("Contents of: " + SourceTabName);
          html = html.Add(BsTabs.TabContent(TabPrefix, nameId, SourceWrapped(), isActive: active == SourceTabName));
          continue;
        }

        // Other: Normal predefined content
        Log.Add("Contents of: " + name + "; nameId: " + nameId);
        var body = FlexibleResult(m, Item);
        var isActive = active == nameId;
        html = html.Add(BsTabs.TabContent(TabPrefix, nameId, body, isActive: isActive));
      }

      html = html.Add(BsTabs.TabContentGroupClose());
      return l(html, "ok");
    }

    private ITag SourceWrapped() {
      var snippet = ScParent.FileHandler.ShowSnippet(SnippetId, item: Item, file: SourceFile) as ITag;
      return SourceWrap == null
        ? snippet
        : Tag.RawHtml(SourceWrap.SourceOpen(), snippet, SourceWrap.SourceClose());
    }

    // Take a result and if it has a special prefix, process that
    private object FlexibleResult(object result, ITypedItem item = null)
    {
      // If it's not a string, then it must be something prepared, typically IHtmlTags; return that
      var strResult = result as string;
      if (strResult == null) return result; 

      // If it's a string such as "file:abc.cshtml" then resolve that first
      if (strResult.StartsWith("file:"))
        return ScParent.FileHandler.GetTabFileContents(strResult.Substring(5));

      // Handle case "html-img:..."
      if (strResult.StartsWith("html-img:"))
        return ScParent.FileHandler.ShowResultImg(strResult.Substring(9));

      // Optionally add tutorial links if defined in the item
      if (item == null) return result;

      // Handle case Tutorials
      if (strResult == TutorialsTabName) {
        var liLinks = Item.Children("Tutorials").Select(tutPage => "\n    " + ScParent.Sys.TutPageLink(tutPage) + "\n");
        return Tag.Ol(liLinks);
      }

      // Handle case Notes
      if (strResult == NotesTabName) {
        if (item.IsEmpty(NotesFieldName)) return NotesTabName + " not found";

        var notesHtml = item.Children(NotesFieldName).Select(tMd => Tag.RawHtml(
          "\n    ",
          Tag.Div().Class("alert alert-" + tMd.String("NoteType"))
            .Attr(ScParent.Kit.Toolbar.Empty().Edit(tMd))
            .Wrap(
              Tag.H4(tMd.String("Title")),
              tMd.Html("Note"),
              Fancybox.Gallery(tMd, "Images")
            ),
          "\n"));
        return notesHtml;
      }

      // handle case In-Depth Explanations
      if (strResult == InDepthTabName) {
        if (item.IsEmpty(InDepthField)) return InDepthTabName + " not found";
        return Tag.RawHtml(
          "\n",
          item.String(InDepthField),
          Fancybox.Gallery(item, "InDepthImages"),
          "\n"
        );
      }

      // Handle Case ViewConfig
      if (strResult == ViewConfigCode) {
        return Tag.Div().Wrap(
          Tag.H4(ViewConfigTabName),
          Tag.P("This is how this view would be configured for this sample."),
          ViewConfig.TabContents()
        );
      }

      // handle case Formulas
      if (strResult == FormulasTabName) {
        var formulaSpecs = item.Child(FormulaField);
        return ScParent.Formulas.Show(formulaSpecs, false);
      }

      // Other cases - just return original - could be the label or a prepared string
      return result;
    }

  }
}