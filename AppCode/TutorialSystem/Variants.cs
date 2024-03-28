using System.Collections.Generic;
using System.Linq;
using ToSic.Razor.Blade;

namespace AppCode.TutorialSystem
{
  public class Variants
  {
    public const string VariantTyped = "typed";
    public const string VariantDyn = "dyn";
    public const string VariantStrong = "strong";

    public const string VariantsDefault = VariantTyped + "," + VariantDyn;

    public const string VariantUrlParameter = "variant";

  }

  public class VariantsService : AppCode.Services.ServiceBase
  {
    public List<VariantButtons> GetVariantButtons(string list, string current)
    {
        var variants = Text.First(list, Variants.VariantsDefault);

        var variantButtons = variants.Split(',').Select(variant => {
          var variantIsSelected = variant == current;
          var classes = variantIsSelected ? "btn btn-success" : "btn btn-primary";
          var newVariant = variantIsSelected ? "" : variant;
          var href = variantIsSelected ? "#" : Link.To(parameters: MyPage.Parameters.Set(Variants.VariantUrlParameter, newVariant));
          var label = "";
          switch (variant)
          {
            case Variants.VariantStrong:
              if (variantIsSelected) {
                label = "Selected: Strong-Typed (2sxc 17.05+)";
              } else {
                label = "Switch to Strong-Typed (2sxc 17.05+)";
              }
              break;
            case Variants.VariantTyped:
              if (variantIsSelected) {
                label = "Selected: Typed (2sxc 16+)";
              } else {
                label = "Switch to Typed (2sxc 16+)";
              }
              break;
            case Variants.VariantDyn:
              if (variantIsSelected) {
                label = "Selected: Dynamic (Razor14 or below)";
              } else {
                label = "Switch to Dynamic (Razor14 or below)";
              }
              break;
            case "pre12":
              if (variantIsSelected) {
                label = "Selected: Pre Razor12";
              } else {
                label = "Switch to Pre Razor12";
              }
              break;
          }
          return new VariantButtons { Classes = classes, Link = href, Label = label };
        })
        .ToList();

        return variantButtons;
    }
  }

  public class VariantButtons
  {
    public string Label { get; set; }
    public string Classes { get; set; }
    public string Link { get; set; }
  }
}