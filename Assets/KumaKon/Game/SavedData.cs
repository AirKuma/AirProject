using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace AirKuma {

  // see: https://unity3dtuts.com/saving-and-loading-scriptable-object-in-unity/

  [Serializable]
  public struct ItemStorage {
    public string itemName;
    public int itemCount;
  }

  // In order to use it at run-time, it should be created in "Resources" folder.
  // Only one savedData should be created.
  [CreateAssetMenu(fileName = "SavedData", menuName = "Create Saved Data")]
  public class SavedData : ScriptableObject {

    public Font defaultFont;

    public Texture bearIcon;

    public List<ItemStorage> ItemStorages = new List<ItemStorage>();
  }

  public class SaveLoad {

    public static SavedData savedData;

    const string savedFileName = "SavedData.txt";

    public static void Reload() {
      if (!File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + savedFileName)) {
        savedData = Resources.Load<SavedData>("SavedData");
      } else {
        savedData = ScriptableObject.CreateInstance<SavedData>();
        string json = File.ReadAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + savedFileName);
        JsonUtility.FromJsonOverwrite(json, savedData);
      }
    }

    public static SavedData Data {
      get {
        if (savedData == null) {
          Reload();
        }
        return savedData;
      }
    }

    public static void Save() {
      if (savedData != null) {
        string json = JsonUtility.ToJson(savedData);
        File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + savedFileName, json);
      }
    }


  }
}