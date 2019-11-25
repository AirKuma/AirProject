using System.Collections.Generic;
using UnityEngine;

namespace AirKuma.Geom {
  public static class RectEx {

    public static IEnumerable<(Vector2, Vector2)> EachCornerPair(this Rect rect) {
      (Vector2 upperLeft, Vector2 upperRight, Vector2 lowerLeft, Vector2 lowerRight) = rect.GetCornerPositions();
      yield return (upperLeft, upperRight);
      yield return (upperLeft, lowerLeft);
      yield return (upperLeft, lowerRight);
      yield return (upperRight, lowerLeft);
      yield return (upperRight, lowerRight);
      yield return (lowerLeft, lowerRight);
    }
    public static IEnumerable<Vector2> EachCornerPositions(this Rect rect) {
      (Vector2 upperLeft, Vector2 upperRight, Vector2 lowerLeft, Vector2 lowerRight) = rect.GetCornerPositions();
      yield return upperLeft;
      yield return upperRight;
      yield return lowerLeft;
      yield return lowerRight;
    }

    #region Line side tests

    static LineSide GetLineSides(this Rect rect, Line line) {
      LineSide flags = LineSide.None;
      foreach (var pos in rect.EachCornerPositions()) {
        flags |= line.GetSide(pos);
      }
      return flags;
    }

    public static bool HasSecant(this Rect rect, Line line) {
      var flags = rect.GetLineSides(line);
      return (flags & LineSide.LeftSide) != LineSide.None
        && (flags & LineSide.RightSide) != LineSide.None;
    }
    // either one corner or two corners touch the line
    public static bool HasTangent(this Rect rect, Line line) {
      var flags = rect.GetLineSides(line);
      return !(flags.HasFlag(LineSide.LeftSide) && flags.HasFlag(LineSide.RightSide))
        && flags.HasFlag(LineSide.Colinear);
    }
    public static bool HasSecantOrTangent(this Rect rect, Line line) {
      var flags = rect.GetLineSides(line);
      return flags != LineSide.Colinear
        && flags != LineSide.LeftSide
        && flags != LineSide.RightSide;
    }

    #endregion

    public static (Vector2 upperLeft, Vector2 upperRight, Vector2 lowerLeft, Vector2 lowerRight) GetCornerPositions(this Rect rect) {
      float xw = rect.x + rect.width;
      float yh = rect.y + rect.height;
      return (rect.position,
        new Vector2(xw, rect.y),
        new Vector2(rect.x, yh),
        new Vector2(xw, yh));
    }

    public static Rect ByMinAndMax(Vector2 min, Vector2 max) {
      return new Rect(min, max - min);
    }

    //============================================================
    public static Rect GetSubFractionRect(this Rect rect, Rect fractionRect) {
      return new Rect(rect.position + rect.size * fractionRect.position,
        rect.size * fractionRect.size);
    }

    //============================================================
    public static Rect ThickPoint(this Vector2 pos, Vector2 thickness) {
      return new Rect(pos - thickness / 2.0f, thickness);
    }
    //============================================================

    public static Rect GetFit(this Rect limitRect, float wantAspect, FitMode mode) {
      Vector2 limitSize = limitRect.size;
      Vector2 fitSize = limitSize.GetFit(wantAspect, mode);
      Vector2 posOffset = (limitSize - fitSize) / 2.0f;
      return new Rect(limitRect.position + posOffset, fitSize);
    }
    public static Rect GetFit2(this Rect limitRect, float wantAspect, FitMode mode) {
      Vector2 limitSize = limitRect.size;
      Vector2 fitSize = limitSize.GetFit2(wantAspect, mode);
      Vector2 posOffset = (limitSize - fitSize) / 2.0f;
      return new Rect(limitRect.position + posOffset, fitSize);
    }
    public static Rect GetFit3(this Rect limitRect, float wantAspect, FitMode mode, TextAlignment textAlign) {
      Vector2 limitSize = limitRect.size;
      Vector2 fitSize = limitSize.GetFit2(wantAspect, mode);
      Vector2 posOffset = (limitSize - fitSize) / 2.0f;
      if (textAlign == TextAlignment.Center)
        return new Rect(limitRect.position + posOffset, fitSize);
      else if (textAlign == TextAlignment.Left)
        return new Rect(limitRect.position + new Vector2(0.0f, posOffset.y), fitSize);
      else
        return new Rect(limitRect.position + new Vector2(posOffset.x + fitSize.x, posOffset.y), fitSize);
    }

    public static Rect GetZoomed(this Rect self, Vector2 scalage) {
      Vector2 zoomedSize = self.size * scalage;
      Vector2 zoomedOffset = (self.size - zoomedSize) / 2.0f;
      return new Rect(self.position + zoomedOffset, zoomedSize);
    }
    public static Rect GetZoomedCentered(this Rect self, Vector2 center, Vector2 scalage) {
      Vector2 zoomedCenter = self.position + (center - self.position) * scalage;
      Vector2 zoomedSize = self.size * scalage;
      return new Rect(self.position - (zoomedCenter - center), zoomedSize);
    }
    public static Rect GetCenteredAt(this Rect self, Vector2 center) {
      self.position = center - (self.size / 2.0f);
      return self;
    }
    //============================================================
    public static Rect Unit() {
      return new Rect(Vector2.zero, Vector2.one);
    }

    //============================================================

    public static IEnumerable<Rect> EqualDiv(this Rect rect, Vector2Int divisions) {
      Vector2 offsetStep = rect.size / divisions;
      Vector2 divSize = offsetStep;
      for (int j = 0; j != divisions.y; ++j) {
        for (int i = 0; i != divisions.x; ++i) {
          yield return new Rect(rect.position + new Vector2(offsetStep.x * i, offsetStep.y * j), divSize);
        }
      }
    }
    //============================================================
    public static bool Includes(this Rect rect, Vector2 point) {
      return point.x >= rect.position.x
        && point.y >= rect.position.y
        && point.x <= rect.position.x + rect.width
        && point.y <= rect.position.y + rect.height;

    }
    public static bool Includes(this Rect self, Rect other) {
      return self.UpperLeft().EachAxisLargeThanOrEqualTo(other.UpperLeft())
        && self.LowerRight().EachAxisLargeThanOrEqualTo(other.LowerRight());
    }
    //============================================================
    public static Rect CutBy(this Rect area, Rect limit) {
      return RectEx.ByMinAndMax(area.UpperLeft().MaxWith(limit.UpperLeft()),
        area.LowerRight().MinWith(limit.LowerRight()));
    }
    //============================================================
    public static Rect ToRectWithSameSize(this Vector2 vec) {
      return new Rect(Vector2.zero, vec);
    }
    //============================================================
    public static Vector2 UpperLeft(this Rect self) {
      return self.position;
    }
    public static Vector2 UpperRight(this Rect self) {
      return new Vector2(self.position.x + self.width, self.position.y);
    }
    public static Vector2 LowerLeft(this Rect self) {
      return new Vector2(self.position.x, self.position.y + self.height);
    }
    public static Vector2 LowerRight(this Rect self) {
      return new Vector2(self.position.x + self.width, self.position.y + self.height);
    }
    //============================================================
    //public static void Shrink(this out Rect self, Vector2 scalage) {
    //  self = 
    //}
    public static Rect GetShrinked(this Rect self, Vector2 scalage) {
      Vector2 oldSize = self.size;
      Vector2 newSize = oldSize * scalage;
      self.position += (oldSize - newSize) / 2;
      self.size = newSize;
      return self;
    }
    public static Vector2 GetRelativeSubLocalPoint(this Rect self, Vector2 point, Rect frame) {
      Vector2 subOrigin = frame.position - self.position;
      Vector2 subExtent = self.size / frame.size;
      return (point - subOrigin) * subExtent;
    }
  }
}
