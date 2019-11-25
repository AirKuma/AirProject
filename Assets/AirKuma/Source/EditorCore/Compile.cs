//#define LOG_WATCHER
#define LOG_COMPILE
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace AirKuma {

  public static class UnityCompiler {

    //[MenuItem("Kuma/refresh AirSystem")]
    //public static void NewAirSystem() {
    //  AirSystem dummy = AirSystem.Service;
    //}

    //private static Tool lastTool = Tool.None;

    //[MenuItem("Kuma/Hide Builtin Transform Handle")]
    //private static void HideBuiltinTransformHandle() {
    //  lastTool = Tools.current;
    //  Tools.current = Tool.None;
    //}
    //[MenuItem("Kuma/Show Builtin Transform Handle")]
    //private static void ShowBuiltinTransformHandle() {
    //  Tools.current = lastTool;
    //}

    [MenuItem("Kuma/Reload And Lock")]
    public static void ReloadSource() {
      EditorApplication.UnlockReloadAssemblies();
      EditorApplication.LockReloadAssemblies();
    }

    [MenuItem("Kuma/Lock Reloading")]
    public static void LockReloading() {
      EditorApplication.LockReloadAssemblies();
    }

    //[MenuItem("Kuma/Lock Reloading")]
    //public static void LockReloading() {
    //  BuildPipeline.BuildPlayer();
    //}

    [MenuItem("Kuma/Unlock Reloading")]
    public static void UnlockReloading() {
      EditorApplication.UnlockReloadAssemblies();
    }
  }


}