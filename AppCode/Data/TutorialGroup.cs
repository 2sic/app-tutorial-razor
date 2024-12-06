
using System.Linq;

namespace AppCode.Data
{
  public partial class TutorialGroup
  {
    // Add your own properties and methods here

    public bool HasVariantsDeep => HasVariants || Accordions.Any(a => a.HasVariants);

    public TutorialGroup ParentOrChildWithVariants => HasVariants
      ? this
      : Accordions.FirstOrDefault(a => a.HasVariants);
  }
}
