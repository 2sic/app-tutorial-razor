// This is necessary to use the [VisualQuery] Attribute
using ToSic.Eav.DataSource.VisualQuery;

// Configure some common aspects in Visual Query
// There are more, but they usually don't make sense on a Dynamic DataSource
[VisualQuery(
  // Give it a nice name and UI-Hint in visual query
  NiceName = "Customized DataSource",
  UiHint = "This demonstrates how to customize the visual query integration",
  // Customize the icon - see google material icons for more
  Icon = "opacity",
  // Customize the link on the (?) icon
  HelpLink = "https://2sxc.org",
  // Customize which Content Type to use when configuring this
  ConfigurationType = "WithConfigConfiguration"
)]
public class VisualQueryCustomized : Custom.DataSource.DataSource16
{
  public VisualQueryCustomized(MyServices services) : base(services)
  {
    // Trivial example, as this demo should focus on the VisualQuery
    ProvideOut(() => new {
      Title = "Hello from VisualQueryCustomized",
      TheAnswer = 42,
    });
  }
}
