using Custom.Hybrid;
using ToSic.Sxc.Demo;

// Important notes: 
// - This class should have the same name as the file it's in
public class ToolbarHelpers : Custom.Hybrid.Code14 
{
  #region Init / Dependencies
  
  public ToolbarHelpers Init(dynamic sys) {
    Sys = sys;
    // ParentRazor = Sys.ParentRazor;
    return this;
  }
  public dynamic Sys = null;
  // public Razor14 ParentRazor;

  #endregion

  // Todo: replace the EnableEditForAnonymous with this everywhere and make it private
  public void EnableEditForAll() {
    EnableEditForAnonymous();
  }

  public void EnableEditForAnonymous() {
    // Special command to ensure that the toolbars appear, even if they are won't work.
    // This is NOT an official API, and may change any time.
    // This will tell the edit-UI that it's enabled (which is usually only the case if a user is an editor)
    Kit.Edit.Enabled = true;

    // This will tell the edit-UI that we need the JS features
    Kit.Page.Activate("2sxc.JsCore", "2sxc.JsCms", "2sxc.Toolbars");
  }

  // Special internal API which will make the toolbars always show
  // even without hover. This is an internal API for demos only. 
  // Must be added after the intro section of this file, as that can also create many toolbar 
  // which shouldn't be affected
  public string AutoShowAllToolbarsStart() {
    Kit.Toolbar.ActivateDemoMode(ui: "show=always");
    return "";// Return empty string so this command can be used inline
  }

  // Special internal API which will make the toolbars always show
  // even without hover. This is an internal API for demos only. 
  public string AutoShowAllToolbarsEnd() {
    Kit.Toolbar.ActivateDemoMode(ui: null);
    return "";
  }
}
