using System.Linq;

public class KeepOdd : Custom.DataSource.DataSource16
{
  public KeepOdd(Dependencies services) : base(services)
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
