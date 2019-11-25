using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AirKuma.UnityCore {

  public static class GOResolver {
    private static Dictionary<string, GameObject> dict;
    static GOResolver() {
      dict = new Dictionary<string, GameObject>();
    }
    public static GameObject Retrieve(string goPath) {
      if (!dict.TryGetValue(goPath, out GameObject go)) {
        go = GameObject.Find(goPath) ?? throw new InvalidOperationException("reference to non-existing GO");
        dict.Add(goPath, go);
      }
      return go;
    }
    public static void InvalidateCache() {
      dict.Clear();
    }
  }
}
