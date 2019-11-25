//#define LOG_RELOAD_TIME
//#define AIRSYSTEM_INIT
#define ShowAirSystemInHierarchy
using System;
using AirKuma.Geom;
using AirKuma.UnityCore;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine.SceneManagement;
using UnityEngine;

namespace AirKuma {


  /// <summary>
  /// provide all the listenable events for a monobehaviour in a scene
  /// a gameobject named AirSystem with <cref>AirSystem</cref> monobahaviour will always present in current active scene
  /// </summary>
#if UNITY_EDITOR && AIRSYSTEM_INIT
  [InitializeOnLoad]
#endif
  [ExecuteAlways]
  public class AirSystem : MonoBehaviour {

    //============================================================

    private const string RootName = "AirSystem";
    private const string RootPath = "/" + RootName;

    private const float LowFps = 20.0f;
    private const int LowFpsCountLimit = 5;

    //============================================================

    public static event Action OnEnabled;
    public static event Action OnDisabled;

    public static event Action OnAwake;
    public static event Action OnStart;

    public static event Action OnUpdate;

    public static event Action OnGui;
    public static event Action OnGameGui;
#if UNITY_EDITOR
    public static event Action OnSceneGui;
#endif

    public static event Action<Vector2> OnResolutionChange;

    public static event Action OnLowInMemory; 
    public static event Action OnFpspTooLow; // runtime or play mode only

    public static event Action OnQuit;

    static AirSystem() {
#if UNITY_EDITOR
      EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChange;
      EditorSceneManager.sceneOpening += BeforeOpeningScene;
      EditorSceneManager.sceneOpened += AfterOpeningScene;
      EditorSceneManager.sceneSaving += BeforeSavingScene;
      EditorSceneManager.activeSceneChangedInEditMode += OnActivatingScene;
      EditorSceneManager.sceneClosing += OnSceneClosing;
#endif
      EditorSceneManager.activeSceneChanged += OnActivatingScene;
    }

    //============================================================
#if UNITY_EDITOR
    private static void OnSceneClosing(Scene scene, bool removingScene) {
      //Debug.Log($"closing the scene '{scene.path}'");
    }
    private static void OnActiveSceneChange(Scene oldScene, Scene newScene) {
      //Debug.Log($"activate the scene '{newScene.path}'");
    }
    private static void AfterOpeningScene(Scene scene, OpenSceneMode mode) {
      //Debug.Log($"after opening the scene '{scene.path}'");
    }
    private static void BeforeOpeningScene(string path, OpenSceneMode mode) {
      //Debug.Log($"before opening the scene '{path}'");
    }
    private static void BeforeSavingScene(Scene scene, string path) {
      //Debug.Log($"before saving the scene '{scene.path}'");
    }
#endif 
    private static void OnActivatingScene(Scene current, Scene next) {
      Debug.Log($"on active scene changed '{current.path}' => '{next.path}'");

      var o = GameObject.Find(RootPath);
      if (o != null) {
        SceneManager.MoveGameObjectToScene(o, next);
      }
    }

    //============================================================
    private static GameObject rootGO = null;
    private static AirSystem rootCompo ;

    private static void EnsureRootPresent() {
      rootGO = GameObject.Find(RootPath);
      if (rootGO is null) {
        rootGO = new GameObject(RootName);
        rootCompo = rootGO.AddComponent<AirSystem>();
      } 
    }
    private static GameObject RootGO {
      get {
        if (rootGO is null) 
          EnsureRootPresent();
        return rootGO; 
      }
    }
    public static AirSystem Service {
      get {
        if (rootGO is null)
          EnsureRootPresent();
        return rootCompo;
      }
    }

    public static GameObject ObjService(string goName) {
      return RootGO.GetOrAddChild(goName);
    }

    public static TComponent CompoService<TComponent>() where TComponent : Component {
      return RootGO.GetOrAddComponent<TComponent>();
    }

    public static void SyncCompoService<TComponent>(bool active) where TComponent : Component {
      if (active && !RootGO.HasComponent<TComponent>())
        RootGO.AddComponent<TComponent>();
      else if (RootGO.HasComponent<TComponent>())
        RootGO.RemoveComponent<TComponent>();
    }

    //============================================================

    public float Fps { get; private set; }

    [NonSerialized]
    private int countOnFpsTooLow;

    [NonSerialized]
    private Vector2 previousGameResolution;
    [NonSerialized]
    private Vector2 previousSceneResolution;

    // should be set via editor
    // future maybe label.richText = true;
    [SerializeField]
    public GUISkin defaultSkin;

#if UNITY_EDITOR && LOG_RELOAD_TIME
    [SerializeField]
    public double startReloadTime;
#endif
     
    //============================================================

    private void Awake() {
      OnAwake?.Invoke();
    }

    private void Start() {
      OnStart?.Invoke();
    }

    private void InitStates() {
      gameObject.hideFlags |= HideFlags.DontSave;
#if !ShowAirSystemInHierarchy
        gameObject.hideFlags |= HideFlags.HideInHierarchy;
#endif

      if (!transform.IsLocalIdentity()) {
        transform.BeIdentity();
      }
    }
     
    private void OnEnable() {
#if UNITY_EDITOR && LOG_RELOAD_TIME
      Debug.Log($"*** reloaded in {EditorApplication.timeSinceStartup - startReloadTime}s ***");
#endif
      InitStates();

      Application.wantsToQuit += WantsToQuit;

      // todo: callback signature same for each platform?
      Application.lowMemory += () => { OnLowInMemory?.Invoke(); };

#if UNITY_EDITOR
      EditorApplication.update += EditorAppUpdateCallback;
      SceneView.onSceneGUIDelegate += HandleOnSceneGUI;
#endif

      OnEnabled?.Invoke();
    }

    private void OnDisable() {  

#if UNITY_EDITOR
      EditorApplication.update -= EditorAppUpdateCallback;
      SceneView.onSceneGUIDelegate -= HandleOnSceneGUI;
#endif 
      OnDisabled?.Invoke();
#if UNITY_EDITOR && LOG_RELOAD_TIME
      Debug.Log("disabled" + EditorApplication.timeSinceStartup.ToString());
      startReloadTime = EditorApplication.timeSinceStartup;
#endif
    }

    private bool WantsToQuit() {
      OnQuit?.Invoke(); // todo: fired for Windows Store app?
      return true;
    }


    //============================================================
    private void Update() {
      Fps = 1.0f / Time.deltaTime;
      if (Fps < LowFps) {
        ++countOnFpsTooLow;
        if (countOnFpsTooLow > LowFpsCountLimit) {
          OnFpspTooLow?.Invoke();
        }
      }
      else {
        countOnFpsTooLow = 0;
      }
      OnUpdate?.Invoke();
    }
#if UNITY_EDITOR
    private void EditorAppUpdateCallback() {
      if (!Application.isPlaying)
        OnUpdate?.Invoke();
    }
#endif

    //public void DelayedCall(float secsToDelay,  Action action) {
    //  this.Invoke()
    //}
    //============================================================
    private void OnGUI() {
      var curResolution = new Vector2(Screen.width, Screen.height);
      if (curResolution != previousGameResolution) {
        OnResolutionChange?.Invoke(curResolution);
        previousGameResolution = curResolution;
      }
      OnGameGui?.Invoke();
      OnGui?.Invoke();
    }
#if UNITY_EDITOR
    // todo: multiple SceneView
    private void HandleOnSceneGUI(SceneView view) {
      var curResolution = new Vector2(Screen.width, Screen.height);
      if (curResolution != previousSceneResolution) {
        OnResolutionChange?.Invoke(curResolution);
        previousSceneResolution = curResolution;
      }
      OnSceneGui?.Invoke();
      OnGui?.Invoke();
    }
#endif
    //============================================================

    public void ExecCoroutine(System.Collections.IEnumerator coroutine) {
      StartCoroutine(coroutine);
    }

    //============================================================
  }

}

//************************************************************
//[AttributeUsage(AttributeTargets.Class)]
//public class InitMeAttribute : System.Attribute {
//}
//System.Reflection.Assembly asm = typeof(AirSystem).Assembly;
//foreach (Type type in asm.GetTypes()) {
//  bool implementsIService = type.GetInterface("IService") != null;
//  if (implementsIService) {

//    bool HasMyAttribute(object[] a) {
//      foreach (object attr in a) {
//        if (attr.GetType() == typeof(InitMeAttribute)) {
//          return true;
//        }
//      }
//      return false;
//    }

//    object[] attrs = type.GetCustomAttributes(true);
//    if (HasMyAttribute(attrs)) {
//      object obj = System.Activator.CreateInstance(type);
//      //Type t = type.BaseType;
//      //while (true) {

//      //  System.Reflection.MethodInfo method = t.GetMethod("Init");
//      //  if (method is null) {
//      //    if ((t = t.BaseType) == null) {
//      //      throw new Exception("no static void Init() defined"); 
//      //    }
//      //  }  
//      //  else {
//      //    if (method.IsStatic) {
//      //      method.Invoke(null, new object[0]);
//      //      var obj = System.Activator.CreateInstance(type);
//      //      break;
//      //    }
//      //  }
//    }
//  }
//}
//************************************************************
