using System;
using UnityEngine;

namespace AirKuma.Geom {

  public enum Plane3D {
    XY, XZ, YZ
  }

  //         1 __ 2
  // Layout1: | /|
  //         0|/_|3
  //         1 __ 2
  // Layout2: |\ |
  //         0|_\|3
  public enum QuadTriangleLayout {
    Layout1, Layout2
  }

  public enum SwirlOrder {
    Inward,
    Outward
  }
  public enum RoundingOrder {
    Clockwise, CounterClockwise
  }

  public static class GeomUtils {

    //============================================================
    public static bool IsLocalIdentity(this Transform trf) {
      return trf.localPosition == Vector3.zero && trf.localScale == Vector3.one && trf.localRotation == Quaternion.identity;
    }
    public static void BeIdentity(this Transform trf) {
      trf.localPosition = Vector3.zero;
      trf.localScale = Vector3.one;
      trf.localRotation = Quaternion.identity;
    }
    public static bool IsZero(this Bounds bounds) {
      return bounds.center == Vector3.zero && bounds.size == Vector3.zero;
    }

    //============================================================

    public static Vector3 CoUnitVector3(this FaceSide face) {
      switch (face) {
        case FaceSide.Left:
          return Vector3.left;
        case FaceSide.Right:
          return Vector3.right;
        case FaceSide.Top:
          return Vector3.up;
        case FaceSide.Bottom:
          return Vector3.down;
        case FaceSide.Back:
          return Vector3.back;
        case FaceSide.Front:
          return Vector3.forward;
        default:
          throw new InvalidOperationException();
      }
    }
    public static Vector3Int CoUnitVector3Int(this FaceSide face) {
      switch (face) {
        case FaceSide.Left:
          return Vector3Int.left;
        case FaceSide.Right:
          return Vector3Int.right;
        case FaceSide.Top:
          return Vector3Int.up;
        case FaceSide.Bottom:
          return Vector3Int.down;
        case FaceSide.Back:
          return new Vector3Int(0, 0, -1);
        case FaceSide.Front:
          return new Vector3Int(0, 0, 1);
        default:
          throw new InvalidOperationException();
      }
    }

    public static Vector3 ToVector(this FaceSide face) {
      switch (face) {
        case FaceSide.Left:
          return Vector3.left;
        case FaceSide.Right:
          return Vector3.right;
        case FaceSide.Top:
          return Vector3.up;
        case FaceSide.Bottom:
          return Vector3.down;
        case FaceSide.Back:
          return Vector3.back;
        case FaceSide.Front:
          return Vector3.forward;
        default:
          throw new Exception();
      }
    }
    public static FaceSide GetWatchingFace(this Vector3 vec) {
      if (vec == Vector3.left)
        return FaceSide.Right;
      if (vec == Vector3.right)
        return FaceSide.Left;
      if (vec == Vector3.up)
        return FaceSide.Bottom;
      if (vec == Vector3.down)
        return FaceSide.Top;
      if (vec == Vector3.back)
        return FaceSide.Front;
      if (vec == Vector3.forward)
        return FaceSide.Back;
      throw new Exception();
    }
    public static FaceSide GetOrientedFace(this Vector3 vec) {
      if (vec == Vector3.left)
        return FaceSide.Left;
      if (vec == Vector3.right)
        return FaceSide.Right;
      if (vec == Vector3.up)
        return FaceSide.Top;
      if (vec == Vector3.down)
        return FaceSide.Bottom;
      if (vec == Vector3.back)
        return FaceSide.Back;
      if (vec == Vector3.forward)
        return FaceSide.Front;
      throw new Exception();
    }

    public static Vector3 GetDirectionVector(this Quaternion angle) {
      return angle * Vector3.forward;
    }

    public static Vector3 GetNearestOrthoUnitVector(this Vector3 direction) {
      Vector3 candidate = Vector3.zero;
      float minAngle = float.PositiveInfinity;
      foreach (Vector3 dir in VectorEx.AllOrthoUnitVectors) {
        float angle = Vector3.Angle(direction, dir);
        if (angle < minAngle) {
          candidate = dir;
          minAngle = angle;
        }
      }
      return candidate;
    }

    
    
  }
}
