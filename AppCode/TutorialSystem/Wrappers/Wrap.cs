using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using System.Collections.Generic;
using System.Linq;
using static AppCode.TutorialSystem.Constants;
using AppCode.TutorialSystem.Tabs;
using AppCode.TutorialSystem.Sections;

namespace AppCode.TutorialSystem.Wrappers
{
  public class Wrap
  {
    public const string Indent1 = "      ";
    public const string Indent2 = "        ";
    
    public Wrap(TutorialSectionEngine sb, string name, bool combined = false, string tabsCsv = null) {
      Section = sb;
      Name = name ?? "Wrap";
      Tabs = (tabsCsv != null) ? tabsCsv.Split(',').ToList() : new List<string> { ResultTabName, SourceTabName };
      TabSelected = Tabs.First();
      TagCount = new TagCount(Name, true);
    }

    protected readonly TutorialSectionEngine Section;
    public List<string> Tabs { get; protected set; }
    public string TabSelected {get; set;}
    protected TagCount TagCount;
    protected string Name;
    private bool ToolbarForAnonymous => _toolbarForAnon ??= (_toolbarForAnon = Section.Item.Bool("ToolbarsForAnonymous")).Value;
    private bool? _toolbarForAnon;
    public virtual ITag OutputOpen() {
      // Special feature for Toolbar Demos
      var noteToolbar = Tag.RawHtml();
      if (ToolbarForAnonymous) {
        Section.ToolbarHelpers.EnableEditForAll();
        Section.ToolbarHelpers.AutoShowAllToolbarsStart();
        noteToolbar = noteToolbar.Add(Indent1, Comment("", "toolbar for anonymous")); // + "<!-- toolbar for anonymous -->\n";
      }
      return Tag.RawHtml(
        noteToolbar,
        Comment(""),
        Indent1
      );
    }
    public virtual ITag OutputClose() {
      var noteToolbar = Tag.RawHtml();
      if (ToolbarForAnonymous) {
        Section.ToolbarHelpers.AutoShowAllToolbarsEnd();
        noteToolbar = noteToolbar.Add(Indent1, Comment("/", "toolbar for anonymous")); // + "<!-- toolbar for anonymous -->\n";
      }
      return Tag.RawHtml(
        noteToolbar,
        Comment("/")
      );
    }

    public virtual ITag SourceOpen() { return Tag.RawHtml(Comment("")); }
    public virtual ITag SourceClose() { return Tag.RawHtml(Comment("/")); }

    protected string Comment(string op, string name = null) {
      return "\n" + Indent1 + "<!-- " + op + (name ?? Name) + " -->\n";
    }
  }
}