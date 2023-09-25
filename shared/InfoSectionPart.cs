// using Custom.Hybrid; 
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using System.Collections.Generic;
using System.Linq;
using ToSic.Sxc.Edit.Toolbar;

// Class to generate shared parts on the page
// Such as navigations etc.
// Should itself not have much code, it's more central API to access everything
public class InfoSectionPart: Custom.Hybrid.Code14
{
  #region Init / Dependencies
  
  public InfoSectionPart Init(dynamic sys, dynamic parent, string field) {
    Sys = sys;
    Parent = parent;
    Field = field;
    return this;
  }
  public dynamic Sys = null;
  public dynamic Parent = null;
  public string Field = null;

  #endregion

  #region Constants
  const string SectRequirements = "requirements";
  const string SectResources = "resources";
  const string SectRelated = "related";
  #endregion

  private object ContextAttribute {
    get {
      return _ctxAttr ?? (_ctxAttr = Kit.Edit
        .ContextAttributes(Parent, field: Field, contentType: NewItemType()));
    }
  }
  private object _ctxAttr;

  private IToolbarBuilder ItemToolbar {
    get {
      return _itmTlb ?? (_itmTlb = Kit.Toolbar.Empty()
        .Edit().AddExisting().MoveUp().MoveDown().Remove());
    }
  }
  private IToolbarBuilder _itmTlb;

  public ITag Section(IEnumerable<dynamic> data) {
    var section = Field.ToLowerInvariant();
    var icon = section == SectRequirements
      ? "fa-exclamation-circle"
      : "fa-info-circle";

    if (!data.Any() && !CmsContext.User.IsSiteAdmin)
      return null;

    var typeToAdd = NewItemType();
    var toolbar = Kit.Toolbar.Empty().Edit(Parent);
    // toolbar = section == SectRelated
    //   ? toolbar.AddExisting(contentType: typeToAdd)
    //   : toolbar.New(data.First()).AddExisting(contentType: typeToAdd);    
    // toolbar = toolbar.Button("add-existing", parameters: "contentType:" + typeToAdd);

    return Tag.Div().Class("card mb-4")
      .Attr(ContextAttribute)
      .Wrap(
        Tag.H6().Class("card-header").Attr(toolbar).Wrap(
          Tag.I().Class("fas " + icon),
          Tag.Span(Field).Class("ml-2 ms-2")
        ),
        LinksInSection(data)
      );
  }

  public ITag LinksInSection(IEnumerable<dynamic> data) {
    if (data == null) return null;

    var result = Tag.RawHtml();
    foreach (var dataEl in data) {
      var tutUrl = dataEl.EntityType == "TutorialGroup" ? Sys.TutPageUrlFromDyn(dataEl) as string : null;
      string section = Field.ToLowerInvariant();
      var url = (section == SectRequirements || section == SectResources)
          ? dataEl.Link
          : (section == SectRelated && tutUrl != null)
            ? tutUrl
            : "unknown info-section";
      result = result.Add(Tag.Li().Attr(ItemToolbar.For(dataEl)).Wrap(
        Tag.A(dataEl.Title).Href(url).Target("_blank")
      ));
    }
    var divs = Tag.Div().Class("list-group list-group-flush")
      .Wrap(
        Tag.Ul().Class("list-unstyled list-group-item ml-2 ms-2").Wrap(
          result
        )
      );
    return divs;
  }

  private string NewItemType() {
    string section = Field.ToLowerInvariant();
    switch (section) {
      case SectResources: return "TutorialResource";
      case SectRequirements: return "TutorialRequirement";
      case SectRelated: return "TutorialSnippet";
      default: return null;
    }
  }

  // private dynamic GetView(dynamic Data) {
  //   var ViewList = AsList(App.Data["2SexyContent-Template"] as object);
  //   var View = ViewList.FirstOrDefault(view => view.Metadata == Data);
  //   if (View != null)
  //     return View;
  //   else
  //     return null;
  // }
}

// The next line is for 2sxc-internal quality checks, you can ignore this
// It's necessary because the term "SexyContent" is used in the code
// 2sxclint:disable:dont-use-sexycontent-ns