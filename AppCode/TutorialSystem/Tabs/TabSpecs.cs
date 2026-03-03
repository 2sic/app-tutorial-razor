using AppCode.Data;
using ToSic.Razor.Blade;

namespace AppCode.TutorialSystem.Tabs
{

  public class TabSpecs
  {
    private object body;

    /// <summary>
    /// New constructor, should be used from now on...
    /// </summary>
    /// <param name="addOn"></param>
    public TabSpecs(TutorialSnippetAddOn addOn) {
      Type = FromAddOn(addOn);
      AddOn = addOn;
      Label = "TT" + addOn.TabTitle; // this will become obsolete, WIP
      Value = addOn.FilePath;
      Original = addOn.FilePath;
    }

    public TabSpecs(TabType type, string everything): this(type, everything, everything, everything) { }

    public TabSpecs(TabType type, string label, string value, string original) {
      Type = type;
      Label = label;
      Value = value;
      Original = original;
    }

    /// <summary>
    /// Dom ID is used to identify the tab in the DOM - both for the activating tab as well as the div with the contents.
    /// </summary>
    public string DomId => _domId ??= Name2TabId(DisplayName);
    private string _domId;

    /// <summary>
    /// The label, but ATM it's also used to generate DOM IDs,
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// The nice display name, derived from the label but shortened and cleaned up
    /// </summary>
    public string DisplayName => _displayName ??= NiceName();
    private string _displayName;

    /// <summary>
    /// The tab contents - can be a reference such as "file:xxx" or a value which is injected later
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// The real body - ic can just be the contents, but it can be a complex, later-injected object
    /// </summary>
    public object Body
    {
      get => body ?? Value;
      set => body = value;
    }

    public string Original { get; }
    public TabType Type { get; }

    public TutorialSnippetAddOn AddOn { get; set;}

    public override string ToString() => $"Label: '{Label}'; Value: '{Value}' ({Type}); #{DomId}";

    private string NiceName() {
      // New 2026-03-03 using add-on, should replace everything else
      if (AddOn != null) {
        if (AddOn.IsNotEmpty(nameof(AddOn.TabTitle)))
          return AddOn.TabTitle;
        if (AddOn.AddOnType == "file" && AddOn.IsNotEmpty(nameof(AddOn.FilePath)))
          return NiceNameNewForAddOn(AddOn.FilePath);
        if (AddOn.AddOnType == "model" && AddOn.IsNotEmpty(nameof(AddOn.FilePath)))
          return "Model: " + NiceNameNewForAddOn(AddOn.FilePath);
        
        return "Err:AddOn-TitleUnclear";
      }
      return NiceName(Label);
    }

    private string NiceNameNewForAddOn(string n) {
      // If a known tab identifier, return the nice name
      // if a file, return the file name only (and on csv, fix a workaround to ensure import/export)
      if (n.EndsWith(".csv.txt"))
        n = n.Replace(".csv.txt", ".csv");
      if (Type == TabType.File || Type == TabType.Model)
        return Text.AfterLast(n, "/") ?? Text.AfterLast(n, ":") ?? n;
      return n;
    }

    private string NiceName(string n) {
      // If a known tab identifier, return the nice name
      // if a file, return the file name only (and on csv, fix a workaround to ensure import/export)
      if (n.EndsWith(".csv.txt"))
        n = n.Replace(".csv.txt", ".csv");
      if (Type == TabType.File || n.StartsWith("file:"))
        return Text.AfterLast(n, "/") ?? Text.AfterLast(n, ":") ?? n;
      return n;
    }

    private static string Name2TabId(string name) {
      return "-" + (name ?? throw new System.ArgumentException("error-name-is-null", nameof(name)))
        .ToLower()
        .Replace(" ", "-")
        .Replace(".", "-")
        .Replace(":", "-")
        .Replace("(", "-")
        .Replace(")", "-")
        .Replace("[", "-")
        .Replace("]", "-")
        .Replace("{", "-")
        .Replace("}", "-")
        .Replace("!", "-")
        .Replace("?", "-")
        .Replace(";", "-")
        .Replace(",", "-")
        .Replace("=", "-")
        .Replace("+", "-")
        .Replace("*", "-")
        .Replace("&", "-")
        .Replace("%", "-")
        .Replace("#", "-")
        .Replace("@", "-")
        .Replace("$", "-")
        .Replace("^", "-")
        .Replace("/", "-")
        .Replace("|", "-")
        .Replace("\\", "-");
    }
  
    private static TabType FromAddOn(TutorialSnippetAddOn addOn) {
      if (addOn.AddOnType == "file")
        return TabType.File;
      if (addOn.AddOnType == "model")
        return TabType.Model;
      if (addOn.AddOnType == "datasource")
        return TabType.DataSource;
      return TabType.Unknown; // default, but should probably be an error
    }

    public string ToAddOnType() {
      if (Type == TabType.File)
        return "file";
      if (Type == TabType.Model)
        return "model";
      return "file";
    }
  }
}