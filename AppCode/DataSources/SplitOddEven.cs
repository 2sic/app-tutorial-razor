using System.Linq;

public class SplitOddEven : Custom.DataSource.DataSource16
{
  public SplitOddEven(MyServices services) : base(services)
  {
    ProvideOut(() => TryGetIn());
    ProvideOut(() => Split().Odd, name: "Odd");
    ProvideOut(() => Split().Even, name: "Even");
  }

  private Cache Split() {
    // If already cached (eg. it's retrieving Even after Odd was already retrieved), return cache
    if (_cache != null)
      return _cache;

    // Make sure we have an In stream - otherwise return an error
    var inStream = TryGetIn();
    if (inStream == null) return new Cache { Odd = Error.TryGetInFailed(), Even = Error.TryGetInFailed() };

    // Build cache so we don't have to re-calculate when retrieving other streams
    return _cache = new Cache {
      Odd = inStream.Where(e => e.EntityId % 2 == 1),
      Even = inStream.Where(e => e.EntityId % 2 == 0)
    };
  }

  private Cache _cache;

  private class Cache {
    public object Odd;
    public object Even;
  }
}