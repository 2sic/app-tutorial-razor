using System;
using System.Linq;

namespace AppCode.TutorialSystem
{
  /// <summary>
  /// A Tutorial ID can be something like "data-csv-query-base"
  /// which means that it's in "data-csv/Snip-query-base.cshtml"
  /// </summary>
  public class TutorialIdToPath {
    public TutorialIdToPath(string tutorialId, string variant) {
      TutorialId = tutorialId;
      Variant = variant;
      var parts = tutorialId.Split('-');
      if (parts.Length > 1) Folder = parts[0] + "-" + parts[1];
      else throw new Exception("Second path is empty, original was '" + tutorialId + "'");
      if (parts.Length > 2) Rest = string.Join("-", parts.Skip(2));
    }
    public string TutorialId { get; set; } = "";
    public string Variant {get;set;}
    public string Folder { get; set; } = "";
    public string Rest { get; set; } = "";
    public string Path => "tutorials/" + Folder;
    public string FileName => "Snip-" + Rest + Variant + ".cshtml";

    public string FullPath => Path + "/" + FileName;
  }

}