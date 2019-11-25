using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace AirKuma {

  [RequireComponent(typeof(BoxCollider))]
  public class MapGate : MonoBehaviour {

    public UnityEngine.Object targetScene;
    public string targetEntryName;

    public void Enter(GameObject character) {
      string path = UnityEditor.AssetDatabase.GetAssetPath(targetScene);
      if (character.GetComponent<PlayerCharacter>()) { character.GetComponent<PlayerCharacter>().EnterScene(path, this.targetEntryName); }
    }

    private void OnCollisionEnter(Collision collision) {

      if (collision.gameObject.GetComponent<PlayerCharacter>()) {
        this.Enter(collision.gameObject);
      }

    }

  }
}