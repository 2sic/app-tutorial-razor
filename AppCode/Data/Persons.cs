namespace AppCode.Data
{
  public partial class Persons
  {
    /// <summary>
    /// Custom property Presentation - as in these tutorials
    /// the Person always uses a QuickRefContentPresentation content-type for the presentation
    /// </summary>
    public new QuickRefContentPresentation Presentation => _presentation ??= As<QuickRefContentPresentation>(base.Presentation);
    private QuickRefContentPresentation _presentation;
  }
}