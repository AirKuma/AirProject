using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AirKuma.Geom {

  public static class BoundsIntEx {
    public static BoundsInt NewBySize(this Vector3Int origin, Vector3Int size) {
      return new BoundsInt(origin + size.Half(), size);
    }
    public static BoundsInt NewByExtent(this Vector3Int center, Vector3Int extent) {
      return new BoundsInt(center, extent * 2);
    }
    public static BoundsInt NewByMinAndMax(this Vector3Int min, Vector3Int max) {
      return new BoundsInt(min + (max - min).Half(), max - min);
    }
    public static BoundsInt FromBounds(Bounds bounds) {
      return NewByMinAndMax(bounds.min.FloorToInt(), bounds.max.CeilToInt());
    }
  }
  public static class BoundsEx {

    public static float GetVolume(this Bounds bounds) {
      return bounds.size.x * bounds.size.y * bounds.size.z;
    }

    public static IEnumerable<(Axis3D axis, NegOrPos dir)> Exposure(this Bounds extreme, Bounds limit) {
      foreach (Axis3D axis in EnumIteration.IterateEnumVals<Axis3D>()) {
        if (extreme.min.GetAxis(axis) < limit.min.GetAxis(axis))
          yield return (axis, NegOrPos.Neg);
        if (extreme.max.GetAxis(axis) > limit.max.GetAxis(axis))
          yield return (axis, NegOrPos.Pos);
      }
    }
    public static FaceSide ExposingFaces(this Bounds extreme, Bounds @base) {
      FaceSide faces = FaceSide.None;
      foreach ((Axis3D axis, NegOrPos dir) in extreme.Exposure(@base)) {
        if (axis == Axis3D.X)
          faces |= dir == NegOrPos.Neg ? FaceSide.Left : FaceSide.Right;
        else if (axis == Axis3D.Y)
          faces |= dir == NegOrPos.Neg ? FaceSide.Bottom : FaceSide.Top;
        else
          faces |= dir == NegOrPos.Neg ? FaceSide.Back : FaceSide.Front;
      }
      return faces;
    }

    public static Bounds NewBySize(this Vector3 origin, Vector3 size) {
      return new Bounds(origin + size * 0.5f, size);
    }
    public static Bounds NewByExtent(this Vector3 center, Vector3 extent) {
      return new Bounds(center, extent * 2);
    }
    public static Bounds NewByMinAndMax(this Vector3 min, Vector3 max) {
      return new Bounds(min + (max - min) / 2.0f, max - min);
    }

    //============================================================
    // hit test with all active colliders in the active scene

    //public static Collider[] GlobalHitTest(this Bounds self) => self.GlobalHitTest(LayerMaskProxy.AllLayer);

    //public static Collider[] GlobalHitTest(this Bounds self, LayerMaskProxy mask) {
    //  return Physics.OverlapBox(self.center, self.extents, Quaternion.identity, (int)mask, QueryTriggerInteraction.Collide);
    //}

    //============================================================

    public static Bounds TransformBounds(this Transform trf, Bounds bounds) {
      return new Bounds(trf.TransformPoint(bounds.center), trf.TransformVector(bounds.size));
    }
    public static Bounds GetGlobalBounds(this Collider collider) {
      return collider.bounds;
    }
    //============================================================

    public static Bounds SnapOutward(this Bounds bounds, float outwardOffset = 0.0f) {
      return BoundsEx.NewByMinAndMax(
        min: (bounds.min - outwardOffset.ToVector3()).Floor(),
        max: (bounds.max + outwardOffset.ToVector3()).Ceil()
      );
    }
    public static Bounds SnapInward(this Bounds bounds, float inwardOffset = 0.0f) {
      return BoundsEx.NewByMinAndMax(
        min: (bounds.min + inwardOffset.ToVector3()).Ceil(),
        max: (bounds.max - inwardOffset.ToVector3()).Floor()
      );
    }



    //============================================================
    public static Bounds IntersectBounds(this Bounds left, Bounds right) {
      return NewByMinAndMax(
       min: right.min.Clamp(left.min, left.max),
       max: right.max.Clamp(left.min, left.max)
     );
    }
    //============================================================
    public static Bounds Extend(this Bounds bounds, float offset) {
      return bounds.Extend(offset.ToVector3());
    }
    public static Bounds Extend(this Bounds bounds, Vector3 offset) {
      return BoundsEx.NewByExtent(bounds.center, bounds.extents + offset);
    }
    //------------------------------------------------------------
    public static Bounds ScaleExtent(this Bounds bounds, Vector3 scalage) {
      return BoundsEx.NewByExtent(bounds.center, Vector3.Scale(bounds.extents, scalage));
    }
    public static Bounds ScaleExtent(this Bounds bounds, float uniformScalage) {
      return bounds.ScaleExtent(uniformScalage.ToVector3());
    }
    //============================================================
    public static bool Includes(this Bounds self, Bounds other) {
      return other.min.EachAxisLargeThanOrEqualTo(self.min) && other.max.EachAxisLessThanOrEqualTo(self.max);
    }
    public static bool Includes(this Bounds bounds, Vector3 point) {
      return bounds.Contains(point);
    }
    //============================================================
    public static Bounds GetButtonBounds(this Bounds bounds) {
      return BoundsEx.NewByMinAndMax(bounds.min, new Vector3(bounds.max.x, bounds.min.y, bounds.max.z));
    }
    public static Bounds GetTopBounds(this Bounds bounds) {
      return BoundsEx.NewByMinAndMax(new Vector3(bounds.min.x, bounds.max.y, bounds.min.z), bounds.max);
    }
    //------------------------------------------------------------
    public static Plane GetButtonPlane(this Bounds bounds) {
      return new Plane(Vector3.up, bounds.min);
    }
    public static Plane GetTopPlane(this Bounds bounds) {
      return new Plane(Vector3.up, bounds.max);
    }
    //============================================================
    public static Bounds WithThickness(this Bounds bounds, float thickness = 0.01f) {
      return BoundsEx.NewByMinAndMax(bounds.min, new Vector3(bounds.max.x, bounds.max.y + thickness, bounds.max.z));
    }
    //============================================================
  }
}
