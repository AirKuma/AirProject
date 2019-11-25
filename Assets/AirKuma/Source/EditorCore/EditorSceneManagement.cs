using System;
using AirKuma.FileSys;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AirKuma.UnityCore {

  [Serializable]
  public struct AirSceneEdit {

    public Scene unityScene;

    public AirSceneEdit(Scene unityScene) {
      this.unityScene = unityScene;
    }
    public AirSceneEdit(int buildIndex) : this(SceneManager.GetSceneByBuildIndex(buildIndex)) {
    }
    public AirSceneEdit(string scenePath) : this(SceneManager.GetSceneByPath(scenePath)) { }
    public AirSceneEdit(UnityEngine.Object sceneAsset) : this(AssetDatabase.GetAssetPath(sceneAsset)) { }

    public string Path => unityScene.path;

    public UnityEngine.Object UnitySceneAsset => AssetDatabase.LoadMainAssetAtPath(Path);

    public bool InHierarchy =>
#pragma warning disable CS0618 // Type or member is obsolete
        EditorSceneManager.GetAllScenes().Has(unityScene);
#pragma warning restore CS0618 // Type or member is obsolete


    public bool Opened => InHierarchy && unityScene.isLoaded;

    public string PathInBuildList => Path.GetRelativePathUnder("Assets");

    public void AddToBuiltList() {
      throw new Exception();
      //Debug.Log($"add the scene '{PathInBuildList}' to built list");
      //EditorBuildSettings.scenes = EditorBuildSettings.scenes.GetAppendedWith(new EditorBuildSettingsScene(Path, true));
    }
    public bool IsInBuiltList() {
      foreach (EditorBuildSettingsScene setting in EditorBuildSettings.scenes) {
        //(setting.path, setting.enabled).ToString().Log();  
        if (setting.path == Path) {
          return true;
        }
      }
      return false;
      //return EditorBuildSettings.scenes.Has(new EditorBuildSettingsScene(Path, true));
    }

    //============================================================

    public void Open(OpenSceneMode mode = UnityEditor.SceneManagement.OpenSceneMode.Additive) {
      Debug.Log($"open the scene '{Path}'");
      EditorSceneManager.OpenScene(Path, mode);
    }
    public void Collapse() {
      Debug.Log($"collapse the scene '{Path}'");
      EditorSceneManager.CloseScene(unityScene, false);
    }
    public void Close() {
      Debug.Log($"close the scene '{Path}'");
      EditorSceneManager.CloseScene(unityScene, true);
    }
    public void Save() {
      EditorSceneManager.SaveScene(unityScene);
    }

    //============================================================
  }
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  [CustomPropertyDrawer(typeof(AirSceneField))]
  public class AirSceneFieldDrawer : PropertyDrawer {


    public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label) {

      //EditorGUI.ObjectField(pos, property, label);
      //UnityEngine.Object tmp = property.objectReferenceValue;

      UnityEngine.Object tmp = EditorGUI.ObjectField(pos, property.objectReferenceValue, typeof(UnityEngine.Object), false);
      if (tmp != property.objectReferenceValue) {
        string path = AssetDatabase.GetAssetPath(tmp);
        if (AirScene.IsSceneAsset(path)) {
          property.objectReferenceValue = tmp;
        }
        else
          Debug.LogError("The given asset is not a scene asset.");
      }

      //// Using BeginProperty / EndProperty on the parent property means that
      //// prefab override logic works on the entire property.
      //EditorGUI.BeginProperty(pos, label, property);

      //// Draw label
      //pos = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), label);

      //// Don't make child fields be indented
      //int indent = EditorGUI.indentLevel;
      //EditorGUI.indentLevel = 0;

      //// Calculate rects
      //var amountRect = new Rect(pos.x, pos.y, 30, pos.height);
      //var unitRect = new Rect(pos.x + 35, pos.y, 50, pos.height);
      //var nameRect = new Rect(pos.x + 90, pos.y, pos.width - 90, pos.height);

      ////// Draw fields - passs GUIContent.none to each so they are drawn without labels
      ////EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("amount"), GUIContent.none);
      ////EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("unit"), GUIContent.none);
      ////EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);

      //// Set indent back to what it was
      //EditorGUI.indentLevel = indent;

      //EditorGUI.EndProperty();
    }

  }
}