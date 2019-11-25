using System;
using System.Collections.Generic;
using System.Text;
using AirKuma.FileSys;
using UnityEngine;

namespace AirKuma {


  public enum AssetType {
    UnknownAssetType = 0,
    Texture,
    Material,
    Script,
    Shader,
    Terrain,
    TerrainLayer,
    Scene,
    Model,
    Animation,
    Audio,
    Video,
  }
  public static class TypeToAssetType {
    public static Dictionary<Type, AssetType> catalog = new Dictionary<Type, AssetType>();
    static TypeToAssetType() {
      catalog.Add(typeof(Texture2D), AssetType.Texture);
      catalog.Add(typeof(Material), AssetType.Material);
      catalog.Add(typeof(Terrain), AssetType.Terrain);
      catalog.Add(typeof(TerrainLayer), AssetType.TerrainLayer);
    }
  }
  public static class FileExtensionToAssetType {

    public static Dictionary<string, AssetType> catalog = new Dictionary<string, AssetType>();
    static FileExtensionToAssetType() {

      catalog.Add(".png", AssetType.Texture);
      catalog.Add(".jpg", AssetType.Texture);
      catalog.Add(".tag", AssetType.Texture);

      catalog.Add(".mat", AssetType.Material);

      catalog.Add(".cs", AssetType.Script);

      catalog.Add(".terrain", AssetType.Terrain);
      catalog.Add(".terrainlayer", AssetType.TerrainLayer);

      catalog.Add(".unity", AssetType.Scene);

      catalog.Add(".mp3", AssetType.Audio);
      catalog.Add(".ogg", AssetType.Audio);
      catalog.Add(".wav", AssetType.Audio);

      catalog.Add(".mp4", AssetType.Video);

      catalog.Add(".obj", AssetType.Model);
      catalog.Add(".fbx", AssetType.Model);
    }
  }
  public static class AssetMatch {
    public static bool IsMatchedByFileExtension(string fileExtension, Type type) {
      return FileExtensionToAssetType.catalog.ContainsKey(fileExtension)
        && TypeToAssetType.catalog.ContainsKey(type)
        && FileExtensionToAssetType.catalog[fileExtension] == TypeToAssetType.catalog[type];
    }
    public static bool IsMatchedByActualType(UnityEngine.Object inputObj, Type requiredAssetObjectType) {
      if (TypeToAssetType.catalog.TryGetValue(inputObj.GetType(), out AssetType outAssetType)) {
        return outAssetType == TypeToAssetType.catalog[requiredAssetObjectType];
      }
      return false;
    }
  }

  public struct GoProxy {

  }

  public struct GoPathProxy {

    private static readonly Dictionary<string, GameObject> accessCache;
    static GoPathProxy() {
      accessCache = new Dictionary<string, GameObject>();
    }

    public readonly string goPath;

    public GameObject Find() {
      return GameObject.Find(goPath);
    }

    public GoPathProxy(string goPath) {
      if (!goPath.StartsWith("/"))
        goPath = "/" + goPath;
      this.goPath = goPath ?? throw new ArgumentNullException(nameof(goPath));
    }

    public GameObject GetOrCreate() {
      if (accessCache.TryGetValue(goPath, out GameObject go))
        return go;
      GameObject o = FindOrCreate();
      accessCache.Add(goPath, o);
      return o;
    }

    public GameObject FindOrCreate() {
      GameObject o = null;
      foreach (string name in goPath.Split('/')) {
        if (name.Length != 0) {
          if (o is null) {
            o = name.GetTopLevelGO() ?? new GameObject(name);
          }
          else {
            o = o.GetOrAddChild(name);
          }
        }
      }
      return o ?? throw new InvalidOperationException();
    }
  }


  public static class GameObjectEx {

    public static void SetBounds(this BoxCollider collider, Bounds bounds) {
      collider.center = bounds.center;
      collider.size = bounds.size;
    }

    public static GameObject AttachToNewEmptyParent(this GameObject self) {
      var par = new GameObject("");
      int i = self.GetSiblingIndex();
      par.transform.parent = self.transform.parent;
      self.transform.parent = par.transform;
      par.SetSiblingIndex(i);
      return par;
    }

    public static int GetSiblingIndex(this GameObject self) {
      return self.transform.GetSiblingIndex();
    }
    public static void SetSiblingIndex(this GameObject self, int index) {
      self.transform.SetSiblingIndex(index);
    }

    public static void SetDescendantActivities(this GameObject self, bool activity) {
      self.SetActive(activity);
      foreach (GameObject child in self.GetChildren()) {
        child.SetDescendantActivities(activity);
      }
    }

    public static GameObject GetTopLevel(string name) {
      Debug.Assert(!name.StartsWith("/"));
      return GameObject.Find("/" + name);
    }
    public static GameObject[] TopLevelObjects => UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

    //============================================================

    public static bool IsActive(this Behaviour behaviour) {
      return behaviour.enabled && behaviour.gameObject.activeSelf;
    }

    //============================================================

    public static GameObject GetTopLevelGO(this string name) {
      // todo: ambiguity?
      return GameObject.Find(name);
    }

    public static GameObject GetOrCreateGo(this GameObject go, string relGOPath) {
      GameObject o = go;
      foreach (string name in relGOPath.Split('/')) {
        if (name.Length != 0) {
          o = o.GetOrAddChild(name);
        }
      }
      if (o == go) {
        throw new Exception();
      }
      return o;
    }
    //------------------------------------------------------------
    private static void FindingPath(StringBuilder s, Transform trf) {
      if (trf.parent != null) {
        FindingPath(s, trf.parent);
      }
      s.Append("/");
      s.Append(trf.name);
    }
    public static string GetFindingPath(this GameObject obj) {
      var s = new StringBuilder();
      FindingPath(s, obj.transform);
      return s.ToString();
    }
    //=========================================================== =
    public static TComponent FindFirstComponentInChildren<TComponent>(this GameObject obj)
      where TComponent : Component {
      foreach (Transform childTrf in obj.transform) {
        TComponent childCompo = childTrf.GetComponent<TComponent>();
        if (childCompo != null) {
          return childCompo;
        }
      }
      return null;
    }
    public static TComponent FindFirstComponentInDescendant<TComponent>(this GameObject self)
        where TComponent : Component {
      TComponent found = self.GetComponent<TComponent>();
      if (found) {
        return found;
      }
      foreach (GameObject child in self.GetChildren()) {
        TComponent childFound = child.FindFirstComponentInDescendant<TComponent>();
        if (childFound) {
          return childFound;
        }
      }
      return null;
    }
    //============================================================
    public static GameObject FindFirstDescendant(this GameObject self, string name) {
      if (self.name == name) {
        return self;
      }
      foreach (Transform childTrf in self.transform) {
        GameObject found = childTrf.gameObject.FindFirstDescendant(name);
        if (found != null) {
          return found;
        }
      }
      return null;
    }
    public static GameObject FindFirstChildWithComponent<TComponent>(this GameObject obj)
        where TComponent : Component {
      foreach (Transform trf in obj.transform) {
        if (trf.gameObject.GetComponent<TComponent>() != null) {
          return trf.gameObject;
        }
      }
      return null;
    }
    public static GameObject FindFirstDescendantWithComponent<TComponent>(this GameObject obj)
        where TComponent : Component {
      TComponent found = obj.GetComponent<TComponent>();
      if (found != null) {
        return obj;
      }
      foreach (Transform childTrf in obj.transform) {
        GameObject childFound = childTrf.gameObject.FindFirstDescendantWithComponent<TComponent>();
        if (childFound != null) {
          return childFound;
        }
      }
      return null;
    }

    //============================================================
    // todo: improve implementation
    private static readonly List<Component> ListForGettingComponents = new List<Component>();
    private static void SetupListForGettingComponents<TComponent>(this GameObject obj)
        where TComponent : Component {
      obj.GetComponents(typeof(TComponent), ListForGettingComponents);
    }
    public static int GetSiblingIndex<TComponent>(this Component compo)
        where TComponent : Component {
      compo.gameObject.SetupListForGettingComponents<TComponent>();
      return ListForGettingComponents.IndexOf(compo);
    }
    public static TComponent NextSibling<TComponent>(this Component compo)
        where TComponent : Component {
      int index = compo.GetSiblingIndex<TComponent>();
      if (ListForGettingComponents.Count() - 1 > index) {
        return ListForGettingComponents[index + 1] as TComponent;
      }
      return null;
    }
    //------------------------------------------------------------
    public static TComponent GetFirstComponent<TComponent>(this GameObject obj)
        where TComponent : Component {
      obj.SetupListForGettingComponents<TComponent>();
      if (ListForGettingComponents.Count == 0) {
        return null;
      }
      return ListForGettingComponents[0] as TComponent;
    }
    public static TComponent GetLastComponent<TComponent>(this GameObject obj)
        where TComponent : Component {
      obj.SetupListForGettingComponents<TComponent>();
      if (ListForGettingComponents.Count == 0) {
        return null;
      }
      return ListForGettingComponents[ListForGettingComponents.Count - 1] as TComponent;
    }
    //============================================================
    public static GameObject GetChildNamed(this GameObject obj, string childName) {
      foreach (Transform childTrf in obj.transform) {
        if (childTrf.gameObject.name == childName) {
          return childTrf.gameObject;
        }
      }
      return null;
    }

    public static GameObject GetOrAddChild(this GameObject obj, string childName) {
      return obj.GetChildNamed(childName) ?? obj.AddChild(childName);
    }



    public static bool HasChild(this GameObject obj, string childName) {
      return obj.GetChildNamed(childName) != null;
    }
    public static GameObject AddChild(this GameObject obj, GameObject child) {
      child.transform.parent = obj.transform;
      return child;
    }
    public static GameObject AddChild(this GameObject obj, string childName) {
      var childObj = new GameObject(childName);
      return obj.AddChild(childObj);
    }
    public static GameObject Gethild(this GameObject obj, string childName) {
      foreach (Transform childTrf in obj.transform) {
        if (childTrf.gameObject.name == childName) {
          return childTrf.gameObject;
        }
      }
      return null;
    }
    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component {
      return obj.GetComponent<T>() ?? obj.AddComponent<T>();
    }
    //------------------------------------------------------------
    public static void Unparent(this GameObject child) {
      child.transform.parent = null;
    }
    public static void RemoveChildren(this GameObject obj) {
      var children = obj.GetChildren().ToList();
      foreach (GameObject child in children) {
#if UNITY_EDITOR
        GameObject.DestroyImmediate(child);
#else
        GameObject.DestroyImmediate(child);
#endif
      }
    }
    public static GameObject Clear(this GameObject obj) {
      obj.RemoveChildren();
      return obj;
    }


    public static bool HasComponent<T>(this GameObject obj) where T : Component {
      return obj.GetComponent<T>() != null;
    }
    public static void RemoveComponent<T>(this GameObject obj) where T : Component {
      GameObject.Destroy(obj.GetComponent<T>());
    }
    public static void RemoveComponents(this GameObject obj) {
      Component[] comps = obj.GetComponents<Component>();
      foreach (Component compo in comps) {
        if (compo.GetType() != typeof(Transform)) {
#if UNITY_EDITOR
          GameObject.DestroyImmediate(compo);
#else
        GameObject.Destroy(compo);
#endif
        }
      }
    }
    //============================================================
    public static void RenameAsType<TBaseComponent>(this GameObject obj) where TBaseComponent : Component {
      obj.name = obj.GetComponent<TBaseComponent>().GetType().Name;
    }
    public static void RenameAsTypeRecursively<TBaseComponentToRenameAs>(this GameObject obj) where TBaseComponentToRenameAs : Component {
      if (obj.GetComponent<TBaseComponentToRenameAs>() != null) {
        obj.RenameAsType<TBaseComponentToRenameAs>();
      }
      foreach (Transform childTrf in obj.transform) {
        GameObject childObj = childTrf.gameObject;
        childObj.RenameAsTypeRecursively<TBaseComponentToRenameAs>();
      }
    }
    //============================================================
    public static IEnumerable<GameObject> GetSiblings(this GameObject obj) {
      GameObject par = obj.GetParent();
      if (par != null) {
        foreach (Transform siblingOrSelfTrf in par.transform) {
          if (siblingOrSelfTrf.gameObject != obj) {
            yield return siblingOrSelfTrf.gameObject;
          }
        }
      }
    }
    //============================================================
    public static Transform GetParent(this Transform trf) {
      return trf.parent;
    }
    public static GameObject GetParent(this GameObject obj) {
      return obj.transform.parent.gameObject;
    }
    public static TBehaviour GetActiveParent<TBehaviour>(this Behaviour behaviour)
      where TBehaviour : Behaviour {
      Transform parTrf = behaviour.gameObject.transform.parent;
      if (parTrf != null) {
        TBehaviour compo = parTrf.gameObject.GetComponent<TBehaviour>();
        if (compo?.IsActive() == true) {
          return compo;
        }
      }
      return null;
    }
    public static void SetParent(this GameObject obj, GameObject par) {
      obj.transform.parent = par.transform;
    }
    //============================================================
    public static Transform GetRoot(this Transform trf) {
      if (trf.parent != null) {
        return trf.parent.GetRoot();
      }
      return trf;
    }
    public static GameObject GetRoot(this GameObject obj) {
      return obj.transform.GetRoot().gameObject;
    }
    //------------------------------------------------------------
    public static bool IsRoot(this Transform trf) {
      return trf.parent == null;
    }
    public static bool IsRoot(this GameObject obj) {
      return obj.transform.IsRoot();
    }
    public static bool IsRoot(this Component compo) {
      return compo.gameObject.transform.IsRoot();
    }
    //============================================================
    // get continuous ancestor with component of type TMonobeahviour which is active
    private static TMonobeahviour GetAncestor<TMonobeahviour>(this Component compo, Predicate<TMonobeahviour> cond)
        where TMonobeahviour : MonoBehaviour {
      Transform parTrf = compo.gameObject.transform.parent;
      if (parTrf != null) {
        GameObject parObj = parTrf.gameObject;
        TMonobeahviour parCompo = parObj.GetComponent<TMonobeahviour>();
        if (parCompo != null && cond(parCompo)) {
          return parCompo.GetAncestor<TMonobeahviour>(cond);
        }
      }
      return compo as TMonobeahviour;
    }
    public static TMonobeahviour GetAncestor<TMonobeahviour>(this Component compo)
        where TMonobeahviour : MonoBehaviour {
      return compo.GetAncestor((TMonobeahviour _) => true);
    }
    public static TMonobeahviour GetActiveAncestor<TMonobeahviour>(this Component compo)
        where TMonobeahviour : MonoBehaviour {
      return compo.GetAncestor((TMonobeahviour behaviour) => behaviour.IsActive());
    }
    //============================================================
    public static IEnumerable<Transform> GetChildren(this Transform trf) {
      foreach (Transform subTrf in trf) {
        yield return subTrf;
      }
    }
    public static IEnumerable<GameObject> GetChildren(this GameObject obj) {
      foreach (Transform subTrf in obj.transform) {
        yield return subTrf.gameObject;
      }
    }
    public static IEnumerable<TMonobeahviour> GetActiveChildren<TMonobeahviour>
        (this Component compo)
        where TMonobeahviour : MonoBehaviour {
      foreach (Transform subTrf in compo.transform) {
        foreach (TMonobeahviour subCompo in subTrf.GetComponents<TMonobeahviour>()) {
          if (subCompo.IsActive()) {
            yield return subCompo;
          }
        }
      }
    }
    //============================================================
    public static void RemoveThisGameObject(this UnityEngine.Object obj) {
#if UNITY_EDITOR
      UnityEngine.Object.DestroyImmediate(obj);
#else
      UnityEngine.Object.Destroy(obj);
#endif
    }

    // behaviour could be "this"
    public static void RemoveMonoBehaviour(this MonoBehaviour behaviour) {
#if UNITY_EDITOR
      UnityEngine.Object.DestroyImmediate(behaviour);
#else
      UnityEngine.Object.Destroy(behaviour);
#endif
    }
    //============================================================
  }

}
