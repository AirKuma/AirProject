using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AirKuma.UnityCore {

  public enum PutModelOperation {
    AddTogether,
    ClearOccupyingOneAndAdd,
    AddIfNoOccupyingOne,
  }

  [Serializable]
  public class PrefabSet : AirSet<PrefabProxy> { }

  [Serializable]
  public class PrefabProxy {

    [SerializeField]
    public string prefabPath;

    public PrefabProxy(string prefabPath) {
      this.prefabPath = prefabPath;
    }

    GameObject GameObject => 
      new AssetFileProxy(this.prefabPath).MainAssetObject.UnityObject as GameObject
      ?? throw new InvalidOperationException();


    public static implicit operator PrefabProxy(string prefabPath) {
      return new PrefabProxy(prefabPath);
    }

    public GameObject Instantiate(GameObject par = null) {
      GameObject go = PrefabUtility.InstantiatePrefab(GameObject) as GameObject ?? throw new Exception();
      if (par != null)
        go.transform.parent = par.transform;
      return go;
    }
    public void Instantiate(Vector3 center, GameObject par = null) {
      var go = this.Instantiate(par);
      go.transform.position = center;
    }

    public override bool Equals(object obj) {
      if (!(obj is PrefabProxy))
        return false;

      var proxy = (PrefabProxy)obj;
      return prefabPath == proxy.prefabPath;
    }

    public override int GetHashCode() {
      return 234013412 + EqualityComparer<string>.Default.GetHashCode(prefabPath);
    }

    public override string ToString() {
      return $"prefab at '{prefabPath}'";
    }
    //public GetOccupiedBlocks
  }

  public static class PrefabEx {


    public static GameObject GetPrefab(this string pathWithoutExt) {
      return (pathWithoutExt + ".prefab").GetAsset() as GameObject ?? throw new Exception();
    }


    //public static GameObject GetOrAddOPrefabInstance(this GameObject self, string objName, string prefabPath) {
    //  return self.GetChildNamed(objName)
    //    ?? new PrefabId(prefabPath).NewPrefabInstance(self.transform);
    //}

    //PrefabUtility.prefabInstanceUpdated
  }
}
