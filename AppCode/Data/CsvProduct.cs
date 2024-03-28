namespace AppCode.Data
{
  /// <summary>
  /// A strong-typed data model for data from the CSV.
  /// </summary>
  public class CsvProduct: Custom.Data.CustomItem
  {
    public string Name => _item.String("Name");

    public string Description => _item.String("Description");

    public string Link => _item.String("Link");
  }
}