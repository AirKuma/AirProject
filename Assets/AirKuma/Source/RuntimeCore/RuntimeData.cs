//using System;
//using System.IO;
//using System.Reflection;
//using UnityEngine;

//namespace AirKuma {

//  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
//  public class DataSpec : Attribute {
//    public int dataId;

//    public DataSpec(int dataId) {
//      this.dataId = dataId;
//    }
//    public DataSpec(uint dataId) {
//      this.dataId = unchecked((int)dataId);
//    }
//  }

//  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//  public class SLData { }

//  public static class SLDataMgr {

//#if UNITY_STANDALONE
//    private const string RuntimeDataFolderPath = "AirData";
//#else
//      static readonly string RuntimeDataFolderPath = Application.persistentDataPath + "/AirData";
//#endif
//    //============================================================
//    private static object NewObjOrLoadObjFromSerializableFile(string path, Type type) {
//      if (!File.Exists(path)) {
//        return Activator.CreateInstance(type);
//      }
//      string json = File.ReadAllText(path);
//      return JsonUtility.FromJson(json, type);
//    }

//    private static string SavedFilePath(string guidStr) {
//      return RuntimeDataFolderPath + "/" + guidStr + ".txt";
//    }

//    //============================================================
//    public static T Load<T>() where T : SLData, new() {
//      if (!SingletonCache.TryGet(typeof(T), out object obj)) {
//        string hex = typeof(T).GetCustomAttribute<DataSpec>(false).dataId.ToHexString();
//        obj = NewObjOrLoadObjFromSerializableFile(SavedFilePath(hex), typeof(T));
//        SingletonCache.Add(typeof(T), obj);
//      }
//      return (T)obj;
//    }
//    public static void Save<T>() where T : SLData, new() {
//      if (SingletonCache.TryGet(typeof(T), out object obj)) {
//        string json = JsonUtility.ToJson(obj);
//        string hex = typeof(T).GetCustomAttribute<DataSpec>().dataId.ToHexString();
//        File.WriteAllText(SavedFilePath(hex), json);
//      }
//    }
//    //============================================================
//  }

//  public class RWSLData { }

//  public static class RWSLDataMgr {

//#if UNITY_STANDALONE
//    private const string RuntimeDataFolderPath = "AirData";
//#else
//      static readonly string RuntimeDataFolderPath = Application.persistentDataPath + "/AirData";
//#endif
//    //============================================================
//    private static object NewObjOrLoadObjFromSerializableFile(string path, Type type) {
//      if (!File.Exists(path)) {
//        return Activator.CreateInstance(type);
//      }
//      string json = File.ReadAllText(path);
//      return JsonUtility.FromJson(json, type);
//    }

//    private static string SavedFilePath(string guidStr) {
//      return RuntimeDataFolderPath + "/" + guidStr + ".txt";
//    }

//    //============================================================
//    public static T Load<T>() where T : RWSLData, new() {
//      if (!SingletonCache.TryGet(typeof(T), out object obj)) {
//        string hex = typeof(T).GetCustomAttribute<DataSpec>(false).dataId.ToHexString();
//        obj = NewObjOrLoadObjFromSerializableFile(SavedFilePath(hex), typeof(T));
//        SingletonCache.Add(typeof(T), obj);
//      }
//      return (T)obj;
//    }
//    public static void Save<T>() where T : RWSLData, new() {
//      if (SingletonCache.TryGet(typeof(T), out object obj)) {
//        string json = JsonUtility.ToJson(obj);
//        string hex = typeof(T).GetCustomAttribute<DataSpec>().dataId.ToHexString();
//        File.WriteAllText(SavedFilePath(hex), json);
//      }
//    }
//    //============================================================
//  }
//}