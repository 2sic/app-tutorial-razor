using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AppCode.Source;

// 2sxclint:disable:avoid-dynamic

public class FileHandler: Custom.Hybrid.CodeTyped
{
  #region Constants

  const int LineHeightPx = 20;
  const int BufferHeightPx = 20; // for footer scrollbar which often appears

  #endregion

  #region Init / Dependencies

  public FileHandler Init(string path) {
    Path = path;
    SourceProcessor = GetService<SourceProcessor>();
    return this;
  }
  public string Path { get; set; }
  private /* dynamic */ SourceProcessor SourceProcessor { get; set; }

  #endregion


  #region Special ShowResult helpers

  public ITag ShowResultJs(string source) { return ShowResult(source, "javascript"); }
  public ITag ShowResultHtml(string source) { return ShowResult(source, "html"); }
  // public ITag ShowResultText(string source) { return ShowResult(source, "text"); }
  
  // Special use case for many picture / image tutorials
  public ITag ShowResultImg(object tag) {
    var cleaned = tag.ToString()
      .Replace(">", ">\n")
      .Replace("' ", "' \n")
      .Replace(",", ",\n");
    return ShowResultHtml(cleaned);
  }

  // public ITag ShowResultPic(object tag) {
  //   var cleaned = tag.ToString()
  //       .Replace(">", ">\n")
  //       .Replace(",", ",\n")
  //       .Replace(" alt=", "\nalt=")
  //       .Replace("' ", "' \n");
  //   return ShowResultHtml(cleaned);
  // }

  private ITag ShowResult(string source, string language) {
    source = SourceProcessor.SourceTrim(source) as string;
    var specs = new ShowSourceSpecs() {
      Processed = source,
      Size = Size(null, source),
      Language = language,
    };
    TurnOnSource(specs, "", false);
    return Tag.Div().Class("pre-result").Wrap(
      SourceBlockCode(specs)
    );
  }

  #endregion




  #region Show Source Block

  /// <summary>
  /// Not used any more!
  /// ...then remove
  /// </summary>
  /// <param name="file"></param>
  /// <param name="titlePath"></param>
  /// <returns></returns>
  // public ITag ShowFile(string file, string titlePath = null) {
  //   return ShowFileContents(file, titlePath: titlePath);
  // }

  /// <summary>
  /// just show a snippet - used in SourceCode.cs
  /// </summary>
  public ITag ShowSnippet(string snippetId, ITypedItem item = null, string file = null) {
    if (file != null)
      return ShowFileContents(file, snippetId, withIntro: false, showTitle: false, expand: true);
    return ShowFileContents(null, snippetId, expand: true);
  }

  public ITag GetTabFileContents(string file)
  {
    return ShowFileContents(file, withIntro: false, showTitle: true);
  }

  // Used in SourceCode.cs to see if it has tabs
  public string GetFileContents(string file){
    return GetFileAndProcess(file).Contents;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="file">The file name - if empty string, will use current file name</param>
  /// <param name="snippetId"></param>
  /// <param name="title"></param>
  /// <param name="titlePath"></param>
  /// <param name="expand"></param>
  /// <param name="wrap"></param>
  /// <param name="withIntro"></param>
  /// <param name="showTitle"></param>
  /// <returns></returns>
  private ITag ShowFileContents(string file,
    string snippetId = null, string title = null, string titlePath = null, 
    bool? expand = null, bool? wrap = null, bool? withIntro = null, bool? showTitle = null)
  {
    var l = Log.Call<ITag>("file: '" + file + "'; SnippetId: '" + snippetId + "'");
    var debug = false;
    var path = Path;
    var errPath = Path;

    // New: support for snippetId in path
    if (snippetId == null && file.Contains("#")) {
      snippetId = Text.AfterLast(file, "#");
      file = Text.BeforeLast(file, "#");
    }

    try
    {
      var specs = GetFileAndProcess(file, snippetId);
      path = specs.Path;  // update in case of error
      errPath = debug ? specs.FullPath : path;
      title = title ?? "Source Code of " + (Text.Has(specs.FileName)
        ? titlePath + specs.FileName  // "Source code of .../SomeCodeFile.cs"
        : "this " + specs.Type); // "this snippet" vs "this file"
      specs.Expand = expand ?? specs.Expand;
      specs.Wrap = wrap ?? specs.Wrap;
      specs.ShowIntro = withIntro ?? specs.ShowIntro;
      specs.ShowTitle = showTitle ?? specs.ShowTitle;

      TurnOnSource(specs, specs.FileName, specs.Wrap);

      return l(Tag.RawHtml(
        debug ? Tag.Div(errPath).Class("alert alert-info") : null,
        "\n<!-- Source Code -->\n",
        SourceBlock(specs, title),
        "\n<!-- /Source Code -->\n"
      ), "ok");
    }
    catch
    {
      throw;
      return Tag.Div("Error showing " + errPath).Class("alert alert-warning");
      return l(ShowError(path), "error");
    }
    // return null;
  }

  public bool FileContainsSnipCode(string file) {
    try {
      var specs = GetFileAndProcess(file);
      return specs.Contents.Contains("snip = Sys.SourceCode.");
    } catch {
      return false;
    }
  }

  private SourceInfo GetFileAndProcess(string file, string snippetId = null, string path = null) {
    path = path ?? Path;
    var fullPath = GetFileFullPath(path, file);
    var cacheKey = (fullPath + "#" + snippetId).ToLowerInvariant();
    // When getting cached, we must re-wrap to get a new randomID
    // otherwise the source-display will get confused with muliple displays of the same file
    if (_sourceInfoCache.ContainsKey(cacheKey))
      return new SourceInfo(_sourceInfoCache[cacheKey]);
    var fileInfo = GetFile(path, file, fullPath);
    fileInfo.Processed = SourceProcessor.CleanUpSource(fileInfo.Contents, snippetId);
    fileInfo.Size = Size(null, fileInfo.Processed);
    var isSnippet = !string.IsNullOrWhiteSpace(snippetId);
    fileInfo.ShowIntro = !isSnippet;
    fileInfo.ShowTitle = !isSnippet;
    fileInfo.Type = isSnippet ? "snippet" : "file";
    fileInfo.DomAttribute = "source-code-" + MyContext.Module.Id;
    // If no File name was specified, then it's the current file; don't expand
    if (!snippetId.Has() && !file.Has()) fileInfo.Expand = false;
    _sourceInfoCache[cacheKey] = fileInfo;
    return fileInfo;
  }
  private Dictionary<string, SourceInfo> _sourceInfoCache = new Dictionary<string, SourceInfo>();

  private IHtmlTag ShowError(string path) {
    return Tag.RawHtml(
      Tag.H2("Error showing file source"),
      Tag.Div("Where was a problem showing the file source for " + path).Class("alert alert-warning")
    );
  }



  private Div SourceBlock(ShowSourceSpecs specs, string title) {
    return Tag.Div().Class("code-block " + (specs.Expand ? "is-expanded" : "")).Attr(specs.DomAttribute).Wrap(
      specs.ShowIntro
        ? Tag.Div().Class("header row justify-content-between").Wrap(
            Tag.Div().Class("col-11").Wrap(
              Tag.H3(title),
              Tag.P("Below you'll see the source code of the " + specs.Type + @". 
                    Note that we're just showing the main part, and hiding some parts of the file which are not relevant for understanding the essentials. 
                    <strong>Click to expand the code</strong>")
            ),
            Tag.Div().Class("col-auto").Wrap(
              // Up / Down arrows as SVG - hidden by default, become visible based on CSS 
              Tag.Custom("<img src='" + App.Folder.Url + "/assets/svg/arrow-up.svg' class='fa-chevron-up' loading='lazy'>"),
              Tag.Custom("<img src='" + App.Folder.Url + "/assets/svg/arrow-down.svg' class='fa-chevron-down' loading='lazy'>")
            )
          ) as ITag
        : specs.ShowTitle
          ? Tag.H3(title) as ITag
          : Tag.Span(),
      "\n<!-- Raw Source in Pre -->\n",
      SourceBlockCode(specs),
      "\n<!-- /Raw Source in Pre -->\n"
    );
  }



  private ITag SourceBlockCode(ShowSourceSpecs specs) {
    return Tag.Div().Class("source-code").Wrap(
      "\n",
      Tag.Pre(Tags.Encode(specs.Processed)).Id(specs.RandomId).Style("height: " + specs.Size + "px; font-size: 16px"),
      "\n"
    );
  }

  private void TurnOnSource(ShowSourceSpecs specs, string filePath, bool wrap) {
    var l = Log.Call("filePath:" + filePath + ", wrap:" + wrap + "; specs.Language: " + specs.Language);
    var language = "ace/mode/" + (specs.Language ?? (Text.Has(filePath)
      ? FindAce3LanguageName(filePath)
      : "html"));

    Kit.Page.TurnOn("window.razorTutorial.initSourceCode()",
      require: "window.ace",
      data: new {
        test = "now-automated",
        domAttribute = specs.DomAttribute,
        aceOptions = new {
          wrap,
          language,
          sourceCodeId = specs.RandomId
        }
      }
    );
    l("language=" + language);
  }

  /// <summary>
  /// Determine the ace9 language of the file
  /// </summary>
  private string FindAce3LanguageName(string filePath) {
    var extension = filePath.Substring(filePath.LastIndexOf('.') + 1);
    switch (extension)
    {
      case "cs": return "csharp";
      case "js": return "javascript";
      case "json": return "json";
      default: return "razor";
    }
  }


  #endregion

  #region (private/internal) File Processing

  private string GetFileFullPath(string filePath, string file) {
    var l = Log.Call<string>("filePath:" + filePath + ", file:" + file);
    if (Text.Has(file)) {
      if (file.IndexOf(".") == -1)
        file = file + ".cshtml";
      var lastSlash = filePath.LastIndexOf("/");
      filePath = filePath.Substring(0, lastSlash) + "/" + file;
    }
    var fullPath = filePath;
    if (filePath.IndexOf(":") == -1 && filePath.IndexOf(@"\\") == -1)
      fullPath = GetFullPath(filePath);
    return l(fullPath, fullPath);
  }

  private SourceInfo GetFile(string filePath, string file, string fullPath = null) {
    var l = Log.Call<SourceInfo>("filePath:" + filePath + ", file:" + file + "; fullPath: " + fullPath);
    fullPath = fullPath ?? GetFileFullPath(filePath, file);
    var cacheKey = fullPath.ToLowerInvariant();
    var contents = (_getFileCache.ContainsKey(cacheKey))
      ? _getFileCache[cacheKey]
      : System.IO.File.ReadAllText(fullPath);
    var fileName = System.IO.Path.GetFileName(fullPath);
    return l(new SourceInfo { FileName = fileName, Path = filePath, FullPath = fullPath, Contents = contents }, fullPath);
  }

  private Dictionary<string, string> _getFileCache = new Dictionary<string, string>();


  internal class ShowSourceSpecs {
    public ShowSourceSpecs() {
      RandomId = "source" + Guid.NewGuid().ToString();
    }
    // public static ShowSourceSpecs CloneWithNewId(ShowSourceSpecs o) {
    //   return new ShowSourceSpecs {
    //     Processed = o.Processed,
    //     Size = o.Size,
    //     Language = o.Language,
    //     Type = o.Type,
    //     DomAttribute = o.DomAttribute,
    //     ShowIntro = o.ShowIntro,
    //     ShowTitle = o.ShowTitle,
    //     Expand = o.Expand,
    //     Wrap = o.Wrap
    //   }
    // }
    public string Processed;
    public int Size;
    public string Language;
    public string Type = "file";
    public string DomAttribute;
    public string RandomId;
    public bool ShowIntro;
    public bool ShowTitle;
    public bool Expand = true;
    public bool Wrap;
  }

  internal class SourceInfo : ShowSourceSpecs {
    public SourceInfo() { }
    public SourceInfo(SourceInfo o) {
      Processed = o.Processed;
      Size = o.Size;
      Language = o.Language;
      Type = o.Type;
      DomAttribute = o.DomAttribute;
      ShowIntro = o.ShowIntro;
      ShowTitle = o.ShowTitle;
      Expand = o.Expand;
      Wrap = o.Wrap;
      // field only in SourceInfo
      FileName = o.FileName;
      Path = o.Path;
      FullPath = o.FullPath;
      Contents = o.Contents;
    }
    public string FileName;
    public string Path;
    public string FullPath;
    public string Contents;
  }

  public string GetFullPath(string filePath) {
    var l = Log.Call<string>(filePath);
    #if NETCOREAPP
      // This is the Oqtane implementation - cannot use Server.MapPath
      // 2sxclint:disable:v14-no-getservice
      var hostingEnv = GetService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>();
      var pathWithTrimmedFirstSlash = filePath.TrimStart(new [] { '/', '\\' });
      var result = System.IO.Path.Combine(hostingEnv.ContentRootPath, pathWithTrimmedFirstSlash);
    #else
      var result = System.Web.HttpContext.Current.Server.MapPath(filePath);
    #endif
    return l(result, result);
  }

  #endregion

  #region (private) Source Code Clean-up Helpers

  // Auto-calculate Size
  private int Size(object sizeObj, string source) {
    var size = Kit.Convert.ToInt(sizeObj, fallback: -1);
    if (size == -1) {
      var sourceLines = source.Split('\n').Length;
      size = sourceLines * LineHeightPx + BufferHeightPx;
    }

    if (size < LineHeightPx) size = 600;
    return size;
  }

  #endregion

}
