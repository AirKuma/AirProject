using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace AirKuma {

  public enum FitIntoMode {
    ShrinkToFit, //  uniform scale to make it inside the region
    ExtentedToFit, // uniform scale to cover over the region
  }

  public static class KumaKonUtils {


    static string lastOpenedScenePath = null;

    [MenuItem("AirKuma/Enter Play Mode")]
    static void EnterPlayMode() {
      lastOpenedScenePath = EditorSceneManager.GetActiveScene().path;
      EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
      EditorSceneManager.OpenScene("Assets/KumaKon/Game/PersistentScene.unity");
      EditorApplication.isPlaying = true;
    }
    [MenuItem("AirKuma/Exit Play Mode")]
    static void ExitPlayMode() {
      EditorApplication.isPlaying = false;
      if (lastOpenedScenePath != null) {
        Debug.Log($"reopen last opened scene '{lastOpenedScenePath}'");
        EditorSceneManager.OpenScene(lastOpenedScenePath);
      }
    }

    public static Rect CenteredScale(this Rect self, Vector2 scalage) {
      Vector2 scaledSize = self.size * scalage;
      Vector2 padding = (self.size - scaledSize) / 2.0f;
      return new Rect(self.position + padding, scaledSize);
    }

    public static Vector2 UpperLeft(this Rect rect) {
      return rect.position;
    }
    public static Vector2 UpperRight(this Rect  rect) {
      return new Vector2(rect.position.x + rect.size.x, rect.position.y);
    }
    public static Vector2 LowerLeft(this Rect rect) {
      return new Vector2(rect.position.x, rect.position.y + rect.size.y);
    }
    public static Vector2 LowerRight(this Rect rect) {
      return new Vector2(rect.position.x + rect.size.x, rect.position.y + rect.size.y);
    }

    public static int CountNewlines(this string str, bool countsMissingEndNewline = false) {
      int c = 0;
      for (int i = 0; i != str.Length; ++i) {
        if (str[i] == '\n') {
          ++c;
        }
      }
      if (countsMissingEndNewline && str.Length > 0 && str[str.Length - 1] != '\n') {
        ++c;
      }
      return c;
    }

    public static float GetAspect(this Vector2 vec) {
      return vec.x / vec.y;
    }

    public static Vector2 FitInto(this Vector2 sizeLimit, Vector2 innerSize, FitIntoMode mode) {
      Vector2 scalage = sizeLimit / innerSize;
      float adjust = mode == FitIntoMode.ShrinkToFit ? Mathf.Min(scalage.x, scalage.y) : Mathf.Min(scalage.x, scalage.y);
      return innerSize * adjust;
    }
    public static Vector2 FitInto(this Vector2 sizeLimit, float wantAspect, FitIntoMode mode) {
      return sizeLimit.FitInto(new Vector2(wantAspect, 1.0f), mode);
    }

    public static Rect FitInto(this Rect limitRect, float wantAspect, FitIntoMode mode) {
      Vector2 limitSize = limitRect.size;
      Vector2 fitSize = limitSize.FitInto(wantAspect, mode);
      Vector2 posOffset = (limitSize - fitSize) / 2.0f;
      return new Rect(limitRect.position + posOffset, fitSize);
    }

    public static Rect FitInto(this Rect limitRect, float wantAspect, FitIntoMode mode, TextAlignment textAlign) {
      Vector2 limitSize = limitRect.size;
      Vector2 fitSize = limitSize.FitInto(wantAspect, mode);
      Vector2 posOffset = (limitSize - fitSize) / 2.0f;
      if (textAlign == TextAlignment.Center)
        return new Rect(limitRect.position + posOffset, fitSize);
      else if (textAlign == TextAlignment.Left)
        return new Rect(limitRect.position + new Vector2(0.0f, posOffset.y), fitSize);
      else
        return new Rect(limitRect.position + new Vector2(posOffset.x + fitSize.x, posOffset.y), fitSize);
    }

    public static bool FloatApproxEq(float left, float right) {
      return Mathf.Abs(left - right) < 0.01f;
      ;
    }
  }
}