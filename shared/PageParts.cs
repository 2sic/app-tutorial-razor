using Custom.Hybrid;
using ToSic.Razor.Blade;
using System;
using System.Web;
#if NETCOREAPP
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Text.Encodings.Web;
#endif

// Class to generate shared parts on the page
// Such as navigations etc.
// Should itself not have much code, it's more central API to access everyhing
public class PageParts: Custom.Hybrid.Code14
{
  #region Init / Dependencies
  
  public PageParts Init(dynamic sys) {
    Sys = sys;
    ParentRazor = Sys.ParentRazor;
    return this;
  }
  public dynamic Sys = null;
  public Razor14 ParentRazor;

  #endregion


  public dynamic InfoWrapper() { return Tag.Attr("class", "row mb-5");}

  public dynamic InfoIntro() { return Tag.Attr("class", "col-lg-7");}
  
  public dynamic InfoBoxWrapper() { return Tag.Attr("class", "col-lg-5 order-1");}

}