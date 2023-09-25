using Custom.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;

public class Helpers: Custom.Hybrid.CodeTyped
{
  public string TutorialSectionType = "TutorialSection";
  // public string TutViewMetadataType = "TutorialViewMetadata";

  public Helpers Init(Razor14 page) {
    Path = page.Path;
    return this;
  }

  public Helpers Init(RazorTyped page) {
    Path = page.Path;
    return this;
  }

  public string Path {get;set;}

  public dynamic SourceCode { get { return _sourceCode ?? (_sourceCode = GetCode("SourceCode.cs").Init(this, Path)); } }
  private dynamic _sourceCode;

  public dynamic PageParts { get { return _pageParts ?? (_pageParts = GetCode("PageParts.cs").Init(this)); } }
  private dynamic _pageParts;

  public dynamic Fancybox { get { return _fancybox ?? (_fancybox = GetCode("Fancybox.cs")); } }
  private dynamic _fancybox;

  public dynamic ToolbarHelpers { get { return _tlbHelpers ?? (_tlbHelpers = GetCode("ToolbarHelpers.cs")).Init(this); } }
  private dynamic _tlbHelpers;

  // public dynamic InfoSection { get { return _infs ?? (_infs = GetCode("InfoSection.cs")).Init(this); } }
  // private dynamic _infs;

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

  public dynamic Highlighted(string specialText) {
    if (specialText == null) { return null; }
    return Tag.Span(specialText).Class("text-warning");
  }

}