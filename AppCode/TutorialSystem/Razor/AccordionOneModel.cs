using AppCode.Data;
using AppCode.Tutorial;

namespace AppCode.TutorialSystem.Razor
{
  public class AccordionOneModel
  {
    public int Recursions { get; set; }
    
    public string NameId { get; set; }
    public Sys Sys { get; set; }

    public TutorialGroup TutPage {get;set;} 

  }
}