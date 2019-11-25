using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMGUItest : MonoBehaviour
{
  public Texture2D icon;
  private bool toggleBool = true;
  private int toolbarInt = 0;
  private float hScrollbarValue;
  private Rect windowRect = new Rect(20, 20, 120, 50);
  private int selectedToolbar = 0;
  private string[] toolbarStrings = { "One", "Two" };
  public GUIStyle customButton;
  public GUISkin mySkin;
  private bool toggle = true;
  private float sliderValue = 1.0f;
  private float maxSliderValue = 10.0f;
  private float mySlider = 1.0f;
  public Color myColor;

  // Start is called before the first frame update
  void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  private void OnGUI() {
    GUI.skin = mySkin;

    // Now create any Controls you like, and they will be displayed with the custom Skin
    GUILayout.Button("I am a re-Skinned Button");

    // You can change or remove the skin for some Controls but not others
    GUI.skin = null;

    // Any Controls created here will use the default Skin and not the custom Skin
    GUILayout.Button("This Button uses the default UnityGUI Skin");
  }

  Color RGBSlider(Rect screenRect, Color rgb) {
    rgb.r = GUI.HorizontalSlider(screenRect, rgb.r, 0.0f, 1.0f);

    // <- Move the next control down a bit to avoid overlapping
    screenRect.y += 20;
    rgb.g = GUI.HorizontalSlider(screenRect, rgb.g, 0.0f, 1.0f);

    // <- Move the next control down a bit to avoid overlapping
    screenRect.y += 20;

    rgb.b = GUI.HorizontalSlider(screenRect, rgb.b, 0.0f, 1.0f);
    return rgb;
  }
}
