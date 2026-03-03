using System;

namespace AppCode.Data
{
  public partial class TutorialSnippetAddOn
  {
    public bool UseForVariant(string variant) {
      return string.IsNullOrWhiteSpace(Variants) || Variants.IndexOf(variant, StringComparison.InvariantCultureIgnoreCase) >= 0;
    }

    public new string Variants => IsNotEmpty(nameof(Variants))
      ? base.Variants
      : AddOnType == "model" // if it's a model, it's automatically only needed in strong-mode
        ? "strong"
        : "";
  }
}
