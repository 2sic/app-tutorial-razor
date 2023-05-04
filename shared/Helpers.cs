using Custom.Hybrid;
using ToSic.Razor.Blade;
using System.Web;
using System.Collections.Generic;
using System.Linq;

public class Helpers: Custom.Hybrid.Code14
{
  public string TutorialSectionType = "TutorialSection";
  public string TutViewMetadataType = "TutorialViewMetadata";
  public string TutViewSharingMetadataType = "TutorialViewSharing";

  public Helpers Init(Razor14 page) {
    ParentRazor = page;
    Path = ParentRazor.Path;
    return this;
  }
  public Razor14 ParentRazor;

  public string Path {get;set;}

  public dynamic SourceCode { get { return _sourceCode ?? (_sourceCode = CreateInstance("SourceCode.cs").Init(this, Path)); } }
  private dynamic _sourceCode;

  public dynamic PageParts { get { return _pageParts ?? (_pageParts = CreateInstance("PageParts.cs").Init(this)); } }
  private dynamic _pageParts;

  public dynamic Fancybox { get { return _fancybox ?? (_fancybox = CreateInstance("Fancybox.cs")); } }
  private dynamic _fancybox;

  public dynamic ToolbarHelpers { get { return _tlbHelpers ?? (_tlbHelpers = CreateInstance("ToolbarHelpers.cs")).Init(this); } }
  private dynamic _tlbHelpers;

  public dynamic InfoSection { get { return _infs ?? (_infs = CreateInstance("InfoSection.cs")).Init(this); } }
  private dynamic _infs;



  
  public dynamic TutLink(string label, string target) {
    return Tag.A(label).Href(Link.To(parameters: GetTargetUrl(target)));
  }

  public string GetTargetUrl(string target) {
    target = target.Replace("/", "=");
    target = target + (target.Contains("=") ? "" : "=page");
    return target;
  }

  public dynamic TutorialLink(string label, string target, string description = null, string newText = null, string appendix = null) {
    var result = Tag.Li(
      Tag.Strong(
        TutLink(label + " ", target),
        Highlighted(newText),
        appendix
      )
    );
    if(!string.IsNullOrWhiteSpace(description)) {
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
    if(!string.IsNullOrWhiteSpace(description)) {
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
    if(specialText == null) { return null; }
    return Tag.Span(specialText).Class("text-warning");
  }

  public dynamic ExternalLink(string target, string description) {
    return Tag.A(description).Href(target).Target("_blank");
  }

  public dynamic LiExtLink(string target, string description) {
    return Tag.Li(ExternalLink(target, description));
  }
}