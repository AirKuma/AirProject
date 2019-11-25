using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using AirKuma.FileSys;

namespace AirKuma.FileSys {

  public class FolderSync : FolderWatcher {

    private FolderPath srcFolder;
    private FolderPath destFolder;
    public FolderSync(FolderPath sourceFolderPath, FolderPath destFolderPath, List<string> allowedExtensions = null)
        : base(sourceFolderPath.PathStr, allowedExtensions) {
      //if (sourceExtensionPattern != null)
      //  throw new NotImplementedException();
      srcFolder = sourceFolderPath;
      destFolder = destFolderPath;
    }

    public void Init() {
      // todo: by extension
      destFolder.MakeFolderEmpty();
      foreach (AbsPath srcChild in srcFolder) {
        srcChild.CopyToDirectoryFiltered(destFolder, allowedExtensions);
      }
      //destFolder.Delete();
      //destFolder.MakeFolder();
    }

    protected override void OnCreate(AbsPath path) {
      if (path.Extension == "" || allowedExtensions.Contains(path.Extension)) {
        RelPath relPath = path.ReplaceAsRelativePath(srcFolder);
        AbsPath destPath = destFolder.Combine(relPath);
        Console.WriteLine($"create '{relPath}' (from '{path.PathStr}' to '{destPath.PathStr}')");
        path.CopyToFiltered(destPath, allowedExtensions);
      }
    }

    protected override void OnRename(AbsPath oldPath, AbsPath newPath) {
      // todo: test oldPath.Extension == "" for directory?
      if ((oldPath.Extension == "" && newPath.Extension == "")
          || (allowedExtensions.Contains(oldPath.Extension) && allowedExtensions.Contains(newPath.Extension))) {
        RelPath relOldPath = oldPath.ReplaceAsRelativePath(srcFolder);
        RelPath relNewPath = newPath.ReplaceAsRelativePath(srcFolder);
        AbsPath destOldPath = destFolder.Combine(relOldPath);
        AbsPath destNewPath = destFolder.Combine(relNewPath);
        Console.WriteLine($"rename '{relOldPath}' as '{relNewPath}' (from '{destOldPath}' to '{destNewPath}')");
        destOldPath.RenameFullPath(destNewPath.PathStr);
      }
    }
    protected override void OnChange(AbsPath path) {
      if (allowedExtensions.Contains(path.Extension)) {
        RelPath relPath = path.ReplaceAsRelativePath(srcFolder);
        Console.WriteLine($"change '{relPath}'");
        AbsPath destPath = destFolder.Combine(relPath);
        if (path is FilePath srcFile) {
          if (destPath.Exists) {
            destPath.Delete();
          }
          srcFile.CopyToFiltered(destPath, allowedExtensions);
        }
      }
    }
    protected override void OnVisualStudioEdit(string editedFileFullPath) {
      if (System.IO.File.Exists(editedFileFullPath)) {
        OnChange(new FilePath(editedFileFullPath));
      }
    }

    protected override void OnDelete(AbsPath path) {
      if (path.Extension == "" || allowedExtensions.Contains(path.Extension)) {
        RelPath relPath = path.ReplaceAsRelativePath(srcFolder);
        Console.WriteLine($"delete '{relPath}'");
        AbsPath destPath = destFolder.Combine(relPath);
        destPath.Delete();
      }
    }
  }
}
