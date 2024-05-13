namespace AppCode.TutorialSystem.Tabs
{
  // TODO: CHANGE SYSTEM
  // to use this type to detect any special behavior, and
  // eg. move model-specific show-code to the model tab
  // so that the model-to-file-name happens much later
  // and we're able to use multiple model files
  public enum TabType
  {
    Unknown,
    /// <summary>
    /// FromCode tabs must be filled by the code to show anything
    /// </summary>
    FromCode,
    File,
    Model,
    DataSource,
    RazorModel,

    Formulas,
    ViewConfig,
    InDepth,

    Notes,

    Results,
    ResultsAndSource,
    Source,

    TutorialReferences,

  }
}