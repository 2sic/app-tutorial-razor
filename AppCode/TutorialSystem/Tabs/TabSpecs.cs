using ToSic.Razor.Blade;

namespace AppCode.TutorialSystem.Tabs
{

  public class TabSpecs
  {
    private object body;

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

    public override string ToString() => $"Label: '{Label}'; Value: '{Value}' ({Type}); #{DomId}";

    private string NiceName() {
      var n = Label;
      // If a known tab identifier, return the nice name
      // if a file, return the file name only (and on csv, fix a workaround to ensure import/export)
      if (n.EndsWith(".csv.txt")) n = n.Replace(".csv.txt", ".csv");
      if (n.StartsWith("file:")) return Text.AfterLast(n, "/") ?? Text.AfterLast(n, ":");
      return n;
    }

    private static string Name2TabId(string name) {
      return "-" + name.ToLower()
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
  }
}