using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace AirKuma {
  //[InitializeOnLoad]
  public class MySceneViewExtension {

      // called when reloading C#
      // also called when starting play mode
     static MySceneViewExtension() {
      //Debug.Log("on reload C#");
      SceneView.duringSceneGui += DuringSceneGui;
    }

    private static void DuringSceneGui(SceneView obj) {

      Event evt = Event.current;

      // uncomment this to enable receive and eat lmb input event
      //if (Event.current.type == EventType.Layout) {
      //  HandleUtility.AddDefaultControl(0);
      //}


      //if (evt.type == EventType.MouseDown && evt.button == 1) {

      //  evt.Use();
      //}


      switch (evt.type) {
        case EventType.MouseDown:
        case EventType.MouseUp:
        case EventType.KeyDown:
        case EventType.KeyUp:
        case EventType.ScrollWheel:
          Debug.Log(evt.type.ToString());
          break;
        case EventType.MouseMove:
        case EventType.MouseDrag:
          Debug.Log(evt.type.ToString());
          break;
        case EventType.Repaint: 
        case EventType.Layout:
          //Debug.Log(evt.type.ToString());
          break;
        case EventType.DragUpdated:
        case EventType.DragPerform:
        case EventType.DragExited:
          Debug.Log(evt.type.ToString());
          break;
        case EventType.Used:
          Debug.Log(evt.type.ToString());
          break;
        case EventType.MouseEnterWindow:
        case EventType.MouseLeaveWindow:
          Debug.Log(evt.type.ToString());
          break;
      }
    }
  }
}