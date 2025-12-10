// Best place this in /AppCode/Data or DataSources so it will be pre-compiled together with all code.
namespace AppCode.DataSources
{
  // Inherit from CustomModel which is the current base class for custom data models.
  public class Basic101Model : Custom.Data.CustomModel
  {
    public string Title => _item.String("Title");
    public int TheAnswer => _item.Int("TheAnswer");
  }
}