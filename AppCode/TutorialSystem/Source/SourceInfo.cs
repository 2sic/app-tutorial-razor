using System;

namespace AppCode.TutorialSystem.Source
{
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

    public override string ToString()
    {
      return $"{nameof(ShowSourceSpecs)} - Processed: {Processed}; Size: {Size}; Language: {Language}; Type: {Type}; DomAttribute: {DomAttribute}; RandomId: {RandomId}; ShowIntro: {ShowIntro}; ShowTitle: {ShowTitle}; Expand: {Expand}; Wrap: {Wrap}";
    }
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

    public override string ToString()
    {
      return $"{nameof(SourceInfo)} - FileName: {FileName}; Path: {Path}; FullPath: {FullPath}; Contents: {Contents}; {base.ToString()}";
    }
  }
}