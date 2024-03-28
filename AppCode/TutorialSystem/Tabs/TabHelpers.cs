namespace AppCode.TutorialSystem.Tabs
{
  public class TabHelpers
  {
      // WARNING: DUPLICATE CODE BootstrapTabs.cs / SourceCode.cs; keep in sync
      public static string Name2TabId(string name) {
        return "-" + name.ToLower()
          .Replace(" ", "-")
          .Replace(".", "-")
          .Replace("/", "-")
          .Replace("\\", "-");
      }
  }
}