using AppCode.Data;

namespace AppCode.TutorialSystem.Razor
{
  public class ManageGroupModel
  {
    public TutorialGroup TutorialGroup { get; set; }
    public int Recursion { get; set; } = 100;
  }
}