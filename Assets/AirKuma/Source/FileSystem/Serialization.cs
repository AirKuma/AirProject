//using System;

//namespace AirKuma.FileSys {
//  public static class Persistent<T> where T : class {

//    private static T obj;

//    private static DateTime lastLoadTime;
//    private static DateTime lastSaveTime;
//    private static DateTime lastBackupTime;

//    private static readonly TimeSpan minimalSavingTimeSpan = new TimeSpan(0, 0, 1);

//    public static T Load() {
//      if (obj is null) {
//        obj = typeof(T).GetAttr<SerializationSpec>().filePath.Deserialize<T>();
//      }
//      lastLoadTime = DateTime.Now;
//      return obj;
//    }
//    public static void Save(bool forcing = false) {
//      if (forcing || lastSaveTime < lastLoadTime) {
//        TimeSpan span = (DateTime.Now - lastSaveTime);
//        if (forcing || span > minimalSavingTimeSpan) {
//          try {
//            if (forcing || typeof(T).GetAttr<SerializationSpec>().backupOnSaving) {
//              TimeSpan? interval = typeof(T).GetAttr<SerializationSpec>().backupOnSavingInterval;
//              if (forcing || interval.HasValue.Not() || DateTime.Now - lastBackupTime > interval.Value) {
//                typeof(T).GetAttr<SerializationSpec>().filePath.AirBackup();
//                lastBackupTime = DateTime.Now;
//              }
//            }
//            obj.Serialize(typeof(T).GetAttr<SerializationSpec>().filePath);
//            lastSaveTime = DateTime.Now;
//          }
//          catch (System.IO.IOException xpt) {
//            if (DiagnosticConfig.WarnEveryThing)
//              UnityEngine.Debug.LogWarning("edit not saved: " + xpt.Message);
//          }
//        }
//      }
//    }

//    public static T Access {
//      get => Load();
//      set {
//        obj = value;
//        Save();
//      }
//    }
//  }
//}
