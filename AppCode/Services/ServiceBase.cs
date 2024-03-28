using AppCode.Tutorial;

namespace AppCode.Services
{
  /// <summary>
  /// Base Class for Services which have a typed App.
  /// </summary>
  partial class ServiceBase: Custom.Hybrid.CodeTyped
  {

    protected TutLinks TutLinks => _tutLinks ??= GetService<TutLinks>();
    private TutLinks _tutLinks;
  }
}
