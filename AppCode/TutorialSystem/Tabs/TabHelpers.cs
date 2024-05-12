namespace AppCode.TutorialSystem.Tabs
{
  public class TabHelpers
  {
      public static string Name2TabId(string name) {
        return "-" + name.ToLower()
          .Replace(" ", "-")
          .Replace(".", "-")
          .Replace("/", "-")
          .Replace("\\", "-");
      }
  }
}