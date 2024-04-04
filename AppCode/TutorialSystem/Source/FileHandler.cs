using ToSic.Eav.Data;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using System.Collections.Generic;
using System;

namespace AppCode.TutorialSystem.Source
{
  public class FileHandler: Custom.Hybrid.CodeTyped
  {
    #region Constants

    const int LineHeightPx = 20;
    const int BufferHeightPx = 20; // for footer scrollbar which often appears

    #endregion

    #region Init / Dependencies

    private SourceProcessor SourceProcessor => _sourceProcessor ??= GetService<SourceProcessor>();
    private SourceProcessor _sourceProcessor;

    private Ace9Editor Ace9Editor => _ace9Editor ??= GetService<Ace9Editor>();
    private Ace9Editor _ace9Editor;

    #endregion


    #region Special ShowResult helpers

    public ITag ShowResultJs(string source) => ShowResult(source, "javascript");
    public ITag ShowResultHtml(string source) => ShowResult(source, "html");
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
      source = SourceProcessor.SourceTrim(source);
      var specs = new ShowSourceSpecs() {
        Processed = source,
        Size = Size(null, source),
        Language = language,
      };
      Ace9Editor.TurnOnSource(specs, "", false);
      return Tag.Div().Class("pre-result").Wrap(
        Ace9Editor.SourceBlockCode(specs)
      );
    }

    #endregion




    #region Show Source Block

    /// <summary>
    /// just show a snippet - used in SourceCode.cs
    /// </summary>
    internal ITag ShowSnippet(string file)
      => ShowFileContents(file, showTitle: false, expand: true);

    internal ITag GetTabFileContents(string file) => ShowFileContents(file, showTitle: true);

    // Used in SourceCode.cs to see if it has tabs
    // internal string GetFileContents(string file) => GetFileAndProcess(file).Contents;

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
    private ITag ShowFileContents(
      string file,
      string title = null,
      string titlePath = null, 
      bool? expand = null,
      bool? wrap = null,
      bool? showTitle = null)
    {
      var l = Log.Call<ITag>("file: '" + file);
      var debug = false;
      var path = "";
      var errPath = "";

      // // New: support for snippetId in path
      // if (snippetId == null && file.Contains("#")) {
      //   snippetId = Text.AfterLast(file, "#");
      //   file = Text.BeforeLast(file, "#");
      // }

      try
      {
        var specs = GetFileAndProcess(file);
        path = specs.Path;  // update in case of error
        errPath = debug ? specs.FullPath : path;
        title ??= "Source Code of " + (Text.Has(specs.FileName)
          ? titlePath + specs.FileName  // "Source code of .../SomeCodeFile.cs"
          : "this " + specs.Type); // "this snippet" vs "this file"
        specs.Expand = expand ?? specs.Expand;
        specs.Wrap = wrap ?? specs.Wrap;
        specs.ShowTitle = showTitle ?? specs.ShowTitle;

        Ace9Editor.TurnOnSource(specs, specs.FileName, specs.Wrap);

        return l(Tag.RawHtml(
          debug ? Tag.Div(errPath).Class("alert alert-info") : null,
          "\n<!-- Source Code -->\n",
          Ace9Editor.SourceBlock(specs, title),
          "\n<!-- /Source Code -->\n"
        ), "ok");
      }
      catch
      {
        throw;
        return Tag.Div("Error showing " + errPath).Class("alert alert-warning");
        return l(ShowError(path), "error");
      }
    }

    internal SourceInfo GetFileAndProcess(string file) {
      var l = Log.Call<SourceInfo>("file: '" + file);
      // Note: for historical reasons the file is a path which can look like
      // - "../../tutorials/razor-quickref\Snip-partials-basic.typed.cshtml"
      // - "../../tutorials/razor-quickref/../razor-partial/line.cshtml"
      // maybe more variations - so we'll try to clean it here to be relative to the AppRoot

      // Todo: remove one or more trailing "../" from the file variable
      if (file.StartsWith("/"))
        file = file.Substring(1);
      
      while (file.StartsWith("../"))
        file = file.Substring(3);

      // var path = App.Folder.Path.Replace("\\", "/"); // /* path ?? */ Path;
      // var before = file;
      // file = Text.After(file, "/tutorials/");
      // var fullPath = GetFileFullPath(Path, file);
      var fullPath = GetPhysicalPathOfFileInApp(file);
      // Log.Add("New Path: " + newPath);
      // throw new Exception($"2dm: before: '{before}'; File: '{file}'; Path '{Path}', AppPath: '{App.Folder.Path}', fullPath: '{fullPath}', tempPathWithHandedInPath: '{tempPathWithHandedInPath}'");

      // When getting cached, we must re-wrap to get a new randomID
      // otherwise the source-display will get confused with multiple displays of the same file
      var cacheKey = fullPath.ToLowerInvariant();
      if (_sourceInfoCache.TryGetValue(cacheKey, out var cached))
        // return but rewrap so changes won't affect the original
        return l(new SourceInfo(cached), "cached");

      // var fileInfo = GetFile(path, file, fullPath);
      var fileInfo = GetFileSourceInfo(fullPath);
      // fileInfo = newInfo;

      // log all properties of FileInfo
      Log.Add($"fileInfo: {fileInfo}");
      // Log.Add($"newInfo: {newInfo}");
      fileInfo.Processed = SourceProcessor.CleanUpSource(fileInfo.Contents);
      fileInfo.Size = Size(null, fileInfo.Processed);
      var isSnippet = false;
      fileInfo.ShowTitle = !isSnippet;
      fileInfo.Type = isSnippet ? "snippet" : "file";
      fileInfo.DomAttribute = "source-code-" + MyContext.Module.Id;
      // If no File name was specified, then it's the current file; don't expand
      if (!file.Has()) fileInfo.Expand = false;
      _sourceInfoCache[cacheKey] = fileInfo;
      return l(fileInfo, "ok");
    }
    private Dictionary<string, SourceInfo> _sourceInfoCache = new Dictionary<string, SourceInfo>();

    private IHtmlTag ShowError(string path) {
      return Tag.RawHtml(
        Tag.H2("Error showing file source"),
        Tag.Div("Where was a problem showing the file source for " + path).Class("alert alert-warning")
      );
    }




    #endregion

    #region new path processing

    private string GetPhysicalPathOfFileInApp(string pathInApp)
    {
      var l = Log.Call<string>(pathInApp);
      // Todo: handle "../" etc.;
      // Note: not possible ATM, because we somehow seem to have these in the path on purpose
      // if (pathInApp.Contains(".."))
      //   throw new Exception("Path contains '..' which is not allowed: " + pathInApp);

      var appPath = App.Folder.PhysicalPath;// + "\\";
      Log.Add("AppPath: " + appPath);
      var pathWithTrimmedFirstSlash = pathInApp.TrimStart(new [] { '/', '\\' });
      var fullPath = System.IO.Path.Combine(appPath, pathWithTrimmedFirstSlash);
      return l(fullPath, fullPath);
    }

    private SourceInfo GetFileSourceInfo(string fullPath) {
      var l = Log.Call<SourceInfo>("fullPath: " + fullPath);
      var cacheKey = fullPath.ToLowerInvariant();
      var fileName = System.IO.Path.GetFileName(fullPath);
      var filePath = fullPath.Substring(0, fullPath.LastIndexOf("/"));
      try
      {
        var contents = _getFileCache.TryGetValue(cacheKey, out var c) ? c : System.IO.File.ReadAllText(fullPath);
        return l(new SourceInfo { FileName = fileName, Path = filePath, FullPath = fullPath, Contents = contents }, fullPath);
      }
      catch (Exception ex)
      {
        throw;
        return l(new SourceInfo { FileName = fileName, Path = filePath, FullPath = fullPath, Contents = ex.Message, IsError = true }, fullPath);
      }
    }
    private readonly Dictionary<string, string> _getFileCache = new Dictionary<string, string>();
    
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


}