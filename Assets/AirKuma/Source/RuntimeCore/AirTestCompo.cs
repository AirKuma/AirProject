using System;
using UnityEngine;
using AirKuma.UnityCore;
using UnityEditor;

namespace AirKuma {
  
   
  [ExecuteAlways]
  public class AirTestCompo : MonoBehaviour {

    //[Serializable]
    // public class MyMap : UnorderedMap<int, int> { }

    // [SerializeField]
    // MyMap myMap = new MyMap();

    private void OnEnable() {
      var time = EditorApplication.timeSinceStartup; 
      var catalog = AttributedTypeCache.catalog;
      Debug.Log("span " + (EditorApplication.timeSinceStartup - time).ToString());
      //this.myMap.Add(myMap.Count, 123);
      //Debug.Log(myMap.Count);
      //Debug.Log("<test>"); 
      //Texture2D txt = new ExternalResourceLoader("C:/Users/imyou/Desktop/MyImage.jpg").LoadAsImaageTexture();
      //Debug.Log("</test>");
    }
  }
}
 