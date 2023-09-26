using Custom.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;

public class Sys: Custom.Hybrid.CodeTyped
{
  // public string TutorialSectionType = "TutorialSection";

  public Sys Init(Razor14 page) {
    Path = page.Path;
    return this;
  }

  public Sys Init(RazorTyped page) {
    Path = page.Path;
    return this;
  }

  public string Path {get;set;}

  public object SourceCode { get { return _sourceCode ?? (_sourceCode = GetCode("./source/SourceCode.cs").Init(this, Path)); } }
  private object _sourceCode;

  public object Fancybox { get { return _fancybox ?? (_fancybox = GetCode("../Shared/Fancybox.cs")); } }
  private object _fancybox;

  public object ToolbarHelpers { get { return _tlbHelpers ?? (_tlbHelpers = GetCode("./ToolbarHelpers.cs")); } }
  private object _tlbHelpers;

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