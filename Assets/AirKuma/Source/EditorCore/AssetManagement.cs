using System;
using System.Collections.Generic;
using System.Text;
using AirKuma.FileSys;
using UnityEditor;
using UnityEngine;

// unused functions

// IsOpenForEdit
// static AssetDatabase.SaveAssets
// static AssetDatabase.Refresh
//AssetDatabase.StartAssetEditing
//AssetDatabase.StopAssetEditing

// other functions about asset bundles, package

// class UnityEditor.VersionControl.Asset 

namespace AirKuma.UnityCore {

  public static class AssetDatabaseEx {
    public static bool HasAssetAtPath(string assetPath) {
      return System.IO.File.Exists(assetPath);
    }
    public static string GetPathExtension(this string assetPath) {
      return assetPath.GetExtension();
    }
    public static T LoadOrCreateAsset<T>(string assetPath, params object[] constructorArgs)
        where T : UnityEngine.Object {
      if (HasAssetAtPath(assetPath))
        return AssetDatabase.LoadAssetAtPath<T>(assetPath);
      else {
        var uo = (T)Activator.CreateInstance(typeof(T), constructorArgs);
        AssetDatabase.CreateAsset(uo, assetPath);
        return uo;
      }
    }
    //public static void Replace(UnityEngine.Object assetObj, string assetPath) {
    //  if (HasAssetAtPath(assetPath)) {
    //    var uo = AssetDatabase.LoadMainAssetAtPath(assetPath);
    //    uo = assetObj;
    //    AssetDatabase.SaveAssets();
    //  }
    //  else
    //    AssetDatabase.CreateAsset(assetObj, assetPath);
    //}
    public static void Recreate(UnityEngine.Object assetObj, string assetPath) {
      if (HasAssetAtPath(assetPath))
        AssetDatabase.DeleteAsset(assetPath);
      AssetDatabase.CreateAsset(assetObj, assetPath);
    }
    public static T Recreate<T>(string assetPath, params object[] constructorArgs)
        where T : UnityEngine.Object {
      if (HasAssetAtPath(assetPath)) {
        AssetDatabase.DeleteAsset(assetPath);
      }
      var uo = (T)Activator.CreateInstance(typeof(T), constructorArgs);
      AssetDatabase.CreateAsset(uo, assetPath);
      return uo;
    }
  }

  public static class AssetResolver {
    private static Dictionary<string, UnityEngine.Object> dict;
    static AssetResolver() {
      dict = new Dictionary<string, UnityEngine.Object>();
    }
    public static UnityEngine.Object Retrieve(string assetPath) {
      if (!dict.TryGetValue(assetPath, out UnityEngine.Object assetObj)) {
        assetObj = AssetDatabase.LoadMainAssetAtPath(assetPath);
        dict.Add(assetPath, assetObj);
      }
      return assetObj;
    }
    public static void InvalidateCache() {
      dict.Clear();
    }
  }


  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  //public class AssetFilePath : Kuma.RelativePath {
  //  // under "Assets/" directory

  //  public AssetFilePath(Kuma.AbsolutePath path) : base(path.GetRelativePathUnder(new Directory(Application.dataPath)).Str) {
  //  }

  //  //public AssetFilePath(string relativeFilePath) : base(relativeFilePath) {
  //  //  //Debug.Assert(relativeFilePath.StartsWith("Assets/") || relativeFilePath.StartsWith("/Assets/"));
  //  //}

  //  public static implicit operator AssetFilePath(string path) {
  //    var newOne = new AssetFilePath(path);
  //    Debug.Assert(newOne.Valid);
  //    return newOne;
  //  }
  //  public static implicit operator string(AssetFilePath path) {
  //    Debug.Assert(path.Valid);
  //    return path.Str;
  //  }

  //  public bool Valid => Str.StartsWith("Assets/") || Str.StartsWith("/Assets");
  //  public bool General => Str.EndsWith(".asset");


  //}
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  public static class AssetExtensions {

    // relAssetPath statrs with "Assets/"
    public static string FullAssetPath(this string relAssetPath) {
      return Application.dataPath.GetParentPath().CombinePath(relAssetPath);
    }

    //============================================================
    public static bool IsPrefabRoot(this GameObject obj) {
      return PrefabUtility.IsAnyPrefabInstanceRoot(obj);
    }
    public static void Revert(this GameObject obj) {
      Debug.Assert(obj.IsPrefabRoot());
      PrefabUtility.RevertPrefabInstance(obj, InteractionMode.AutomatedAction);
    }
    public static GameObject OutmostPrefabRoot(this GameObject obj) {
      return PrefabUtility.GetOutermostPrefabInstanceRoot(obj) ?? throw new Exception();
    }


    //============================================================
    private static void AssetsWithGOs(Dictionary<string, GameObject> result, GameObject underGO, string underAssetFolder, string assetFileExtension, bool removeExtension) {
      foreach (string file in underAssetFolder.GetChildFilePaths(assetFileExtension)) {
        string name = removeExtension ? file.GetBaseName() : file.GetFileName();
        result.Add(underAssetFolder.CombinePath(file.GetFileName()), underGO.GetOrAddChild(name));
      }
      foreach (string folder in underAssetFolder.GetChildFolderPaths()) {
        AssetsWithGOs(result, underGO.GetOrAddChild(folder.GetBaseName()), folder, assetFileExtension, removeExtension);
      }
    }
    public static Dictionary<string, GameObject> AssetsWithGOs(this GameObject go, string underAssetFolder, string assetFileExtension, bool removeExtension = true) {
      var result = new Dictionary<string, GameObject>();
      AssetsWithGOs(result, go, underAssetFolder, assetFileExtension, removeExtension);
      return result;
    }
    //============================================================

    public static AssetFileProxy ToAssetFileByGuid(this string guid) {
      return new AssetFileProxy(new Guid(guid));
    }
    public static TObj GetAssetObject<TObj>(this string assetPath)
        where TObj : UnityEngine.Object {
      return AssetDatabase.LoadMainAssetAtPath(assetPath) as TObj
        ?? throw new Exception($"the main asset at asset path '{assetPath}' is not of type '{typeof(TObj).Name}'");
    }
    public static UnityEngine.Object GetAsset(this string pathWithoutAssetsRoot) {
      return new AssetFileProxy(new FilePath("Assets/" + pathWithoutAssetsRoot)).MainAssetObject.UnityObject;
    }


    public static AssetFileProxy CreateAsset(this AbsPath path, UnityEngine.Object obj) {
      AssetDatabase.CreateAsset(obj, path.PathStr);
      return new AssetFileProxy(new FilePath(path));
    }
    public static void CreateAsset(this UnityEngine.Object obj, string assetPath) {
      AssetDatabase.CreateAsset(obj, assetPath);
    }

    public static void CreateAssetFolder(this string assetFolderPath) {
      AssetDatabase.CreateFolder(assetFolderPath.GetParentPath(), assetFolderPath.GetFolderName());
    }

    public static AssetFileProxy CreateAssetFolder(this AbsPath path) {
      string dirGuid = AssetDatabase.CreateFolder(path.PathStr.GetParentPath().GetPathUnderProjectFolder(), path.FileName);
      return new AssetFileProxy(new FilePath(path));
    }

    public static void ImportPackage(AbsPath path, bool interactive = true) {
      AssetDatabase.ImportPackage(path.PathStr, interactive);
    }
    public static AssetObjProxy AsAssetObject(this UnityEngine.Object obj) {
      Debug.Assert(AssetDatabase.Contains(obj));
      return new AssetObjProxy(obj);
    }

#if UNITY_EDITOR
    public static string AssetsPath => Application.dataPath;
    public static string ProjectPath => Application.dataPath.GetParentPath();
    public static string AssetPathToFullPath(this string assetPath) {
      Debug.Assert(assetPath.StartsWith("Assets/"));
      return ProjectPath.CombinePath(assetPath);
    }
#else
    public static string AssetsFolderPath => throw new Exception();
      public static string ProjectPath =>throw new Exception();
#endif

    public static string GetPathUnderProjectFolder(this string str) {
      return str.GetRelativePathUnder(ProjectPath);
    }
  }
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  public struct AssetFileProxy {

    public static AssetFileProxy ByAssetPath(string assetPath) {
      string guid = AssetDatabase.AssetPathToGUID(assetPath);
      return new AssetFileProxy(new Guid(guid));
    }
    public static AssetFileProxy ByFullPath(string fullPath) {
      return ByAssetPath(fullPath.GetRelativePathUnder(AssetExtensions.ProjectPath));
    }

    public FilePath File => new FilePath(AssetExtensions.ProjectPath.CombinePath(AssetDatabase.GUIDToAssetPath(FileGuid)));
    public string Path => AssetDatabase.GUIDToAssetPath(FileGuid);

    public AssetFileProxy(string assetPath) : this() {
      FileGuid = AssetDatabase.AssetPathToGUID(assetPath);
    }
    public AssetFileProxy(Guid guid) : this() { /*: base(AssetExtensions.ProjectPath.CombinePath(AssetDatabase.GUIDToAssetPath(guid))) {*/
      // todo: check
      FileGuid = guid.ToString();
    }
    public AssetFileProxy(FilePath path) : this() {
      FileGuid = AssetDatabase.AssetPathToGUID(path.PathStr.GetPathUnderProjectFolder());
    }


    //============================================================

    public string FileGuid { get; private set; }

    private UnityEngine.Object[] loadedObjects;
    public UnityEngine.Object[] LoadedObjects {
      get {
        if (loadedObjects == null) {
          return Reload();
        }
        return loadedObjects;
      }
    }

    public string[] DirectDependencies => AssetDatabase.GetDependencies(Path, false);
    public string[] RecursiveDependencies => AssetDatabase.GetDependencies(Path, true);

    public Type MainAssetType => AssetDatabase.GetMainAssetTypeAtPath(Path);

    //------------------------------------------------------------

    public bool IsFolder => AssetDatabase.IsValidFolder(Path);

    //============================================================
    public AssetFileProxy CopyTo(AbsPath path) {
      AssetDatabase.CopyAsset(Path, path.PathStr.GetPathUnderProjectFolder()).Assert();
      return new AssetFileProxy(new FilePath(path.PathStr));
    }
    public void Delete() {
      AssetDatabase.DeleteAsset(Path).Assert();
    }
    public void MoveToTrash() {
      AssetDatabase.MoveAssetToTrash(Path).Assert();
    }
    public void MoveTo(AbsPath newPath) {
      string msg = AssetDatabase.ValidateMoveAsset(Path, newPath.PathStr.GetPathUnderProjectFolder());
      (msg == "").Assert(msg);
      AssetDatabase.MoveAsset(Path, newPath.PathStr.GetPathUnderProjectFolder());
      (msg == "").Assert(msg);

    }
    // ? is name including extension, a path?
    public void Rename(string newName) {
      string msg = AssetDatabase.RenameAsset(Path, newName);
      (msg == "").Assert(msg);
    }
    //============================================================
    public void Add(UnityEngine.Object obj) {
      //PathUnderProjectFolder.General.Assert();
      AssetDatabase.AddObjectToAsset(obj, Path);
    }
    public void Remove(UnityEngine.Object obj) {
      //PathUnderProjectFolder.General.Assert();
      // ?? why it does not require a asset path parameter?? by inernally record?
      AssetDatabase.RemoveObjectFromAsset(obj);
    }
    //============================================================
    public AssetObjProxy FirstAssetObject => new AssetObjProxy(Load()[0]);
    public AssetObjProxy MainAssetObject => new AssetObjProxy(AssetDatabase.LoadMainAssetAtPath(Path));
    public UnityEngine.Object[] Load() {
      return LoadedObjects;
    }
    public UnityEngine.Object[] Reload() {
      return loadedObjects = AssetDatabase.LoadAllAssetsAtPath(Path);
      ;
    }
    //============================================================

    //============================================================
    public void OpenAllAssetObjectsWithTools() {
      AssetDatabase.OpenAsset(Load());
    }
    //============================================================
    public void ExportAsPackage(string packageStorePath, bool interactive = true,
      ExportPackageOptions options = ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse) {
      AssetDatabase.ExportPackage(Path, packageStorePath, options | (interactive ? ExportPackageOptions.Interactive : 0));
    }
    //============================================================
    public string[] FindAssets(List<string> alternativeNames = null, System.Type baseType = null, List<string> labels = null) {
      IsFolder.Assert();
      var searchStr = new StringBuilder();
      if (alternativeNames != null) {
        foreach (string name in alternativeNames) {
          searchStr.Append($"{alternativeNames} ");
        }
      }
      if (baseType != null) {
        searchStr.Append($"t:byType.Name ");
      }
      if (labels != null) {
        foreach (string label in labels) {
          searchStr.Append($"l:{label} ");
        }
      }
      return AssetDatabase.FindAssets(searchStr.ToString(), new string[] { Path });
    }
  }
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  public struct AssetObjProxy {

    //============================================================
    public int InstanceId { get; }
    public UnityEngine.Object UnityObject { get; }

    public string AssetFileGuid {
      get {
        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(InstanceId, out string guid, out long localId).Assert();
        return guid;
      }
    }
    public long LocalId {
      get {
        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(InstanceId, out string guid, out long localId).Assert();
        return localId;
      }
    }

    public AssetObjProxy(int instanceId) {
      InstanceId = instanceId;
      UnityObject = EditorUtility.InstanceIDToObject(instanceId);
    }
    public AssetObjProxy(UnityEngine.Object obj) {
      UnityObject = obj;
      InstanceId = obj.GetInstanceID();
    }

    //============================================================
    public void OpenWithTool() {
      AssetDatabase.OpenAsset(InstanceId);
    }
    public void OpenWithTool(int lineNumber) {
      AssetDatabase.OpenAsset(InstanceId, lineNumber);
    }
    //============================================================
    public string[] Labels {
      get => AssetDatabase.GetLabels(UnityObject);
      set => AssetDatabase.SetLabels(UnityObject, value);
    }
    public bool HasLabel(string label) {
      return Labels.Has(label);
    }
    public void AddLabel(string label) {
      throw new Exception();
      //Debug.Assert(!HasLabel(label));
      //Labels = Labels.GetAppendedWith(label);
    }
    public void AddLabels(string[] label) {
      throw new Exception();
      //Labels = Labels.GetConcatedWith(label);
    }
    public void ToggleLabel(string label) {
      if (HasLabel(label)) {
        RemoveLabel(label);
      }
      else {
        AddLabel(label);
      }
    }
    public void RemoveLabel(string label) {
      Debug.Assert(Labels.Has(label));
      string[] newLabels = new string[Labels.Count() - 1];
      int i = 0;
      foreach (string l in Labels) {
        if (l != label) {
          newLabels[i++] = l;
        }
      }
      Labels = newLabels;
    }
    public void ClearLabels() {
      AssetDatabase.ClearLabels(UnityObject);
    }
    //============================================================
    // correct?
    public AssetFileProxy AssetFile => AssetFileProxy.ByAssetPath(AssetDatabase.GetAssetOrScenePath(UnityObject));
    //============================================================
    // unity inernal used type, e.g., .mat
    public bool IsNative => AssetDatabase.IsNativeAsset(InstanceId);
    // 3rd external asset type, e.g., .png
    public bool IsForeign => AssetDatabase.IsForeignAsset(InstanceId);
    public bool IsSubAsset => AssetDatabase.IsSubAsset(InstanceId);
    //============================================================
  }
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  public static class MeshAssetExtensions {
    public static bool ContainsMeshWithBounds(this GameObject obj) {
      MeshRenderer m = obj.FindFirstComponentInDescendant<MeshRenderer>();
      if (m) {
        Collider c = m.gameObject.GetComponent<Collider>();
        if (c) {
          return true;
        }
      }
      return false;
    }
  }
}
