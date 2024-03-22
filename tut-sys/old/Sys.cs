using Custom.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using AppCode.Source;
using AppCode.Output;

public class Sys: Custom.Hybrid.CodeTyped
{
  public Sys Init(Razor14 page) {
    Path = page.Path;
    return this;
  }

  public Sys Init(RazorTyped page) {
    Path = page.Path;
    return this;
  }

  public string Path {get;set;}

  public SourceCode SourceCode => _sourceCode ??= GetService<SourceCode>().Init(this, Path);
  private SourceCode _sourceCode;

  public Fancybox Fancybox => _fancybox ??= GetService<Fancybox>(); // (_fancybox = GetCode("../Shared/Fancybox.cs")); } }
  private Fancybox _fancybox;

  public ToolbarHelpers ToolbarHelpers => _tlbHelpers ??= GetService<ToolbarHelpers>();
  private ToolbarHelpers _tlbHelpers;

  #region New Links to the new setup

  public IHtmlTag TutPageLink(ITypedItem tutPage) {
    var label = tutPage.String(tutPage.IsNotEmpty("LinkTitle") ? "LinkTitle" : "Title", scrubHtml: "p") + " ";
    var result = Tag.Li()
      .Attr(Kit.Toolbar.Empty().Edit(tutPage))
      .Wrap(
        Tag.Strong(
          Tag.A(label).Href(TutPageUrl(tutPage)),
          Highlighted(tutPage.String("LinkEmphasis"))
        )
      );
    if (tutPage.IsNotEmpty("LinkTeaser")) {
      result = result.Add(Tag.Br(), tutPage.String("LinkTeaser"));
    } else if (tutPage.IsNotEmpty("Intro")) {
      result = result.Add(Tag.Br(), Text.Ellipsis(tutPage.String("Intro", scrubHtml: true), 250));
    }
    return result;
  }
  // public string TutPageUrlFromDyn(object dynTutPage) {
  //   return TutPageUrl(AsItem(dynTutPage));
  // }

  public string TutPageUrl(ITypedItem tutPage) {
    if (tutPage == null) return null;
    return Link.To(parameters: MyPage.Parameters.Set("tut", tutPage.String("NameId").BeforeLast("-Page")));
  }

  #endregion


  // TODO: find usages (especially in app.xml) and correct
  
  public IHtmlTag TutLink(string label, string target) {
    return Tag.A(label).Href(Link.To(parameters: GetTargetUrl(target)));
  }

  public string GetTargetUrl(string target) {
    target = target.Replace("/", "=");
    target = target + (target.Contains("=") ? "" : "=page");
    return target;
  }

  public IHtmlTag Highlighted(string specialText) {
    if (specialText == null) { return null; }
    return Tag.Span(specialText).Class("text-warning");
  }

}