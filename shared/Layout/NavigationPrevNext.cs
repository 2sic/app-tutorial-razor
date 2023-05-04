using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Dynlist = System.Collections.Generic.IEnumerable<dynamic>;

public class NavigationPrevNext: Custom.Hybrid.Code14
{
  // todo: make sure not duplicate, probabbly hlp.... doesn't need it anymore
  const string viewMdType = "TutorialViewMetadata";

  public NavigationPrevNext Init(dynamic hlp, ICmsView view) {
    View = view;
    this.hlp = hlp;
    return this;
  }
  public ICmsView View;
  public dynamic hlp = null;

  // public NavigationParts GetNavParts() {
  //   var currentView = CmsContext.View;
  //   var viewMd = NavPrevNext.TryGetViewMd(currentView);
  //   var sortedTutorialSections = (AsList(App.Data["Tutorial"]).Last().Sections as Dynlist).ToList();
  //   return new NavigationParts {
  //     ViewMetadata = viewMd,
  //     SortedSections = sortedTutorialSections
  //   };
  // }


  // Fallback for missing metadata
  dynamic DummyViewMd { get { return _dummyViewMd ?? (_dummyViewMd = AsDynamic( new { LinkTitle = "Todo" })); }}
  dynamic _dummyViewMd = null;

  public dynamic TryGetViewMd(dynamic tutorial) {
    return tutorial != null
      ? AsDynamic((tutorial.Metadata.OfType(viewMdType) as Dynlist).FirstOrDefault() ?? DummyViewMd)
      : null;
  }

}

public class NavigationParts
{
  public dynamic ViewMetadata { get; set; }
  // public List<dynamic> SortedSections { get; set; }
}