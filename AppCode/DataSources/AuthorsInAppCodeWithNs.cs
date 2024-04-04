using System.Linq;
using ToSic.Eav.DataSources;

public class AuthorsInAppCodeWithNs : Custom.DataSource.DataSource16
{
  public AuthorsInAppCodeWithNs(MyServices services) : base(services)
  {
    ProvideOut(() => {
      // Get the app data
      var appData = Kit.Data.GetAppSource();

      // Get the Content-Type Filter DataSource
      var contentTypeFilter = Kit.Data.GetSource<EntityTypeFilter>(attach: appData, parameters: new { TypeName = "Persons"});

      // Return all the items after filtering
      return contentTypeFilter.List;
    });
  }
}
