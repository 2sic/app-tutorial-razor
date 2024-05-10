namespace AppCode.TutorialSystem.Tabs
{
  public class TabSpecs
  {
    private object body;

    public TabSpecs(string type, string domId, string label, string value, string original) {
      DomId = domId;
      Label = label;
      Contents = value;
      Original = original;
      Type = type;
    }
    public string DomId { get; set; }

    /// <summary>
    /// The label, but ATM it's also used to generate DOM IDs,
    /// TODO: SHOULD SPLIT FUNCTIONALITY, SO NICE NAME DOESN'T BREAK DOM
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// The tab contents - can be a reference such as "file:xxx" or a value
    /// </summary>
    public string Contents { get; set; }

    public object Body
    {
      get => body ??= Contents;
      set => body = value;
    }

    public string Original { get; set; }
    public string Type { get; set; }

    public override string ToString() => $"Label: '{Label}'; Value: '{Contents}' ({Type}); #{DomId}";
  }
}