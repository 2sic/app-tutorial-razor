using AppCode.Data;
using AppCode.Source;

namespace AppCode.Razor
{
  public class SnippetIntroModel
  {
    public TutorialGroup TutorialGroup { get; set; }
    public Accordion Accordion { get; set; }
  }

  public class ShowSectionModel
  {
    public int Recursions { get; set; }
    public Accordion Accordion { get; set; }

  }
  public class AccordionMultiModel : ShowSectionModel
  {
    public string NameId { get; set; }

    public TutorialGroup TutorialGroup { get; set; }
  }

  public class SectionBlockModel : AccordionMultiModel
  {
    public bool SkipPageTools { get; set; }
  }
}