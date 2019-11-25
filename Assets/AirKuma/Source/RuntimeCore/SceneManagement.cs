using System;
using System.Collections;
using System.Collections.Generic;
using AirKuma.FileSys;
using AirKuma.Geom;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AirKuma {

  [Serializable]
  public class AirScene {

    public static bool IsSceneAsset(string assetPath) {
      return System.IO.File.Exists(assetPath) && System.IO.Path.GetExtension(assetPath) is ".unity";
    }

    public Scene unityScene;

    public AirScene(Scene unityScene) {
      this.unityScene = unityScene;
    }
    public AirScene(int buildIndex) : this(SceneManager.GetSceneByBuildIndex(buildIndex)) { }
#if UNITY_EDITOR
    public AirScene(string scenePath) : this(SceneManager.GetSceneByPath
      (scenePath)) { }
#endif
    //============================================================

    public int BuildIndex => unityScene.buildIndex;

    public string Path => unityScene.path;

    public static int CountOfSceneToBuild => UnityEngine.SceneManagement.SceneManager.sceneCount;

    public static AirScene Active => new AirScene(SceneManager.GetActiveScene().buildIndex);

    public string PathInBuildList => Path.GetRelativePathUnder("Assets");

    public string Name => unityScene.name;
    public int TopLevelGOCount => unityScene.rootCount;

    public bool Loaded => unityScene.isLoaded;

    public GameObject[] TopLevelGOs => unityScene.GetRootGameObjects();

    //============================================================
    public void Activate() {
      SceneManager.SetActiveScene(unityScene);
    }

    //============================================================
    public IEnumerable<TComponent> AllComponentsByTypes<TComponent>() {
      foreach (GameObject go in TopLevelGOs) {
        foreach (TComponent compo in go.GetComponentsInChildren<TComponent>()) {
          yield return compo;
        }
      }
    }
    //============================================================
    public void TranslateBy(Vector3 offset) {
      foreach (GameObject go in TopLevelGOs) {
        go.transform.localPosition += offset;
      }
    }
    public void RotateBy(Quaternion angleDelta) {
      foreach (GameObject go in TopLevelGOs) {
        go.transform.localRotation = go.transform.localRotation * angleDelta;
      }
    }
    //============================================================
    public Bounds GetOverallBounds() {
      var b = new Bounds(Vector3.zero, Vector3.zero);
      foreach (Collider r in AllComponentsByTypes<Collider>()) {
        if (b.IsZero()) {
          b = r.bounds;
        }
        else {
          b.Encapsulate(r.bounds);
        }
      }
      return b;
    }
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //============================================================
    #region loading

    //------------------------------------------------------------

    public void LoadImmediately() {
      Application.isPlaying.Assert();
      Debug.Log($"immediately load the scene '{Path}'");
      SceneManager.LoadScene(Path, LoadSceneMode.Additive);
    }
    //------------------------------------------------------------

    static HashSet<AirScene> loadingScenes = new HashSet<AirScene>();
    public bool Loading => loadingScenes.Contains(this);

    public void Load(Action<AirScene> afterLoading = null) {
      Application.isPlaying.Assert();
      Debug.Log($"load the scene '{Path}'");
      Debug.Assert(!loadingScenes.Contains(this));
      loadingScenes.Add(this);
      AirSystem.Service.ExecCoroutine(LoadAsyncCoroutine(afterLoading));

    }
    IEnumerator LoadAsyncCoroutine(Action<AirScene> afterLoading) {
      AsyncOperation op = SceneManager.LoadSceneAsync(Path, LoadSceneMode.Additive);
      while (!op.isDone) {
        yield return null;
      }
      loadingScenes.Remove(this);
      afterLoading?.Invoke(this);
    }

    #endregion
    //------------------------------------------------------------
    //============================================================
    #region unloading

    public void Unload(Action<AirScene> afterUnloading = null) {
      Application.isPlaying.Assert();
      Debug.Log($"unload the scene '{Path}'");
      Debug.Assert(!unloadingScenes.Contains(this));
      unloadingScenes.Add(this);
      AirSystem.Service.ExecCoroutine(UnloadAsyncCoroutine(afterUnloading));
    }

    static HashSet<AirScene> unloadingScenes = new HashSet<AirScene>();
    public bool Unloading => unloadingScenes.Contains(this);

    IEnumerator UnloadAsyncCoroutine(Action<AirScene> afterUnloading) {
      AsyncOperation op = SceneManager.UnloadSceneAsync(Path);
      while (!op.isDone) {
        yield return null;
      }
      unloadingScenes.Remove(this);
      afterUnloading?.Invoke(this);
    }

    #endregion
    //============================================================
    public override bool Equals(object obj) {
      return obj is AirScene s && this.unityScene == s.unityScene;
    }

    public override int GetHashCode() {
      return unityScene.GetHashCode();
    }

    public override string ToString() {
      return Path;
    }
    //============================================================
  }
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  public class AirSceneField : PropertyAttribute { }
}