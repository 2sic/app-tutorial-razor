// using Custom.Hybrid;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
// Class to generate shared parts on the page
// Such as navigations etc.
// Should itself not have much code, it's more central API to access everyhing
public class InfoSection: Custom.Hybrid.Code14
{
  #region Init / Dependencies
  
  public InfoSection Init(dynamic sys) {
    Sys = sys;
    // ParentRazor = Sys.ParentRazor;
    return this;
  }
  public dynamic Sys = null;
  // public Razor14 ParentRazor;

  #endregion

  public dynamic Part(dynamic parent, string field) {
    return CreateInstance("InfoSectionPart.cs").Init(Sys, parent, field);
  }
}