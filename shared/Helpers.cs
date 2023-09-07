using Custom.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;

public class Helpers: Custom.Hybrid.CodeTyped
{
  public string TutorialSectionType = "TutorialSection";
  public string TutViewMetadataType = "TutorialViewMetadata";
  public string TutViewSharingMetadataType = "TutorialViewSharing";

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

  public dynamic InfoSection { get { return _infs ?? (_infs = GetCode("InfoSection.cs")).Init(this); } }
  private dynamic _infs;



  
  public IHtmlTag TutLink(string label, string target) {
    return Tag.A(label).Href(Link.To(parameters: GetTargetUrl(target)));
  }

  public string GetTargetUrl(string target) {
    target = target.Replace("/", "=");
    target = target + (target.Contains("=") ? "" : "=page");
    return target;
  }

  public IHtmlTag TutorialLiLinkLookup(string target) {
    var view = App.Data["2SexyContent-Template"].FirstOrDefault(e => e.Get<string>("ViewNameInUrl") == target + "/.*");
    return (view == null)
      ? Tag.Li("View not Found: " + target)
      : TutorialLiFromView(view);
  }

  public IHtmlTag TutorialLiFromViewMd(ITypedItem viewMd) {
    var target = viewMd.Entity.MetadataFor;
    var targetId = target.KeyGuid;
    var view = App.Data["2SexyContent-Template"].FirstOrDefault(e => e.EntityGuid == targetId);
    if (view == null) throw new Exception("Can't find view - tried target: " + targetId + "; target: " + target);
    return TutorialLiFromView(view);
  }

  public IHtmlTag TutorialLiFromView(object tutViewObj) {
    var tutView = AsItem(tutViewObj);
    var urlPattern = tutView.String("ViewNameInUrl").Replace("/.*", "");
    var viewMetadata = tutView.Metadata;
    var viewMdType = TutViewMetadataType as string;
    var viewMd = AsItem((viewMetadata.OfType(viewMdType)).FirstOrDefault());

    var tutLink = (viewMd == null)
      // If we don't have metadata, just create a dummy entry but add the toolbar to allow editing
      ? TutorialViewLink(
        label: "Todo - add metadata",
        target: urlPattern,
        description: "",
        newText: "",
        deprecated: false)
      : TutorialViewLink(
        label: Kit.Scrub.Only(viewMd.String("LinkTitle"), "p"),
        target: urlPattern,
        description: viewMd.String("LinkTeaser"),
        newText: viewMd.String("LinkEmphasis"),
        deprecated: viewMd.Bool("Deprecated"));
    // Metadata with new toolbar services
    tutLink.Attr(Kit.Toolbar.Metadata(tutView, viewMdType));
    return tutLink;
  }


  public dynamic TutorialLink(string label, string target, string description = null, string newText = null, string appendix = null) {
    var result = Tag.Li(
      Tag.Strong(
        TutLink(label + " ", target),
        Highlighted(newText),
        appendix
      )
    );
    if (!string.IsNullOrWhiteSpace(description)) {
      result.Add(Tag.Br(), description);
    }
    return result;
  }

  public dynamic TutorialViewLink(string label, string target, string description = null, string newText = null, string appendix = null, bool deprecated = false) {
    var result = Tag.Li(
      Tag.Strong(
        TutLink(label + " ", target).Class(deprecated ? "deprecated" : ""),
        Highlighted(newText),
        appendix
      )
    );
    if (!string.IsNullOrWhiteSpace(description)) {
      result.Add(description);
    }
    return result;
  }

  
  public dynamic TutorialLinkHome(string label, string target, string description, string newText = null) {
    return Tag.Li(
      Tag.Strong(
        Tag.A().Href(Link.To(parameters: target + "=home")).Wrap(
          label + " ",
          Highlighted(newText)
        )
      ),
      Tag.Br(),
      description
    );
  }


  public dynamic Highlighted(string specialText) {
    if (specialText == null) { return null; }
    return Tag.Span(specialText).Class("text-warning");
  }

  public dynamic ExternalLink(string target, string description) {
    return Tag.A(description).Href(target).Target("_blank");
  }

  public dynamic LiExtLink(string target, string description) {
    return Tag.Li(ExternalLink(target, description));
  }
}