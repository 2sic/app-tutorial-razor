using System.Collections.Generic;

namespace AppCode.Data
{
  public interface IHasNotes
  {
    IEnumerable<TutorialNote> Notes {get;}
  }

  partial class TutorialGroup: IHasNotes { }

  partial class TutorialSnippet: IHasNotes { }
}