//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using UnityEditor;
//using UnityEngine;

//namespace AirKuma {

//  public static class DataUtils {
//    public static UnityEngine.Object LoadOrCreateSOAsset(string path, Type type) {
//      UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, type);
//      if (obj == null) {
//        obj = ScriptableObject.CreateInstance(type);
//        AssetDatabase.CreateAsset(obj, path);
//        AssetDatabase.SaveAssets();
//      }
//      return obj;
//    }
//  }

//  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//  [CreateAssetMenu]
//  public class RWData : ScriptableObject { }

//  public static class RWDataMgr {
//    const string EditorAssetDataFolderPath = "Assets/AirKuma/Data/Editor";

//    public static T Get<T>() where T : RWData, new() {
//      if (!SingletonCache.TryGet(typeof(T), out object obj)) {
//        string hexStr = typeof(T).GetCustomAttribute<DataSpec>().dataId.ToHexString();
//        string path = EditorAssetDataFolderPath + "/" + hexStr + ".asset";
//        obj = DataUtils.LoadOrCreateSOAsset(path, typeof(T));
//        SingletonCache.Add(typeof(T), obj);
//      }
//      return (T)obj;
//    }
//  }

//  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//  [CreateAssetMenu]
//  public class RWLData : ScriptableObject { }
//  public static class RWLDataMgr {
//    const string AssetDataFolderPath = "Assets/AirKuma/Data";

//    public static T Get<T>() where T : RWLData, new() {
//      if (!SingletonCache.TryGet(typeof(T), out object obj)) {
//        var hexStr = typeof(T).GetCustomAttribute<DataSpec>().dataId.ToHexString();
//        string path = AssetDataFolderPath + "/" + hexStr + ".asset";
//        obj = DataUtils.LoadOrCreateSOAsset(path, typeof(T));
//        SingletonCache.Add(typeof(T), obj);
//      } 
//      return (T)obj;
//    }
//  }

//  [DataSpec(0x3F053E91)]
//  class FullScreenEditorSettings : RWData {

//    [SerializeField]
//    public bool keepMenuBar = false;
//    [SerializeField]
//    public bool keepTaskBar = false;

//    [SettingsProvider]
//    public static SettingsProvider Provide() {
//      var settings = new SerializedObject(RWDataMgr.Get<FullScreenEditorSettings>());
//      void GuiCall(string search) {
//        settings.UpdateIfRequiredOrScript();
//        EditorGUILayout.PropertyField(settings.FindProperty("keepMenuBar"));
//        EditorGUILayout.PropertyField(settings.FindProperty("keepTaskBar"));
//        settings.ApplyModifiedProperties();
//      }
//      return new SettingsProvider("AirKuma/Full Screen Mode", SettingsScope.User) {
//        guiHandler = GuiCall,
//        keywords = new List<string>(),
//      };
//    }

//  }
//}