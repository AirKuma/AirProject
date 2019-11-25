using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeComponent : MonoBehaviour {

  // called before the first frame update in play mode
  void Start() {
    Debug.Log("start in play mode");
  }

  // called once per frame in play mode
  void Update() {
    Debug.Log("update during play mode");
  }
}
