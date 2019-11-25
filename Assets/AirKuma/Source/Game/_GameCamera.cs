//using System;
//using System.Collections.Generic;
//using Kuma.Geom;
//using Kuma.UnityCore;
//using UnityEngine;

//namespace Kuma.Game {


//  [ExecuteAlways, RequireComponent(typeof(Camera), typeof(BoxCollider))]
//  public class GameCamera : MonoBehaviour {

//    //============================================================

//    public const float ApproachRate = 0.03f;
//    public const float HeightOffset = 10;
//    [NonSerialized]
//    public readonly Vector3 OffsetFromPlayer = new Vector3(-HeightOffset, HeightOffset, -HeightOffset);

//    //------------------------------------------------------------

//    [SerializeField]
//    public GameObject thePlayer;

//    //------------------------------------------------------------

//    [NonSerialized]
//    private Camera theCamera;
//    [NonSerialized]
//    private BoxCollider theBoxCollider;

//    [NonSerialized]
//    private Vector2 prevScreenSize;
//    [NonSerialized]
//    private Vector3 interpolatingCameraPosition;

//    [NonSerialized]
//    private bool isTracking;

//    //============================================================

//    public Bounds VisibleBounds => theBoxCollider.bounds;
//    public Bounds LoadingBounds => VisibleBounds.ScaleExtent(1.5f);
//    public Bounds UnloadingBounds => VisibleBounds.ScaleExtent(2.0f);

//    //todo: Does identical bounds always hit-test with same result without precision problem?
//    private void ValidateBounds() {
//      UnloadingBounds.Includes(UnloadingBounds).Assert();
//      LoadingBounds.Includes(VisibleBounds).Assert();
//    }

//    private static int fixedUpdateCount = 0;
//    private void FixedUpdate() {
//      if (fixedUpdateCount % 100 == 0) {
//        TryToLoadVisibleScenes();
//      }
//      if (fixedUpdateCount % 100 == 33) {
//        TryToLoadNearScenes();
//      }
//      if (fixedUpdateCount % 100 == 66) {
//        TryToUnloadFarScenes();
//      }
//      ++fixedUpdateCount;
//    }

//    //============================================================
//    #region probing methods

//    private IEnumerable<AirScene> HitMaps(Bounds bounds) {
//      int count = bounds.GlobalHitTest(HitTestBuffer.buffer, new HitScope(mask: new LayerIndexProxy("MapTrigger")));
//      for (int i = 0; i != count; ++i) {
//        var collider = (BoxCollider)HitTestBuffer.buffer[i];
//        MapTrigger mt = collider.GetComponent<MapTrigger>();
//        yield return mt.Scene;
//      }
//    }
//    //------------------------------------------------------------
//    private void TryToLoadVisibleScenes() {
//      foreach (AirScene scene in HitMaps(VisibleBounds)) {
//        // todo: immediately load it while loading (loading async)
//        if (!scene.Loaded && !scene.Loading && !scene.Unloading) {
//          scene.Load();
//        }
//      }
//    }
//    //------------------------------------------------------------
//    private void TryToLoadNearScenes() {
//      foreach (AirScene scene in HitMaps(LoadingBounds)) {
//        if (!scene.Loaded && !scene.Loading && !scene.Unloading) {
//          scene.Load();

//        }
//      }
//    }

//    //------------------------------------------------------------
//    private HashSet<AirScene> prevUnloadingScenes = new HashSet<AirScene>();
//    private void TryToUnloadFarScenes() {
//      IEnumerable<AirScene> maps = HitMaps(UnloadingBounds);
//      var curUnloadingScenes = new HashSet<AirScene>(maps);
//      foreach (AirScene scene in prevUnloadingScenes) {
//        if (curUnloadingScenes.Contains(scene).Not()) {
//          if (scene.Loaded && !scene.Loading && !scene.Unloading) {
//            scene.Unload();
//          }
//        }
//      }
//      prevUnloadingScenes = curUnloadingScenes;
//    }
//    //------------------------------------------------------------

//    #endregion
//    //============================================================

//    private void SetupForIsometrics() {
//      theCamera.orthographic = true;
//      theCamera.backgroundColor = Color.black;
//      theCamera.clearFlags = CameraClearFlags.SolidColor;
//      theCamera.transform.rotation = CameraEx.IsometricViewAngle;
//      theCamera.nearClipPlane = 0;
//      theCamera.farClipPlane = 100;
//      theCamera.orthographicSize = 5;

//      UpdateToFitRegion();

//      theBoxCollider.SetBounds(theCamera.IsometricClipBounds());
//      theBoxCollider.isTrigger = true;

//      prevScreenSize = new Vector2(Screen.width, Screen.height);
//    }

//    private void OnEnable() {
//      theCamera = GetComponent<Camera>();
//      theBoxCollider = GetComponent<BoxCollider>();
//      isTracking = false;
//      ValidateBounds();
//      SetupForIsometrics();
//    }

//    private void UpdateToFitRegion() {
//      theCamera.FitCamera(new Rect(0, 0, Screen.width, Screen.height), CameraEx.VerticalLengthFactor);
//    }

//    // Update is called once per frame
//    private void Update() {
//      if (prevScreenSize != new Vector2(Screen.width, Screen.height)) {
//        OnViewportRegionSizeChanged();
//      }

//      InterpolationUpdate();
//    }

//    private void OnViewportRegionSizeChanged() {
//      UpdateToFitRegion();
//    }

//    //private void OnGUI() {
//    //  Event e = Event.current;
//    //  if (e.type == EventType.KeyDown) {
//    //    //if (e.keyCode == KeyCode.A) {
//    //    //  Air.FitCamera(theCamera, theCamera.pixelRect, Sqrt3, FitMode.Shrink);
//    //    //}
//    //    //if (e.keyCode == KeyCode.B) {
//    //    //  Air.FitCamera(theCamera, theCamera.pixelRect, Sqrt3, FitMode.Extented);
//    //    //}
//    //    //if (e.keyCode == KeyCode.Q) {
//    //    //  Debug.Log("enter FullScreen mode");
//    //    //  Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
//    //    //}
//    //    //if (e.keyCode == KeyCode.W) {
//    //    //  Debug.Log("enter FullScreen mode");
//    //    //  Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
//    //    //}
//    //    //if (e.keyCode == KeyCode.E) {
//    //    //  Debug.Log("enter FullScreen mode");
//    //    //  Screen.fullScreenMode = FullScreenMode.Windowed;
//    //    //}
//    //    //if (e.keyCode == KeyCode.R) {
//    //    //  Debug.Log("enter FullScreen mode");
//    //    //  Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
//    //    //}

//    //  }
//    //}

//    public bool IsTracking {
//      get => isTracking;
//      set {
//        if (isTracking) {
//          interpolatingCameraPosition = thePlayer.transform.position;
//        }
//        isTracking = value;
//      }
//    }

//    private void InterpolationUpdate() {
//      if (thePlayer is null) {
//        return;
//      }

//      if (IsTracking) {
//        interpolatingCameraPosition += (thePlayer.transform.position - interpolatingCameraPosition) * ApproachRate;
//        theCamera.transform.position = interpolatingCameraPosition + OffsetFromPlayer;
//      } else {
//        theCamera.transform.position = thePlayer.transform.position + OffsetFromPlayer;
//      }
//    }
//  }


//  //public partial class Utils {

//  //  //static FitMode screenFitMode = FitMode.Extented;
//  //  //public static FitMode ScreenFitMode {
//  //  //  get => screenFitMode;
//  //  //  set {
//  //  //    if (value == FitMode.Stretch) {
//  //  //      Camera.main.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
//  //  //      Camera.main.ResetAspect();
//  //  //      float d = ActualViewAspect / Sqrt3;
//  //  //      if (d > 1.0f) {
//  //  //        Camera.main.orthographicSize = 8 / d;
//  //  //      } else {
//  //  //        Camera.main.orthographicSize = 8;
//  //  //      }
//  //  //    } else if (value == FitMode.Shrink) {

//  //  //      var wantedRatio = new Vector2(Sqrt3, 1.0f);
//  //  //      var limit = new Vector2(ActualViewAspect, 1.0f);
//  //  //      Vector2 fit = Core.Fit(wantedRatio, limit, FitMode.Shrink);

//  //  //      bool isHorizontallyShrinked = Mathf.Approximately(fit.y, limit.y);
//  //  //      bool isVerticallyShrinked = Mathf.Approximately(fit.x, limit.x);
//  //  //      KumaAssert(isHorizontallyShrinked && !isVerticallyShrinked
//  //  //        || !isHorizontallyShrinked && isVerticallyShrinked);
//  //  //      if (isHorizontallyShrinked) {
//  //  //        float shrinkingRate = fit.x / limit.x;
//  //  //        Camera.main.rect = new Rect((1.0f - shrinkingRate) / 2, 0.0f, shrinkingRate, 1.0f);
//  //  //      } else {
//  //  //        float shrinkingRate = fit.y / limit.y;
//  //  //        Camera.main.rect = new Rect(0.0f, (1.0f - shrinkingRate) / 2, 1.0f, shrinkingRate);
//  //  //      }

//  //  //      Camera.main.aspect = Sqrt3;
//  //  //      Camera.main.orthographicSize = 8;
//  //  //    } else {
//  //  //      Camera.main.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
//  //  //      Camera.main.ResetAspect();
//  //  //      Camera.main.orthographicSize = ;
//  //  //    }
//  //  //    screenFitMode = value;
//  //  //  }
//  //  //}
//  //}
//}