using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AirKuma.FileSys {

  // todo: support any non-folder file without extension

  public class FolderWatcher {

    private FileSystemWatcher watcher;
    public List<string> allowedExtensions;
    private readonly string DirectoryPath;

    public FolderWatcher(string directoryPath, List<string> allowedExtensions = null) {
      DirectoryPath = directoryPath;
      watcher = new FileSystemWatcher {
        Path = directoryPath,
        NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
         | NotifyFilters.FileName | NotifyFilters.DirectoryName,
        IncludeSubdirectories = true
      };
      this.allowedExtensions = allowedExtensions;
      //if (allowedExtensions != null) {
      //  watcher.Filter = extensionPattern;
      //}
      watcher.Created += HandleOnCreate;
      watcher.Renamed += HandleOnRename;
      watcher.Changed += HandleOnChanage;
      watcher.Deleted += HandleOnDelete;
    }

    //public event Action<AbsolutePath> OnCreate;
    //public event Action<AbsolutePath, AbsolutePath> OnRename;
    //public event Action<AbsolutePath> OnChange;
    //public event Action<AbsolutePath> OnDelete;

    //============================================================
    // following are only called/reported for most ancestor directory or file

    // called when create new file or for destination of moving file
    protected virtual void OnCreate(AbsPath path) { }

    // called only when in-place rename the basename of the wathcing file
    // not called when move file from one place to another
    protected virtual void OnRename(AbsPath oldPath, AbsPath newPath) { }


    // called when any the watching directory's direct child file or directory is renamed or create or delete (including moving)
    // not called when child file's content is modified
    protected virtual void OnChange(AbsPath path) { }

    // called when delete file or for source of moving file
    protected virtual void OnDelete(AbsPath path) { }

    //============================================================

    private static AbsPath GetPathByString(string fullPathStr) {
      AbsPath path;
      if (System.IO.Directory.Exists(fullPathStr)) {
        path = new FolderPath(fullPathStr);
      }
      else if (System.IO.File.Exists(fullPathStr)) {
        path = new FilePath(fullPathStr);
      }
      else {
        path = new AbsPath(fullPathStr);
      }
      return path;
    }

    private static AbsPath GetPathByEvent(FileSystemEventArgs e) {
      return GetPathByString(e.FullPath);
    }

    protected virtual void OnVisualStudioEdit(string editedFileFullPath) { }

    private void HandleOnCreate(object source, FileSystemEventArgs e) {
      //Console.WriteLine($"on create {e.Name}");
      // VectorExt.cs~RFcd8a484.TMP
      {
        var x = System.IO.Path.GetExtension(e.FullPath);
        if (x == ".TMP") {
          int i = e.FullPath.IndexOf('~');
          var fix = e.FullPath.Substring(0, i);
          this.OnVisualStudioEdit(fix);
        }
      }
      string ext = System.IO.Path.GetExtension(e.FullPath);
      if (allowedExtensions.Contains(ext) || System.IO.Directory.Exists(e.FullPath)) {
        OnCreate(GetPathByEvent(e));
      }
    }
    private void HandleOnRename(object source, RenamedEventArgs e) {
      //Console.WriteLine($"on renmae {e.Name}");
      string ext = System.IO.Path.GetExtension(e.FullPath);
      // todo: detect e.OldFullPath  == "" for directory ?
      if (allowedExtensions.Contains(ext) || System.IO.Path.GetExtension(e.OldFullPath) == "") {
        OnRename(GetPathByString(e.OldFullPath), GetPathByString(e.FullPath));
      }
    }
    private void HandleOnChanage(object source, FileSystemEventArgs e) {
      //Console.WriteLine($"on change {e.Name}");
      string ext = System.IO.Path.GetExtension(e.FullPath);
      if (allowedExtensions.Contains(ext) || System.IO.Directory.Exists(e.FullPath)) {
        OnChange(GetPathByEvent(e));
      }
    }
    private void HandleOnDelete(object source, FileSystemEventArgs e) {
      //Console.WriteLine($"on delete {e.Name}");
      string ext = System.IO.Path.GetExtension(e.FullPath);
      // todo: detect e.OldFullPath  == "" for directory ?
      if (allowedExtensions.Contains(ext) || System.IO.Path.GetExtension(e.FullPath) == "") {
        OnDelete(GetPathByEvent(e));
      }
    }

    public void Start() {
      watcher.EnableRaisingEvents = true;
    }
    public void Stop() {
      watcher.EnableRaisingEvents = false;
    }
    //  try {
    //      Console.WriteLine("============================================================");
    //      Console.WriteLine(e.Name);
    //      Console.WriteLine(System.IO.File.ReadAllText(e.FullPath));
    //      string dest = @"C:\Users\imyou\Desktop\New folder (2)\" + e.Name;

    //      if (System.IO.File.Exists(dest)) {
    //        System.IO.File.Delete(dest);
    //      }
    //System.IO.File.Copy(e.FullPath, dest);
    //      Console.WriteLine("ok");

    //    }
    //    catch (System.Exception) {
    //      Console.WriteLine("failed");
    //    }
  }
}
