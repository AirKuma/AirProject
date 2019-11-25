using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using AirKuma.Text;
using AirKuma.Chrono;

namespace AirKuma.FileSys {

  struct AbsFilePath {
    string pathStr;
    public override string ToString() {
      return pathStr;
    }
  }

  public static class AirFileAssociation {
    public static readonly UnorderedSet<string> PlainTextFiles = new UnorderedSet<string> {
          ".txt",
          ".AirData", ".ini", ".xml", ".opml", ".reg", ".info",
          ".cs",
          ".html", ".css", ".js",
          ".py",
          ".bat", ".ps1", ".vbs", ".ahk",
          ".h", ".hpp", ".cpp",
          ".sln", ".csproj", ".vcxproj", ".props", ".vssettings", ".ruleset",
          ".uproject",
          ".clang-format", ".cmake", ".bff", ".collabignore", ".gitignore",
    };
    public static readonly UnorderedSet<string> RichDocuments = new UnorderedSet<string> {
      ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".chm", ".csv",
    };
    public static readonly UnorderedSet<string> Images = new UnorderedSet<string> {
      ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".tif", ".tiff",
    };
    public static readonly UnorderedSet<string> Audios = new UnorderedSet<string> {
      ".midi", ".wav", ".mp3",
    };
    public static readonly UnorderedSet<string> Videos = new UnorderedSet<string> {
      ".avi", ".mp4",
    };
    public static readonly UnorderedSet<string> LargerProjectFiles = new UnorderedSet<string> {
      ".mb",".hip", ".dwg"
    };
    public static readonly UnorderedSet<string> ComprehensiveFilesToBackup
      = PlainTextFiles
      | RichDocuments
      | Images
      | Audios
      | Videos;

    public static readonly UnorderedSet<string> ExcludedFolderNames = new UnorderedSet<string> {
          // general
          "backup",
          "Build",
          "Tmp",
          "Temp",
          "Temporary",
          "Logs",
          "obj",
          "bin",
          "Cache",
          "Intermediate",
          // hidden ones
          ".vs",
          ".git",
          // for Unity
          "Library",
          "metadata",
          // for UE4
          "ThirdParty",
          "CrashReportClient",
          // for AirKuma's
          "ReportInfo.AirData.AirBackup",
          "AirTextIO.txt.AirBackup",
    };
  }

  public static class PathEx {

    //============================================================
    public static void OpenWithAssociatedApplication(this string filePath) {
      filePath.IsExistingFilePath().Assert();
      // todo: wihtout explorer.exe
      System.Diagnostics.Process.Start("explorer.exe", filePath.ToWindowsNativePath());
    }
    public static void OpenFolderWithFileExplorer(this string folderPath) {
      folderPath.IsExistingFolderPath().Assert();
      //System.Diagnostics.Process p = new System.Diagnostics.Process {
      //  StartInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe")
      //};
      //p.Start();
      System.Diagnostics.Process.Start("explorer.exe", folderPath.ToWindowsNativePath());
    }
    public static void ShowWithFileExplorer(this string path) {
      path.IsExistingPath().Assert();
      //System.Diagnostics.Process p = new System.Diagnostics.Process {
      //  StartInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe")
      //};
      //p.Start();
      System.Diagnostics.Process.Start("explorer.exe", "/select," + path.ToWindowsNativePath());
    }

    //============================================================
    public static void MoveFileTo(this string filePath, string destFilePath) {
      filePath.IsExistingFilePath().Assert();
      destFilePath.IsExistingPath().Not().Assert();
      System.IO.File.Move(filePath, destFilePath);
    }
    public static void MoveFolderTo(this string folderPath, string destFolderPath) {
      folderPath.IsExistingFolderPath().Assert();
      destFolderPath.IsExistingPath().Not().Assert();
      System.IO.Directory.Move(folderPath, destFolderPath);
    }
    public static void MovePath(this string path, string destPath) {
      if (path.IsExistingFilePath()) {
        path.MoveFileTo(destPath);
      } else if (path.IsExistingFolderPath()) {
        path.MoveFolderTo(destPath);
      } else {
        throw new Exception();
      }
    }
    //============================================================
    public static void DeleteFile(this string filePath) {
      filePath.IsExistingFilePath().Assert();
      System.IO.File.Delete(filePath);
    }
    public static void DeleteFolder(this string folderPath) {
      folderPath.IsExistingFolderPath().Assert();
      System.IO.Directory.Delete(folderPath, true);
    }
    public static void DeletePath(this string path) {
      if (path.IsExistingFolderPath()) {
        path.DeleteFolder();
      } else if (path.IsExistingFilePath()) {
        path.DeleteFile();
      } else {
        throw new Exception();
      }
    }
    //============================================================
    public static string TimedWithNow(this string path) {
      return TimedWithDate(path, DateTime.Now);
    }
    public static string TimedWithDate(this string path, DateTime date) {
      string dateTimeStr = date.ToYYYYMMDDHHMMSS('-', '-', '-');
      if (path.HasExtension()) {
        return path.GetBaseName() + $" [{dateTimeStr}]" + path.GetExtension();
      } else {
        return path + $" [{dateTimeStr}]";
      }
    }
    //============================================================
    public static void RenameFolder(this string folderPath, string newFolderPath) {
      folderPath.IsExistingFolderPath().Assert();
      newFolderPath.IsExistingPath().Not().Assert();
      string p1 = folderPath.GetParentPath().GetStandardizedPath();
      string p2 = newFolderPath.GetParentPath().GetStandardizedPath();
      p1.Equals(p2).Assert();
      System.IO.Directory.Move(folderPath, newFolderPath);
    }
    public static void RenameFile(this string filePath, string newFilePath) {
      filePath.IsExistingFilePath().Assert();
      newFilePath.IsExistingPath().Not().Assert();
      string p1 = filePath.GetParentPath().GetStandardizedPath();
      string p2 = newFilePath.GetParentPath().GetStandardizedPath();
      p1.Equals(p2).Assert();
      System.IO.File.Move(filePath, newFilePath);
    }
    public static void RenamePath(this string path, string newPath) {
      if (path.IsExistingFilePath()) {
        path.RenameFile(newPath);
      } else if (path.IsExistingFolderPath()) {
        path.RenameFolder(newPath);
      } else
        throw new Exception();
    }
    //============================================================
    //------------------------------------------------------------
    public static void CopyFileTo(this string filePath, string destFilePath, UnorderedSet<string> includedExtensions) {
      filePath.IsExistingFilePath().Assert();
      destFilePath.IsExistingPath().Not().Assert();
      if (includedExtensions.Contains(filePath.GetExtension()).Not()) {
        return;
      }

      filePath.CopyFileTo(destFilePath);
    }
    public static void CopyFolderTo(this string folderPath, string destFolder, UnorderedSet<string> includedExtensions, UnorderedSet<string> excludedFolderNames) {
      folderPath.IsExistingFolderPath().Assert();
      destFolder.IsExistingPath().Not().Assert();
      if ((excludedFolderNames?.Contains(folderPath.GetFolderName())).IsTrue()) {
        return;
      }

      destFolder.MakeDir();
      foreach (string folder in folderPath.GetChildFolderPaths()) {
        folder.CopyFolderTo(destFolder.CombinePath(folder.GetFolderName()), includedExtensions, excludedFolderNames);
      }
      foreach (string file in folderPath.GetChildFilePaths()) {
        if (includedExtensions is null || includedExtensions.Contains(file.GetExtension())) {
          file.CopyFileTo(destFolder.CombinePath(file.GetFileName()), includedExtensions);
        }
      }
    }
    //------------------------------------------------------------
    public static void CopyPathTo(this string path, string destPath, string extensionFilter = null) {
      destPath.IsExistingPath().Not().Assert();
      if (path.IsExistingFolderPath()) {
        path.CopyFolderTo(destPath, extensionFilter);
      } else if (path.IsExistingFilePath()) {
        if (extensionFilter is null || path.GetExtension() == extensionFilter) {
          path.CopyFileTo(destPath);
        }
      } else {
        throw new Exception();
      }
    }
    //------------------------------------------------------------
    public static void CopyFileTo(this string filePath, string destPath) {
      filePath.IsExistingFilePath().Assert();
      destPath.IsExistingPath().Not().Assert();
      System.IO.File.Copy(filePath, destPath);
    }
    public static void CopyFolderTo(this string folderPath, string destFolder, string extensionFilter = null) {
      folderPath.IsExistingFolderPath().Assert();
      destFolder.IsExistingPath().Not().Assert();
      destFolder.MakeDir();
      foreach (string folder in folderPath.GetChildFolderPaths()) {
        folder.CopyFolderTo(destFolder.CombinePath(folder.GetFolderName()), extensionFilter);
      }
      foreach (string file in folderPath.GetChildFilePaths()) {
        if (extensionFilter is null || file.GetExtension() == extensionFilter) {
          file.CopyFileTo(destFolder.CombinePath(file.GetFileName()));
        }
      }
    }
    //============================================================
    public static string GetNonConflictedPath(this string path) {
      while (path.IsExistingPath()) {
        string ext = path.GetExtension();
        string par = path.GetParentPath();
        string bn = path.GetBaseName();
        if (bn.RegexMatchFirst(@"(.+) \(([0-9]+)\)", out System.Text.RegularExpressions.Match match)) {
          string mainBaseName = match.Groups[1].ToString();
          int ordinal = match.Groups[2].ToString().ParseAsInt();
          path = par.CombinePath(mainBaseName + $" ({ordinal + 1})" + ext);
        } else {
          path = par.CombinePath(bn + " (2)" + ext);
        }
      }
      return path;
    }
    //============================================================
    public static string AirBackup(this string pathToFileOrDir) {
      string fn = pathToFileOrDir.GetFileName();
      string backupDirPath = pathToFileOrDir.GetParentPath().CombinePath($"{fn}.AirBackup");
      backupDirPath.WantDir();
      string backupFilePath = backupDirPath.CombinePath($"{fn}").GetNonConflictedPath();
      pathToFileOrDir.CopyPathTo(backupFilePath);
      return backupFilePath;
    }

    //============================================================
    public static void MakeDir(this string nonExistingFolderPath) {
      nonExistingFolderPath.IsExistingPath().Not().Assert();
      System.IO.Directory.CreateDirectory(nonExistingFolderPath);
    }
    public static void WantDir(this string folderPath) {
      if (!folderPath.IsExistingFolderPath()) {
        folderPath.MakeDir();
      }
    }
    //============================================================
    public static int GetChildFileCount(this string folderPath) {
      return folderPath.GetChildFilePaths().Count();
    }
    public static int GetChildFileCount(this string folderPath, string extension) {
      return folderPath.GetChildFilePaths(extension).Count();
    }
    public static int GetDescendantFileCount(this string folderPath) {
      int c = 0;
      foreach (string folder in folderPath.GetChildFolderPaths()) {
        c += folder.GetDescendantFileCount();
      }
      foreach (string file in folderPath.GetChildFilePaths()) {
        c += 1;
      }
      return c;
    }
    //============================================================

    public static string GetFileName(this string path) {
      return System.IO.Path.GetFileName(path);
    }
    public static string GetFolderName(this string path) {
      return System.IO.Path.GetFileName(path);
    }
    public static string GetBaseName(this string path) {
      //int i = path.Length;
      //for (; i != 0; --i) {
      //  if (path[i - 1] == '.') {
      //    return path.Substring(0, i - 1);
      //  }
      //}
      //return path;
      return System.IO.Path.GetFileNameWithoutExtension(path);
    }
    public static string GetExtension(this string path) {
      return System.IO.Path.GetExtension(path).ToLower();
    }
    public static bool HasExtension(this string str) {
      for (int i = str.Length; i != 0; --i) {
        if (str[i - 1] == '.') {
          return true;
        }
        if (str[i - 1].IsPathSeperator()) {
          return false;
        }
      }
      return false;
    }

    //============================================================
    //private static Demand<Dictionary<string, List<string>>> _cachedChildFilePathes;
    //private static Dictionary<string, List<string>> CachedChildFilePathes => _cachedChildFilePathes.Value;

    //public static List<string> GetCachedChildFilePaths(this string folderPath) {
    //  if (CachedChildFilePathes.TryGetValue(folderPath, out List<string> childPaths))
    //    return childPaths;
    //  var list = folderPath.GetChildFilePaths().ToList();
    //  CachedChildFilePathes.Add(folderPath, list);
    //  return list;
    //}
    //============================================================
    public static IEnumerable<string> GetChildFilePaths(this string path) {
      foreach (string p in System.IO.Directory.GetFiles(path)) {
        yield return p.GetStandardizedPath();
      }
    }
    public static IEnumerable<string> GetChildFilePaths(this string path, string withExtension) {
      foreach (string p in System.IO.Directory.GetFiles(path)) {
        if (p.GetExtension() == withExtension) {
          yield return p.GetStandardizedPath();
        }
      }
    }
    public static IEnumerable<string> GetChildFolderPaths(this string path) {
      foreach (string p in System.IO.Directory.GetDirectories(path)) {
        yield return p.GetStandardizedPath();
      }
    }
    public static IEnumerable<string> GetChildPaths(this string path) {
      foreach (string p in path.GetChildFolderPaths()) {
        yield return p;
      }
      foreach (string p in path.GetChildFolderPaths()) {
        yield return p;
      }
    }
    //------------------------------------------------------------
    public static IEnumerable<string> GetDescendantsFiles(this string path, string extensionFilter = null) {
      path.IsExistingFolderPath().Assert();
      foreach (string subfile in path.GetChildFilePaths()) {
        if (extensionFilter is null || subfile.GetExtension() == extensionFilter) {
          yield return subfile;
        }
      }
      foreach (string subfolder in path.GetChildFolderPaths()) {
        foreach (string matched in GetDescendantsFiles(subfolder, extensionFilter)) {
          yield return matched;
        }
      }
    }
    //============================================================
    public static bool IncludesCommonRootPath(this string path) {
      return path.Count() >= 2 && path[0].IsAsciiLetter() && path[1] == ':' && (path[2] == '/' || path[2] == '\\');
    }

    //============================================================
    public static string CombinePath(this string parentPath, string partialPath) {
      return System.IO.Path.Combine(parentPath, partialPath).GetStandardizedPath();
    }
    //============================================================
    public static bool IsExistingFilePath(this string str) {
      return System.IO.File.Exists(str);
    }
    public static bool IsExistingFolderPath(this string str) {
      return System.IO.Directory.Exists(str);
      //return System.IO.File.GetAttributes(str).HasFlag(System.IO.FileAttributes.Directory);
    }
    public static bool IsExistingPath(this string str) {
      return str.IsExistingFilePath() || str.IsExistingFolderPath();
    }
    //============================================================
    public static string GetParentPath(this string path) {
      return System.IO.Path.GetDirectoryName(path).GetStandardizedPath();
    }
    public static string[] EachNameBetweenSeparators(this string path) {
      return path.GetStandardizedPath().Split('/');
    }
    // does not take . or .. into accounts
    public static string GetRelativePathUnder(this string longerPath, string shorterPath) {
      string[] longerNames = longerPath.EachNameBetweenSeparators();
      string[] shorterNames = shorterPath.EachNameBetweenSeparators();
      (longerNames.Length > shorterNames.Length).Assert();

      int n1 = shorterNames.Length;
      int i;
      for (i = 0; i != n1; ++i) {
        if (longerNames[i] != shorterNames[i]) {
          break;
        }
      }
      int n2 = longerNames.Length;
      var str = new StringBuilder();
      for (; i != n2; ++i) {
        str.Append(longerNames[i]);
        if (i != n2 - 1) {
          str.Append('/');
        }
      }
      return str.ToString();
      //return new System.Uri(shorterPath).MakeRelativeUri(new System.Uri(longerPath)).ToString().GetStandardizedPath();
    }
    //============================================================
    public static bool IsPathSeperator(this char c) {
      return c == '/' || c == '\\';
    }
    //============================================================
    // convert from "/A/B/C/" to "A/B/C"
    public static string GetPathWithoutBoundarySeparators(this string path) {
      int prefixSlashCount = 0;
      for (int i = 0; i != path.Length; ++i) {
        if (path[i].IsPathSeperator()) {
          ++prefixSlashCount;
        } else {
          break;
        }
      }
      int suffixSlashCount = 0;
      for (int i = path.Length; i != 0; --i) {
        if (path[i - 1].IsPathSeperator()) {
          ++suffixSlashCount;
        } else {
          break;
        }
      }
      if (prefixSlashCount != 0 || suffixSlashCount != 0) {
        return path.Substring(prefixSlashCount, path.Length - prefixSlashCount - suffixSlashCount);
      }
      return path;
    }
    // convert from "/A/B\C/D.unity" to "A/B/C/D.unity"
    // also convert from "/./B\../D\" to "./B/../D" without reduce the relactive path marks ('.', '..')
    public static string GetStandardizedPath(this string path) {
      return path.Replace('\\', '/').GetPathWithoutBoundarySeparators();
    }
    public static string GetUniqualizedPath(this string path) {
      return System.IO.Path.GetFullPath(path).GetStandardizedPath();
    }
    public static string ToWindowsNativePath(this string filePath) {
      return filePath.Replace('/', '\\');
    }
    //============================================================

    public static AbsPath GetFileObject(this string path) {
      if (System.IO.Directory.Exists(path)) {
        return new FolderPath(path);
      } else if (System.IO.Directory.Exists(path)) {
        return new FilePath(path);
      } else {
        return new AbsPath(path);
      }
    }
    //============================================================
  }

  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  public abstract class GeneralPath {

    //============================================================
    public string PathStr { get; set; }

    public override string ToString() {
      return PathStr;
    }

    public GeneralPath(string pathStr) { PathStr = pathStr; }

    //public static implicit operator string(GeneralPath path) {
    //  return path.Str;
    //}

    //============================================================

    public string Extension {
      get => System.IO.Path.GetExtension(PathStr);
      set => System.IO.Path.ChangeExtension(PathStr, value);
    }
    public void RemoveExtension() {
      Extension = null;
    }
    public bool HasExtension() {
      return System.IO.Path.HasExtension(PathStr);
    }
    public string FileName => System.IO.Path.GetFileName(PathStr);
    public string BaseName => PathStr.GetBaseName();

    //============================================================

    public bool IsCommonImageFormat() {
      switch (Extension) {
        case ".png":
        case ".jpg":
        case ".jpeg":
        case ".bmp":
        case ".gif":
        case ".tif":
        case ".tiff":
          return true;
        default:
          return false;
      }
    }
    public bool IsCommonDocumentFormat() {
      switch (Extension) {
        case ".txt":
        case ".doc":
        case ".docx":
        case ".html":
        case ".pdf":
          return true;
        default:
          return false;
      }
    }
    public bool IsCommonAudioFormat() {
      switch (Extension) {
        case ".mp3":
        case ".midi":
          return true;
        default:
          return false;
      }
    }
    public bool IsCommonVideoFormat() {
      switch (Extension) {
        case ".mp4":
        case ".avi":
        case ".wmv":
          return true;
        default:
          return false;
      }
    }

    //============================================================

    public FolderPath Parent => new FolderPath(PathStr.GetParentPath());

  }
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  public class RelPath : GeneralPath {
    public RelPath(string pathStr) : base(pathStr.GetStandardizedPath()) {
      Debug.Assert(!pathStr.IncludesCommonRootPath());
    }
  }
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  public class AbsPath : GeneralPath, IEnumerable<AbsPath> {
    public AbsPath(string pathStr) : base(pathStr.GetUniqualizedPath()) {
      // todo: assert
      //Debug.Assert(pathStr.IncludesCommonRootPath());
    }

    // pre-cond: this.Standrardize
    public RelPath ReplaceAsRelativePath(FolderPath shorterFolder) {
      return new RelPath(PathStr.Replace(shorterFolder.PathStr, "."));
    }

    //============================================================
    public bool IsDirectory => PathStr.IsExistingFolderPath();
    public bool IsFile => PathStr.IsExistingFilePath();
    public bool Exists => IsDirectory || IsFile;

    //============================================================

    public virtual void Delete() {
      throw new Exception();
    }
    public virtual AbsPath CopyTo(AbsPath dest, string extensionToFilter = null) {
      throw new Exception("path is at neither a directory nor a file object");
    }
    public virtual AbsPath CopyToFiltered(AbsPath dest, List<string> extensions) {
      throw new Exception("path is at neither a directory nor a file object");
    }
    public AbsPath CopyToDirectory(FolderPath dir, string extensionToFilter = null) {
      return CopyTo(dir.Combine(new RelPath(FileName)), extensionToFilter);
    }
    public AbsPath CopyToDirectoryFiltered(FolderPath dir, List<string> extensions) {
      return CopyToFiltered(dir.Combine(new RelPath(FileName)), extensions);
    }
    public virtual void MoveTo(AbsPath dest) {
      throw new Exception("path is at neither a directory nor a file object");
    }
    public void RenameFileName(string fileName) {
      RenameFullPath(PathStr.GetParentPath().CombinePath(fileName));
    }
    public virtual void RenameFullPath(string fullPath) {
      throw new Exception("path is at neither a directory nor a file object");
    }
    public void CopyOrReplaceTo(AbsPath dest) {
      if (dest.Exists) {
        dest.Delete();
      }
      CopyTo(dest);
    }

    public FolderPath MakeFolder() {
      System.IO.Directory.CreateDirectory(PathStr);
      return new FolderPath(PathStr);
    }

    //============================================================
    public IEnumerator<AbsPath> GetEnumerator() {
      foreach (AbsPath gf in Children) {
        yield return gf;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      foreach (GeneralPath gf in Children) {
        yield return gf;
      }
    }

    public virtual IEnumerable<AbsPath> Children => throw new Exception();
    //============================================================
    public AbsPath Combine(RelPath relativePathOrName) {
      string newPath = PathStr.CombinePath(relativePathOrName.PathStr);
      if (newPath.IsExistingFolderPath()) {
        return new FolderPath(newPath);
      } else if (newPath.IsExistingFilePath()) {
        return new FilePath(newPath);
      }

      return new AbsPath(newPath);
    }
  }
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  public class FilePath : AbsPath {

    public FilePath(string pathStr) : base(pathStr) {
      Debug.Assert(pathStr.IsExistingFilePath());
    }
    public FilePath(AbsPath path) : base(path.PathStr) {
      Debug.Assert(path.PathStr.IsExistingFilePath());
    }

    //============================================================
    public override void Delete() {
      Debug.Assert(Exists);
      System.IO.File.Delete(PathStr);
    }

    public override AbsPath CopyTo(AbsPath dest, string extensionToFilter = null) {
      Debug.Assert(Exists && !dest.Exists);
      if (extensionToFilter is null || Extension == extensionToFilter) {
        System.IO.File.Copy(PathStr, dest.PathStr);
        return new FilePath(dest.PathStr);
      }
      return null;
    }
    public override AbsPath CopyToFiltered(AbsPath dest, List<string> extensions) {
      Debug.Assert(Exists && !dest.Exists);
      if (extensions is null || extensions.Contains(Extension)) {
        System.IO.File.Copy(PathStr, dest.PathStr);
        return new FilePath(dest.PathStr);
      }
      return null;
    }
    public override void RenameFullPath(string fullPath) {
      System.IO.File.Move(PathStr, fullPath);
    }

    public override void MoveTo(AbsPath dest) {
      Debug.Assert(Exists && !dest.Exists);
      System.IO.Directory.Move(PathStr, dest.PathStr);
      PathStr = dest.PathStr;
    }
    //============================================================

    public override IEnumerable<AbsPath> Children {
      get {
        yield break;
      }
    }

    public new bool Exists => System.IO.File.Exists(PathStr);
  }

  public class FolderPath : AbsPath {

    public FolderPath(string pathStr) : base(pathStr) {
      Debug.Assert(pathStr.IsExistingFolderPath());
    }

    public static FolderPath Current => new FolderPath(System.IO.Directory.GetCurrentDirectory());

    // recursively
    public override void Delete() {
      System.IO.Directory.Delete(PathStr, true);
    }
    public override void RenameFullPath(string fullPath) {
      System.IO.Directory.Move(PathStr, fullPath);
    }
    // recursively
    public override AbsPath CopyTo(AbsPath dest, string extensionToFilter = null) {
      Debug.Assert(Exists && !dest.Exists);
      FolderPath destDir = dest.MakeFolder();
      foreach (AbsPath path in Children) {
        if (extensionToFilter is null || !path.IsFile || path.Extension == extensionToFilter) {
          path.CopyTo(destDir.Combine(new RelPath(path.FileName)), extensionToFilter);
        }
      }
      return new FolderPath(dest.PathStr);
    }
    public override AbsPath CopyToFiltered(AbsPath dest, List<string> extensions) {
      Debug.Assert(Exists && !dest.Exists);
      FolderPath destDir = dest.MakeFolder();
      foreach (AbsPath path in Children) {
        if (extensions is null || !path.IsFile || extensions.Contains(path.Extension)) {
          path.CopyToFiltered(destDir.Combine(new RelPath(path.FileName)), extensions);
        }
      }
      return new FolderPath(dest.PathStr);
    }
    // recursively
    public override void MoveTo(AbsPath dest) {
      Debug.Assert(Exists && !dest.Exists);
      System.IO.Directory.Move(PathStr, dest.PathStr);
      PathStr = dest.PathStr;
    }

    public new bool Exists => System.IO.Directory.Exists(PathStr);

    public void MakeFolderEmpty() {
      foreach (AbsPath child in this) {
        child.Delete();
      }
    }

    public override IEnumerable<AbsPath> Children {
      get {
        foreach (string dirPath in System.IO.Directory.GetDirectories(PathStr)) {
          yield return new FolderPath(dirPath);
        }
        foreach (string filePath in System.IO.Directory.GetFiles(PathStr)) {
          yield return new FilePath(filePath);
        }
      }
    }
  }
}
