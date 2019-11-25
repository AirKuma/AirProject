using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using AirKuma.UnityCore;
using AirKuma.HitTest;

namespace AirKuma.UnityCore {

  public static class CustomHierarchyMenu {


    //============================================================

    //[MenuItem("Kuma/Test/String Parsing")]
    //public static void TestTextParsing() {
    //  //Debug.Log("aa (bb cc) dd".GetFullView().EachBalancedWord().ToStringForEach());
    //  //StringView str2 = "'aa' (bb(cc)'ff') 'dd'".GetFullView().StripBoundaryWhitespaces();
    //  //Debug.Log(str2.ToString());
    //  //StringBuilder str = str2.Builder.RemoveContinousWhitespaces();
    //  //Debug.Log(str.ToString());
    //  //Debug.Log(str.GetFullView().InsertOuterSpaces().ToString());
    //  //Debug.Log("'aa' (bb(cc)'ff') 'dd'".GetFullView().EachTerm().ToStringForEach());

    //  Debug.Log("'aa' (bb(cc)'ff')'ii''kk'jj'dd'".KumaFormat());
    //  Debug.Log("'aa'(bb(cc)'ff')'dd'jj".KumaFormat());
    //  Debug.Log("  'aa'   '  (bb (cc) ff) ' 'dd'  ".KumaFormat());
    //}



    //============================================================

    // note: menu item function for hierarchy will only called for each unrelated ancenstor GameObject
    // note: !! doc: should call GameObjectUtility.SetParentAndAlign?

    public static GameObject GetGameObject(this MenuCommand cmd) {
      return cmd.context as GameObject;
    }

    //============================================================
    [MenuItem("GameObject/Kuma/delete children", false, 0)]
    public static void DeleteChildren(MenuCommand cmd) {
      cmd.GetGameObject().RemoveChildren();
    }

    //============================================================
    [MenuItem("GameObject/Kuma/rename as first behaviour", false, 0)]
    public static void RenameAsFirstBehaviour(MenuCommand cmd) {
      cmd.GetGameObject().RenameAsType<Behaviour>();
    }

    [MenuItem("GameObject/Kuma/Save Prefab", false, 0)]
    public static void SavePrefab(MenuCommand cmd) {
      if (cmd.GetGameObject().IsPrefabRoot()) {
        PrefabUtility.ApplyPrefabInstance(cmd.GetGameObject(), InteractionMode.UserAction);
      }
      else {
        Debug.LogWarning($"'{cmd.GetGameObject().GetFindingPath()}' is not a prefab root");
      }
    }



    [MenuItem("GameObject/Kuma/__Test__", false, 0)]
    public static void TestCommand(MenuCommand cmd) {
      Debug.Log(cmd.context);
    }

    [MenuItem("GameObject/Kuma/rename descendants as first behaviours", false, 0)]
    public static void RenameDescendantsAsFirstBehaviours(MenuCommand cmd) {
      Debug.Log($"Rename As Type Recursively for {cmd.GetGameObject().GetFindingPath()}");
      cmd.GetGameObject().RenameAsTypeRecursively<Behaviour>();
    }

    [MenuItem("GameObject/Kuma/rename children as first behaviour", false, 0)]
    public static void RenameChildrenAsFirstBehaviour(MenuCommand cmd) {
      Debug.Log($"Rename Children As Type Recursively for {cmd.GetGameObject().GetFindingPath()}");
      foreach (Transform childTrf in cmd.GetGameObject().transform) {
        childTrf.gameObject.RenameAsTypeRecursively<Behaviour>();
      }
    }

    //============================================================
    [MenuItem("GameObject/Kuma/parent under empty GameObject", false, 0)]
    public static void ParentUnderEmpty(MenuCommand cmd) {
      cmd.GetGameObject().AttachToNewEmptyParent();
    }
    [MenuItem("GameObject/Kuma/create empty sibling GameObject", false, 0)]
    public static void CreateEmptySibling(MenuCommand cmd) {
      var newOne = new GameObject("");
      newOne.SetParent(cmd.GetGameObject().GetParent());
      newOne.SetSiblingIndex(cmd.GetGameObject().GetSiblingIndex() + 1);
    }
    //============================================================
    [MenuItem("GameObject/Kuma/activate descendants", false, 0)]
    public static void ActivateDescendants(MenuCommand cmd) {
      cmd.GetGameObject().SetDescendantActivities(true);
    }
    [MenuItem("GameObject/Kuma/deactivate descendants", false, 0)]
    public static void DeactivateDescendants(MenuCommand cmd) {
      cmd.GetGameObject().SetDescendantActivities(false);
    }
    //============================================================
    [MenuItem("Kuma/Clear Log _#r")]
    public static void ClearLogConsole() {
      try {
        var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
        MethodInfo clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
      }
      catch {
        Debug.LogWarning("can not clear log via reflection");
      }
    }

    //============================================================
    private static void HierarchyInfo(this StringBuilder str, GameObject obj) {
      str.Append($"'{obj.name} <id {obj.GetInstanceID()}>'");
      if (obj.GetChildren().Count() != 0) {
        str.Append("{");
        foreach (GameObject child in obj.GetChildren()) {
          str.HierarchyInfo(child);
        }
        str.Append("}");
      }
    }
    //[MenuItem("GameObject/Kuma/hierarchy info", false, 0)]
    //public static void HierarchyInfo(this MenuCommand cmd) {
    //  var str = new StringBuilder("hierarchy-info");
    //  GameObject obj = cmd.GetGameObject();
    //  //KumaTree<string> info = new KumaTree<string>(obj.name);
    //  str.HierarchyInfo(obj);
    //  Debug.Log(str.ToString().HeadTail());
    //}

    //[MenuItem("Kuma/About Selected Assets", false, 0)]
    //public static void AboutSelectedAssets() {
    //  foreach (string guid in SelectionManager.Service.SelectedIndividualAssetGuids) {
    //    Debug.Log("::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
    //    UnityEngine.Object[] objs = guid.ToAssetFileByGuid().LoadedObjects;
    //    string str = objs.ToStringForEach();
    //    Debug.Log(str.KumaFormat());
    //  }
    //}
  }

  public static class UnityEditorCoreExtensions {

    public static Ray MouseRay(this Event e) {
      return HandleUtility.GUIPointToWorldRay(e.mousePosition);
    }
    public static bool MouseHit(this Event e, out RaycastHit hit) {
      return e.MouseRay().RayCastFirst(out hit);
    }
  }
}