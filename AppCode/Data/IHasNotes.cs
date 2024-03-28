using System.Collections.Generic;
using ToSic.Sxc.Data;

namespace AppCode.Data
{
  public interface IHasNotes: ITypedItem
  {
    IEnumerable<TutorialNote> Notes {get;}
  }

  partial class TutorialGroup: IHasNotes { }

  partial class TutorialSnippet: IHasNotes { }
}