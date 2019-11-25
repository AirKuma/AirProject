//using AirKuma.UnityCore;
//using UnityEngine;

//namespace AirKuma.Game {

//  //[Serializable]
//  //public class Scene {

//  //}

//  [ExecuteAlways, RequireComponent(typeof(BoxCollider))]
//  public class AirZone : MonoBehaviour {


//    const string LayerName = "AirZones";

//    //[SerializeField]
//    //public UnityEngine.Object sceneAsset;

//    [AirSceneField, SerializeField]
//    UnityEngine.Object scene;

//    [SerializeField]
//    int builtIndex; // this should be set before building game


//    public AirScene TheScene {
//#if UNITY_EDITOR
//      get => new AirScene(UnityEditor.AssetDatabase.GetAssetPath(scene));
//      set {
//        scene = UnityEditor.AssetDatabase.LoadMainAssetAtPath(value.Path);
//        Reposition();
//      }
//#else
//#endif
//    }

//    public int Layer => (int)new LayerIndexProxy(LayerName);

//    public void Reposition() {
//      gameObject.layer = Layer;
//      GetComponent<BoxCollider>().SetBounds(TheScene.GetOverallBounds());
//    }

//  }

//  //public class ConditionalZone : AirZone {


//  //} 

//}