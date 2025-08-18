using System.Collections.Generic;
using System.Linq;

// v20
using ToSic.Eav.Data;

public class TreeFoldersAndFiles : Custom.DataSource.DataSource16
{
  public TreeFoldersAndFiles(Dependencies services) : base(services)
  {
    // The "Default" stream contains both files and folders
    ProvideOut(() => {
      return new List<object> {
        CreateFolder("/", ""),            // Root Folder
          CreateFolder("/", "101"),       // Subfolder 101
            CreateFolder("/101", "1011"),
            CreateFolder("/101", "1012"),
            CreateFile("/101", "Text in 101.txt"),
          CreateFolder("/", "102"),       // Subfolder 102
          CreateFile("/", "Test.txt"),    // File in Root folder
          CreateFile("/", "Image.jpg"),   // File in Root Folder
      };
    });
    ProvideOut(() => TryGetOut("Default").Where(f => !f.Get<bool>("IsFile")), name: "Folders");
    ProvideOut(() => TryGetOut("Default").Where(f => f.Get<bool>("IsFile")), name: "Files");
  }


  private object CreateFile(string path, string name) {
    var parentPath = path.ToLowerInvariant();
    var fullPath = (parentPath + "/" + name).ToLowerInvariant();
    return new {
      IsFile = true,
      Path = fullPath,
      Title = "File " + name,

      // Parent is the entity (one expected) which has a key saying they are this folder
      Parent = new { Relationships = "folder:" + parentPath },

      // Declare keys for anything that wants a relationship to this file
      RelationshipKeys = new [] {
        "file:" + fullPath,       // things that explicitly need this file
        "file-in:" + parentPath   // the parent folder will look for all of its files
      },
    };
  }
  private object CreateFolder(string parent, string name) {
    parent = parent.ToLowerInvariant();
    var path = (parent + (parent.EndsWith("/") ? "" : "/") + name).ToLowerInvariant();
    var parentPath = (path == "/" ? "" : parent).ToLowerInvariant();
    return new {
      IsFile = false,
      Path = path,
      Title = "Folder '" + path + "'",
      // Files should list all files which have this folder as parent
      Files = new { Relationships = "file-in:" + path },
      // Folders should list all folders which have this folder as parent
      Folders = new { Relationships = "folder-in:" + path },
      // Parent should point to the folder which is the parent of this folder
      Parent = new { Relationships = "folder:" + path },

      // Declare keys for anything that wants a relationship to this folder
      RelationshipKeys = new [] {
        "folder:" + path,           // things that explicitly need this folder
        "folder-in:" + parentPath,  // the parent folder will look for all of its files
      },
    };
  }
}
