using UnityEngine;
using UnityEditor;

namespace AirKuma {
  public class MyEditorWindow : EditorWindow {

    public string myString = "Hello World";

    public bool groupEnabled;

    public bool myBool = true;
    public float myFloat = 1.23f;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/My Window _g")]
    static void Abbbb() {
      // Get existing open window or if none, make a new one:
      MyEditorWindow window = (MyEditorWindow)EditorWindow.GetWindow(typeof(MyEditorWindow));
      window.Show();
    }

    void OnGUI() {

      var imgui = new Imgui();
      imgui.Reset(this.position.size);
      imgui.Scale(0.5f);
      imgui.Image(SaveLoad.Data.bearIcon, FitIntoMode.ShrinkToFit);
      imgui.Border(Color.red, 10);
      imgui.Text("Pressed me!", Color.blue);
      if (imgui.MouseButtonHits(Imgui.Lmb))
        Debug.Log("OK");


      //GUILayout.Label("Base Settings", EditorStyles.boldLabel);
      //myString = EditorGUILayout.TextField("Text Field", myString);

        //groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        //myBool = EditorGUILayout.Toggle("Toggle", myBool);
        //myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        //EditorGUILayout.EndToggleGroup();
    }
  }
}