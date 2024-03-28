using System.Collections.Generic;
using AppCode.Data;

namespace AppCode.TutorialSystem.Razor
{
  /// <summary>
  /// Base class for manage view which expects a TutorialGroup parameter.
  /// </summary>
  public abstract class ManageGroup: AppCode.Razor.AppRazor<AppCode.TutorialSystem.Razor.ManageGroupModel>
  {
    public TutorialGroup TutorialGroup => Model.TutorialGroup;
    
    public IEnumerable<TutorialGroup> Accordions => Model.TutorialGroup.Accordions;

    public IEnumerable<TutorialSnippet> Sections => Model.TutorialGroup.Sections;

  }
}