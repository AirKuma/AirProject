using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AirKuma {

  internal static class DrawingImpl {

    public static void FillRect(Rect rect, Color color) {
      bool alphaBlend = color.a != 1.0f;
      //if (color.a != 1.0f && color.a != 0.5f) {
      //  throw new System.Exception();
      //}
      var dummyTexture = new Texture2D(1, 1);
      UnityEngine.GUI.DrawTexture(rect, dummyTexture, ScaleMode.StretchToFill, alphaBlend, 0, color, 0, 0);
    }

    public static void StrokeLine(Vector2 p0, Vector2 p1, Color color, float lineWidth) {
      bool orthographic = p0.x == p1.x || p0.y == p1.y;
      if (orthographic) {
        if (p0.x == p1.x) { FillRect(new Rect(p0.x, p0.y, lineWidth, p1.y - p0.y), color); } else if (p0.y == p1.y) { FillRect(new Rect(p0.x, p0.y, p1.x - p0.x, lineWidth), color); }
      } else {
        Matrix4x4 matrixToRestore = UnityEngine.GUI.matrix;
        if (p0.x < p1.x) {
          float angle = Vector2.SignedAngle(Vector2.right, p1 - p0);
          GUIUtility.RotateAroundPivot(angle, p0);
          FillRect(new Rect(p0.x, p0.y, p1.x - p0.x, lineWidth), color);
        } else {
          float angle = Vector2.SignedAngle(Vector2.right, p0 - p1);
          GUIUtility.RotateAroundPivot(angle, p1);
          FillRect(new Rect(p1.x, p1.y, p0.x - p1.x, lineWidth), color);
        }
        UnityEngine.GUI.matrix = matrixToRestore;
      }
    }

    public static Vector2 MeasureText(string text, Font font) {
      int totalAdvance = 0;
      var cinfo = new CharacterInfo();
      foreach (char c in text) {
        font.GetCharacterInfo(c, out cinfo);
        if (cinfo.advance is 0)
          return new Vector2(font.lineHeight * 0.65f * text.Length, font.lineHeight);
        else
          totalAdvance += cinfo.advance;
      }
      return new Vector2((float)totalAdvance, font.lineHeight);
    }

    public static GUIStyle guiStyle = new GUIStyle();

    public static void DrawText(Rect layoutRect, string text, Font dynamicFont, Color textColor, TextAlignment textAlignment) {
      if (text.Length == 0)
        return;
      Vector2 size = MeasureText(text, dynamicFont);
      if (size.x == 0)
        return;
      //Rect drawnRect = layoutRect.GetFit2(size.GetAspect(), FitMode.Shrink);
      Rect drawnRect = layoutRect.FitInto(size.GetAspect(), FitIntoMode.ShrinkToFit, textAlignment);
      //Rect drawnRect = layoutRect;
      int nLines = text.CountNewlines(true);

      guiStyle.alignment = TextAnchor.UpperCenter; // why is TextAnchor.MiddleCenter make text somewhat lower?  (thus use UpperCenter currently
      guiStyle.font = dynamicFont;
      guiStyle.fontSize = (int)(drawnRect.size.y / nLines); // dynamic font required?
      guiStyle.normal.textColor = textColor;

      GUI.Label(drawnRect, text, guiStyle);
    }
  }

  class Imgui {

    public const int Lmb = 0;
    public const int Mmb = 2;
    public const int Rmb = 1;

    List<Rect> layoutRectStack = new List<Rect>();

    public Imgui() {

    }


    Rect CurrentLayoutRect {
      get => layoutRectStack[layoutRectStack.Count - 1];
      set => layoutRectStack[layoutRectStack.Count - 1] = value;
    }

    public void Reset(Vector2 totalSize) {
      layoutRectStack.Clear();
      layoutRectStack.Add(new Rect(0, 0, totalSize.x, totalSize.y));
    }

    public void SaveLayout(Rect newLayoutRect) {
      layoutRectStack.Add(newLayoutRect);
    }
    public void RestoreLayout() {
      layoutRectStack.RemoveAt(layoutRectStack.Count - 1);
    }

    public bool MouseButtonHits(int mouseButtonId) {

      return Event.current.type == EventType.MouseDown && Event.current.button == mouseButtonId
        && CurrentLayoutRect.Contains(Event.current.mousePosition);
    }

    public void Scale(Vector2 scaleRate) {
      CurrentLayoutRect = CurrentLayoutRect.CenteredScale(scaleRate);
    }
    public void Scale(float scaleRate) {
      this.Scale(new Vector2(scaleRate, scaleRate));
    }

    public void Solid(Color color) {
      DrawingImpl.FillRect(this.CurrentLayoutRect, color);
    }
    public void Border(Color color, float lineWidth) {
      var rect = CurrentLayoutRect;
      DrawingImpl.StrokeLine(rect.UpperLeft(), rect.UpperRight(), color, lineWidth);
      DrawingImpl.StrokeLine(rect.UpperRight(), rect.LowerRight(), color, lineWidth);
      DrawingImpl.StrokeLine(rect.LowerLeft(), rect.LowerRight() + new Vector2(lineWidth, 0f), color, lineWidth);
      DrawingImpl.StrokeLine(rect.UpperLeft() + new Vector2(0, lineWidth), rect.LowerLeft(), color, lineWidth);
    }
    // stretch to fit
    public void Image(Texture texture) {
      GUI.DrawTexture(this.CurrentLayoutRect, texture);
    }
    public void Image(Texture texture, FitIntoMode fitIntoMode) {
      this.SaveLayout(CurrentLayoutRect.FitInto(texture.width / texture.height, fitIntoMode));
      GUI.DrawTexture(this.CurrentLayoutRect, texture);
      this.RestoreLayout();
    }
    public void Text(string text, Color color) {
      DrawingImpl.DrawText(this.CurrentLayoutRect, text, SaveLoad.Data.defaultFont, color, TextAlignment.Center);
    }
  }
}
