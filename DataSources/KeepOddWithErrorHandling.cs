using System.Linq;

public class KeepOddWithErrorHandling : Custom.DataSource.DataSource16
{
  public KeepOddWithErrorHandling(MyServices services) : base(services)
  {
    ProvideOut(() => {
      // Make sure we have an In stream - otherwise return an error
      var inStream = TryGetIn();
      if (inStream == null) return Error.TryGetInFailed();

      // Return only odd items
      return inStream.Where(e => e.EntityId % 2 == 1);
    });
  }
}
