using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AirKuma {
  [RequireComponent(typeof(MapEntry))]
  public class MapDoor : Interaction {

    public UnityEngine.Object targetScene;
    public string targetEntryName;

    public void Enter(GameObject character) {
      string path = UnityEditor.AssetDatabase.GetAssetPath(targetScene);
      if (character.GetComponent<PlayerCharacter>()) { character.GetComponent<PlayerCharacter>().EnterScene(path, this.targetEntryName); }
    }

    public override void ReceiveAction(GameObject character, PlayerAction action) {
      if (action == PlayerAction.GeneralInteract) {
        this.Enter(character);
      }
    }
  }
}