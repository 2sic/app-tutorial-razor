using ToSic.Razor.Blade;

namespace AppCode.TutorialSystem.Tabs
{
  public class TabSpecs
  {
    private object body;

    public TabSpecs(string type, string everything): this(type, everything, everything, everything) { }

    public TabSpecs(string type, string label, string value, string original) {
      Type = type;
      Label = label;
      Contents = value;
      Original = original;
    }
    public string DomId => _domId ??= Name2TabId(DisplayName);
    private string _domId;

    /// <summary>
    /// The label, but ATM it's also used to generate DOM IDs,
    /// TODO: SHOULD SPLIT FUNCTIONALITY, SO NICE NAME DOESN'T BREAK DOM
    /// </summary>
    public string Label { get; set; }

    public string DisplayName => _displayName ??= NiceName();
    private string _displayName;

    /// <summary>
    /// The tab contents - can be a reference such as "file:xxx" or a value
    /// </summary>
    public string Contents { get; set; }

    public object Body
    {
      get => body ?? Contents;
      set => body = value;
    }

    public string Original { get; set; }
    public string Type { get; set; }

    public override string ToString() => $"Label: '{Label}'; Value: '{Contents}' ({Type}); #{DomId}";

    private string NiceName() {
      var n = Label;
      // If a known tab identifier, return the nice name
      if (n == Constants.ViewConfigCode) return Constants.ViewConfigTabName;
      if (n == Constants.InDepthField) return Constants.InDepthTabName;
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
        .Replace("\\", "-");
    }
  }
}