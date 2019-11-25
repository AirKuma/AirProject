using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class EditorTimeAndRuntimeComponent : MonoBehaviour {

  public int Data;

  // called before the first frame update in play mode
  // with [ExecuteAlways]: also called when loading scene in editor mode and switching to editor mode
  void Start() {
    
    Debug.Log("start!!!");
  }

  // called once per frame in play mode
  // with [ExecuteAlways]: also frequently called when its data change (such as transformation) in editor mode
  void Update() {
    
    Debug.Log("update!!");
  }
}
     