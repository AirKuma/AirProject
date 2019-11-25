using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeEventObserver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  private void OnGUI() {
    Event evt = Event.current;

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
        Debug.Log(evt.type.ToString());
        break;
      case EventType.DragUpdated:
      case EventType.DragPerform:
      case EventType.DragExited:
        Debug.Log(evt.type.ToString());
        break;
      case EventType.Used:
        Debug.Log(evt.type.ToString());
        break;
    }
  }
}

