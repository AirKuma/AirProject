using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using static AirKuma.KumaKonUtils;

namespace AirKuma {
  public enum PlayerAction {
    GeneralInteract,
    Sword,
    Bow,
    Axe,
    Pickaxe,
    Shovel,
    Hammer,
    Saw,
  }

  [RequireComponent(typeof(Rigidbody))]
  public class PlayerCharacter : MonoBehaviour {

    private int currentCollisionCount;
    public bool IsColliding => currentCollisionCount != 0;

    public UnityEngine.Object startScene;

    private float MoveDistancePerSecond { get; set; } = 6f;

    public float JumpForce { get; set; } = 6;

    //public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents, Quaternion orientation = Quaternion.identity, int layerMask = AllLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal);

    void DoAction(PlayerAction action, Bounds bounds) {
      var colliders = Physics.OverlapBox(this.transform.position + bounds.center, bounds.extents);
      foreach (var collider in colliders) {
        if (collider.GetComponent<Interaction>()) {
          collider.GetComponent<Interaction>().ReceiveAction(this.gameObject, action);
        }
      }
    }


    // Start is called before the first frame update
    void Start() {
      if (Application.isPlaying) {
        UnityEngine.Object.DontDestroyOnLoad(this);
        string path = UnityEditor.AssetDatabase.GetAssetPath(startScene);
        this.EnterScene(path, "PlayerEntry");
      }
    }


    private void OnCollisionEnter(Collision collision) {
      //Debug.Log("start to collide");
      ++currentCollisionCount;
    }

    private void OnCollisionStay(Collision collision) {

    }
    private void OnCollisionExit(Collision collision) {
      //Debug.Log("stop colliding");
      --currentCollisionCount;
    }



    private void FixedUpdate() {
      var evt = Event.current;

      Vector3 potentialOffset = Vector3.zero;

      if (Input.GetKey(KeyCode.UpArrow))
        potentialOffset += Vector3.forward;
      if (Input.GetKey(KeyCode.DownArrow))
        potentialOffset += Vector3.back;
      if (Input.GetKey(KeyCode.LeftArrow))
        potentialOffset += Vector3.left;
      if (Input.GetKey(KeyCode.RightArrow))
        potentialOffset += Vector3.right;

      if (potentialOffset != Vector3.zero) {
        var actualOffset = potentialOffset.normalized * (MoveDistancePerSecond * Time.fixedDeltaTime);
        //Debug.Log("actual offset = " + actualOffset.x.ToString()
        //  + ", " + actualOffset.z.ToString());
        this.transform.position += actualOffset;
      }
    }

    private void OnGUI() {
      Event e = Event.current;
      if (e.type == EventType.KeyDown) {
        if (e.keyCode == KeyCode.Space) {
          if (IsColliding) {
            if (FloatApproxEq(GetComponent<Rigidbody>().velocity.y, 0)) {
              GetComponent<Rigidbody>().AddForce(new Vector3(0, JumpForce, 0),
                ForceMode.Impulse);
            }
          }
        }
        if (e.keyCode == KeyCode.E) {
          this.DoAction(PlayerAction.GeneralInteract, new Bounds(Vector3.zero, new Vector3(2, 2, 2)));
        }
      }
    }

    IEnumerator LoadAsyncCoroutine(string path, Action afterLoading) {
      AsyncOperation op = SceneManager.LoadSceneAsync(path, LoadSceneMode.Single);
      while (!op.isDone) {
        yield return null;
      }
      afterLoading?.Invoke();
    }

    MapEntry ResolveEntry(string entryName) {
      foreach (var entry in GameObject.FindObjectsOfType<MapEntry>()) {
        if (entry.gameObject.name == entryName) {
          return entry;
        }
      }
      return null;
    }

    public void EnterScene(string scenePath, string entryName) {
      Debug.Log($"Enter the entry '{entryName}' of the scene '{scenePath}'.");
      //Scene unityScene = SceneManager.GetSceneByPath(scenePath);
      this.StartCoroutine(LoadAsyncCoroutine(scenePath, () => {
        MapEntry entry = ResolveEntry(entryName);
        if (entry != null) {
          this.transform.position = entry.transform.position;
        } else {
          throw new Exception($"entry named '{entryName}' is not found in scene '{scenePath}'");
        }
      }));
    }


  }
}
