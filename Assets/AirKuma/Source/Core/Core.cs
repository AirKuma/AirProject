using UnityEngine;
using System;
using System.Collections.Generic;

namespace AirKuma {


  

  [Serializable]
  public class AirSet<T> : HashSet<T> {

  }
  [Serializable]
  public class AirMap<TKey, TValue> : Dictionary<TKey, TValue> {

  }

  public static class Misc {

    public static Vector3 GetVectorFromKeyCode(KeyCode keyCode) {
      if (keyCode == KeyCode.UpArrow)
        return new Vector3(0, 0, +1);
      if (keyCode == KeyCode.DownArrow)
        return new Vector3(0, 0, -1);
      if (keyCode == KeyCode.LeftArrow)
        return new Vector3(-1, 0, 0);
      if (keyCode == KeyCode.RightArrow)
        return new Vector3(+1, 0, 0);
      throw new Exception();
    }

    public static Predicate<TPredArg> InvertCondition<TPredArg>(this Predicate<TPredArg> pred) {
      return (TPredArg arg) => !pred(arg);
    }

    public static bool IsTrue(this bool? b) {
      return b.HasValue && b.Value;
    }

    public static float ToRad(this float deg) {
      return deg * Mathf.Deg2Rad;
    }
    public static float ToDeg(this float rad) {
      return rad * Mathf.Rad2Deg;
    }
         
    public static void ExecCommandLine(this string cmdLineStr) {
      UnityEngine.Debug.Log($"run command '{cmdLineStr}'");
      var process = new System.Diagnostics.Process();
      var startInfo = new System.Diagnostics.ProcessStartInfo {
        WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
        //startInfo.FileName = "cmd.exe";
        //startInfo.Arguments = $"/C {cmdLineStr}";
        FileName = "powershell.exe",
        Arguments = "-NoLogo -NonInteractive -NoProfile -Command " + cmdLineStr
      };
      process.StartInfo = startInfo;
      process.Start();
    }
  }


  public enum TodoGoal {
    UseAlgorithmOfProperTimeComplex,
    Cache,
    Implement,
    Optimize,
    CheckPerformanceIssue,
    FreeMemory,
    UseStackContainerInstead,
    ProvideExceptionDetail,
    AbstractOutDuplicatedCode,
    MoreSupportedInput,
    MoreSupportedOption,
    RemoveUnusedCode,
    CheckLicense,
  }
  [AttributeUsage(AttributeTargets.Class
      | AttributeTargets.Struct
      | AttributeTargets.Constructor
      | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
  public class Todo : Attribute {
    private TodoGoal goal;
    string desc;

    public Todo(TodoGoal goal, string desc = null) {
      this.goal = goal;
      this.desc = desc;
    }
  }

  public static class BoolEx {
    public static bool Opportunity(float rate) {
      return FloatEx.Random() <= rate;
    }
  }

  public static class Logging {
    public static Color GetLoggingColor(this LogType type) {
      switch (type) {
        case LogType.Exception:
          return Color.red;
        case LogType.Assert:
          return Color.yellow;
        case LogType.Error:
          return ColorEx.Orange;
        case LogType.Warning:
          return Color.yellow;
        case LogType.Log:
          return Color.green;
        default:
          throw new Exception();
      }
    }

    public static void PrintLine(this string msg, LogType scenario) {
      switch (scenario) {
        case LogType.Log:
          Debug.Log(msg);
          break;
        case LogType.Warning:
          Debug.LogWarning(msg);
          break;
        case LogType.Error:
          Debug.LogError(msg);
          break;
        default:
          throw new Exception();
      }
    }
  }

  // note: extension method call in this class will be ignored in stack trace output
  public static class DebugEx {

    //============================================================
    public static bool Not(this bool condition) {
      return !condition;
    }
    //============================================================
    // for both debug build only
    [System.Diagnostics.Conditional("DEBUG")]
    public static void Assert(this bool condition, object msg = null) {
      if (!condition)
        throw new Exception();
      //if (msg is null) {
      //  UnityEngine.Debug.Assert(condition);
      //} else {
      //  UnityEngine.Debug.Assert(condition, msg);
      //}
    }
    // for both debug and release build
    public static void Check(this bool condition, object msg = null) {
      if (msg is null) {
        UnityEngine.Debug.Assert(condition);
      } else {
        UnityEngine.Debug.Assert(condition, msg);
      }
    }
    //============================================================
    [System.Diagnostics.Conditional("DEBUG")]
    public static void Log(this string msg) {
      Logging.PrintLine(msg, LogType.Log);
    }
    [System.Diagnostics.Conditional("DEBUG")]
    public static void LogWarning(this string msg) {
      Logging.PrintLine(msg, LogType.Warning);
    }
    [System.Diagnostics.Conditional("DEBUG")]
    public static void LogError(this string msg) {
      Logging.PrintLine(msg, LogType.Error);
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void LogUnityGuiEvent(this Event evt) {
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
          break;
        case EventType.Repaint:
        case EventType.Layout:
          break;
        case EventType.DragUpdated:
        case EventType.DragPerform:
        case EventType.DragExited:
          Debug.Log(evt.type.ToString());
          break;
        case EventType.Ignore:
        case EventType.Used:
          Debug.Log(evt.type.ToString());
          break;
        case EventType.ValidateCommand:
        case EventType.ExecuteCommand:
        case EventType.ContextClick:
        case EventType.MouseEnterWindow:
        case EventType.MouseLeaveWindow:
          Debug.Log(evt.type.ToString());
          break;
      }
    }
  }
}