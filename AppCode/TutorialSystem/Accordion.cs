using System;
using System.Collections.Generic;
using System.Linq;
using AppCode.Data;
using AppCode.TutorialSystem.Tabs;
using AppCode.Tutorial;
using ToSic.Razor.Blade;
using AppCode.TutorialSystem.Sections;
using static AppCode.TutorialSystem.Constants;

namespace AppCode.TutorialSystem
{
  public class Accordion: Custom.Hybrid.CodeTyped
  {
    public Accordion Setup(string variantExtension, TutorialGroup item) {
      _variantExtension = variantExtension;
      Item = item;
      return this;
    }
    public TagCount TagCount = new TagCount("Accordion", true);
    private string _variantExtension;

    public IHtmlTag Start(TutorialGroup item) {
      Item = item;
      Name = item.NameId;
      if (!_variantExtension.Has())
        _variantExtension = "." + Variant;
      var t = Kit.HtmlTags;

      return t.RawHtml(
        "\n<!-- Accordion.Start(" + Name + ") -->\n",
        TagCount.Open(t.Div().Class("accordion").Id(Name))
      );
    }

    public bool IsTyped => Variant == VariantTyped;
    public string Variant => MyPage.Parameters[VariantUrlParameter] ?? VariantTyped;

    public string Name { get; private set; }

    public TutorialGroup Item { get; private set; }

    public IHtmlTag End() {
      var end = TagCount.Close(DivEnd + "<!-- /Accordion -->");
      Item = null;
      return end;
    }

    private string NextName() => Name + "-" + AutoPartName + AutoPartIndex++;

    public IEnumerable<Section> Sections(string basePath, string backtrack) {
      if (Item == null) throw new Exception("Item in Accordion is null");
      // var appPath = App.Folder.Path;
      basePath = Text.BeforeLast(basePath, "/");
      var names = Item.Sections
        .Select(itm => {
          var tutorialId = itm.TutorialId;
          // first try special extension eg. .Typed.Cshtml
          if (!CheckFile(backtrack, tutorialId, _variantExtension, out string fileName))
            CheckFile(backtrack, tutorialId, null, out fileName);
          return new Section(this, Kit.HtmlTags, NextName(), item: itm, fileName: fileName);
        })
        .ToList();
      return names;
    }

    private static string[] MovedTutorials = new [] {
      "basics",
      "block",
      "code",
      "context",
      "data",
      "datasources",
      "hybrid",
      "img",
      "items",
      "js",
      "json",
      "koi",
      "languages",
      "linq",
      "query",
      "page",
      "razor",
      "razorblade",
      "settings",
      "turnon",
      "ui",
      "webapi",
    };

    private bool CheckFile(string relBacktrack, string tutorialId, string variant, out string fileName) {
      var l = Log.Call<bool>("tutorialId: " + tutorialId);
      var topPath = Text.Before(tutorialId, "-");
      var rest = Text.After(tutorialId, "-");
      var secondPath = Text.Before(rest, "-");
      rest = Text.After(rest, "-");

      if (!Text.Has(secondPath))
        throw new Exception("Second path is empty, original was '" + tutorialId + "'");

      // New 2023-09-26 2dm - moving all snips to an own folder
      if (MovedTutorials.Contains(topPath)) {
        topPath = "tutorials/" + topPath + "-" + secondPath;
        secondPath = "";
      }


      var realName = "Snip-" + rest + variant + ".cshtml";
      var fullPath = System.IO.Path.Combine(App.Folder.PhysicalPath + "\\", topPath, secondPath, realName);

      if (System.IO.File.Exists(fullPath)) {
        fileName = relBacktrack + "/" + System.IO.Path.Combine(topPath, secondPath, realName);
        return l(true, "exists");
      }
      fileName = null;
      return l(false, "not found");
    }

    private const string AutoPartName = "auto-part-";
    private int AutoPartIndex = 0;

    internal string DivEnd = "</div>";
  }

}