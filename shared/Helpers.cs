using ToSic.Razor.Blade;
using System.Web;
using System.Collections.Generic;
using System.Linq;
public class Helpers: Custom.Hybrid.Code14
{
  public string TutorialSectionType = "TutorialSection";
  public string TutViewMetadataType = "TutorialViewMetadata";
  public string TutViewSharingMetadataType = "TutorialViewSharing";

  public dynamic Title(string title) {
    // set browser title for SEO
    Kit.Page.SetTitle(title + " DNN / 2sxc Razor Tutorials ");
    return Tag.Custom(
      InitializedPageAssets()
      + AddFeatureLogo("app-icon.png", "https://2sxc.org/dnn-tutorials/en/razor")
      + Tag.H1(title).Attr(Edit.TagToolbar()).Class("col-auto p-0")
    );
  }


  private dynamic Breadcrumb(string name, string topicUrl) {
    var result = Tag.Custom(null).Wrap( 
        Tag.A("Tutorial Home").Href(Link.To())
    );
    if (!string.IsNullOrEmpty(name)) {
      result.Add(" › ", Tag.A(name).Href(Link.To(parameters: topicUrl.Replace("/", "="))));
    }
    return result;
  }

  public dynamic TitleAndBreadcrumb(string title, string name, string topicUrl) {
    return Tag.Custom(
      Title(title).ToString() +
      Breadcrumb(name, topicUrl)
    );
  }


  public dynamic AddFeatureLogo(string path, string link, int size = 0) {
    var img = Tag.Div(Tag.Img().Src(App.Path + "/" + path + "?w=75&h=75").Class("img-fluid")).Class("icon-wrapper");
    if (size != 0) img.Style("height: " + size + "px;" + " width: " + size + "px;");
    return Tag.A().Href(link).Target("_blank").Wrap(img.Class("float-right ml-3 ms-3 float-end"));
  }
  
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


  dynamic InitializedPageAssets() {
    // Tell the page that we need the 2sxc Js APIs
    Kit.Page.Activate("2sxc.JsCore"); 

    var bsCheck = CreateInstance("Bootstrap4.cs");
    bsCheck.EnsureBootstrap4();
    return "<link rel='stylesheet' href='" + App.Path + "/assets/styles.css' enableoptimizations='true' />";
  }

  public dynamic ExternalLink(string target, string description) {
    return Tag.A(description).Href(target).Target("_blank");
  }

  public dynamic LiExtLink(string target, string description) {
    return Tag.Li(ExternalLink(target, description));
  }

  public string GetFullPath(string filePath) {
    #if NETCOREAPP
      // This is the Oqtane implementation - cannot use Server.MapPath
      // 2sxclint:disable:v14-no-getservice
      var hostingEnv = GetService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>();
      var pathWithTrimmedFirstSlash = filePath.TrimStart(new [] { '/', '\\' });
      return System.IO.Path.Combine(hostingEnv.ContentRootPath, pathWithTrimmedFirstSlash);
    #else
      return HttpContext.Current.Server.MapPath(filePath);
    #endif
  }
  
  public dynamic GetViewUrl(dynamic Data){
    var ViewList = AsList(App.Data["2SexyContent-Template"] as object);
    var View = ViewList.FirstOrDefault(view => view.Metadata == Data);
    if(View != null)
      return View;
    else
      return null;
  }
}