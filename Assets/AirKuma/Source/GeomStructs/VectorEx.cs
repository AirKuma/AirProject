using System.Collections.Generic;
using UnityEngine;

namespace AirKuma.Geom {
  //############################################################
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#if KUMA
#pragma warning disable IDE1006 // Naming Styles
  public struct Vector2 {
    public float x, y;  

    public Vector2(float x, float y) {
      this.x = x; 
      this.y = y;
    }

    public static Vector2 zero => new Vector2(0, 0);
    public static Vector2 one => new Vector2(1, 1);

    public static Vector2 right => new Vector2(1, 0);
    public static Vector2 left => new Vector2(-1, 0);
    public static Vector2 up => new Vector2(0, 1);
    public static Vector2 down => new Vector2(0, -1);

    public static bool operator ==(Vector2 left, Vector2 right) {
      return left.x == right.x && left.y == right.y;
    }
    public static bool operator !=(Vector2 left, Vector2 right) {
      return left.x != right.x || left.y != right.y;
    }

    public static Vector2 operator -(Vector2 self) {
      return new Vector2(-self.x, -self.y);
    }

    public static Vector2 operator +(Vector2 left, Vector2 right) {
      return new Vector2(left.x + right.x, left.y + right.y);
    }
    public static Vector2 operator -(Vector2 left, Vector2 right) {
      return new Vector2(left.x - right.x, left.y - right.y);
    }
    public static Vector2 operator *(Vector2 self, float scalar) {
      return new Vector2(self.x * scalar, self.y * scalar);
    }
    public static Vector2 operator /(Vector2 self, float scalar) {
      return new Vector2(self.x / scalar, self.y / scalar);
    }

    public override bool Equals(object obj) {
      return obj is Vector2 vec && x == vec.x &&
             y == vec.y;
    }
    public override int GetHashCode() {
      int hashCode = 1502939027;
      hashCode = hashCode * -1521134295 + x.GetHashCode();
      hashCode = hashCode * -1521134295 + y.GetHashCode();
      return hashCode;
    }
  }
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  public struct Vector2Int {
    public int x, y;

    public Vector2Int(int x, int y) {
      this.x = x;
      this.y = y;
    }

    public static Vector2Int zero => new Vector2Int(0, 0);
    public static Vector2Int one => new Vector2Int(1, 1);

    public static Vector2Int right => new Vector2Int(1, 0);
    public static Vector2Int left => new Vector2Int(-1, 0);
    public static Vector2Int up => new Vector2Int(0, 1);
    public static Vector2Int down => new Vector2Int(0, -1);

    public static bool operator ==(Vector2Int left, Vector2Int right) {
      return left.x == right.x && left.y == right.y;
    }
    public static bool operator !=(Vector2Int left, Vector2Int right) {
      return left.x != right.x || left.y != right.y;
    }

    public static Vector2Int operator -(Vector2Int self) {
      return new Vector2Int(-self.x, -self.y);
    }

    public static Vector2Int operator +(Vector2Int left, Vector2Int right) {
      return new Vector2Int(left.x + right.x, left.y + right.y);
    }
    public static Vector2Int operator -(Vector2Int left, Vector2Int right) {
      return new Vector2Int(left.x - right.x, left.y - right.y);
    }
    public static Vector2Int operator *(Vector2Int self, int scalar) {
      return new Vector2Int(self.x * scalar, self.y * scalar);
    }
    public static Vector2Int operator /(Vector2Int self, int scalar) {
      return new Vector2Int(self.x / scalar, self.y / scalar);
    }

    public override bool Equals(object obj) {
      return obj is Vector2Int vec && x == vec.x &&
             y == vec.y;
    }

    public override int GetHashCode() {
      int hashCode = 1502939027;
      hashCode = hashCode * -1521134295 + x.GetHashCode();
      hashCode = hashCode * -1521134295 + y.GetHashCode();
      return hashCode;
    }
  }
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  public struct Vector3 {

    public float x, y, z;

    public Vector3(float x, float y, float z) {
      this.x = x;
      this.y = y;
      this.z = z;
    }

    public static Vector3 zero => new Vector3(0, 0, 0);
    public static Vector3 one => new Vector3(1, 1, 1);

    public static Vector3 right => new Vector3(1, 0, 0);
    public static Vector3 left => new Vector3(-1, 0, 0);
    public static Vector3 up => new Vector3(0, 1, 0);
    public static Vector3 down => new Vector3(0, -1, 0);
    public static Vector3 forward => new Vector3(0, 0, 1);
    public static Vector3 back => new Vector3(0, 0, -1);

    public static bool operator ==(Vector3 left, Vector3 right) {
      return left.x == right.x && left.y == right.y && left.z == right.z;
    }
    public static bool operator !=(Vector3 left, Vector3 right) {
      return left.x != right.x || left.y != right.y || left.z != right.z;
    }

    public static Vector3 operator -(Vector3 self) {
      return new Vector3(-self.x, -self.y, -self.z);
    }

    public static Vector3 operator +(Vector3 left, Vector3 right) {
      return new Vector3(left.x + right.x, left.y + right.y, left.z + right.z);
    }
    public static Vector3 operator -(Vector3 left, Vector3 right) {
      return new Vector3(left.x - right.x, left.y - right.y, left.z - right.z);
    }
    public static Vector3 operator *(Vector3 self, float scalar) {
      return new Vector3(self.x * scalar, self.y * scalar, self.z * scalar);
    }
    public static Vector3 operator /(Vector3 self, float scalar) {
      return new Vector3(self.x / scalar, self.y / scalar, self.z / scalar);
    }

    public override bool Equals(object obj) {
      return obj is Vector3 vec && x == vec.x &&
             y == vec.y && z == vec.z;
    }

    public override int GetHashCode() {
      int hashCode = 373119288;
      hashCode = hashCode * -1521134295 + x.GetHashCode();
      hashCode = hashCode * -1521134295 + y.GetHashCode();
      hashCode = hashCode * -1521134295 + z.GetHashCode();
      return hashCode;
    }
  }
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  public struct Vector3Int {
    public int x, y, z;

    public Vector3Int(int x, int y, int z) {
      this.x = x;
      this.y = y;
      this.z = z;
    }

    public static Vector3Int zero => new Vector3Int(0, 0, 0);
    public static Vector3Int one => new Vector3Int(1, 1, 1);

    public static Vector3Int right => new Vector3Int(1, 0, 0);
    public static Vector3Int left => new Vector3Int(-1, 0, 0);
    public static Vector3Int up => new Vector3Int(0, 1, 0);
    public static Vector3Int down => new Vector3Int(0, -1, 0);
    public static Vector3Int forward => new Vector3Int(0, 0, 1);
    public static Vector3Int back => new Vector3Int(0, 0, -1);

    public static bool operator ==(Vector3Int left, Vector3Int right) {
      return left.x == right.x && left.y == right.y && left.z == right.z;
    }
    public static bool operator !=(Vector3Int left, Vector3Int right) {
      return left.x != right.x || left.y != right.y || left.z != right.z;
    }

    public static Vector3Int operator -(Vector3Int self) {
      return new Vector3Int(-self.x, -self.y, -self.z);
    }

    public static Vector3Int operator +(Vector3Int left, Vector3Int right) {
      return new Vector3Int(left.x + right.x, left.y + right.y, left.z + right.z);
    }
    public static Vector3Int operator -(Vector3Int left, Vector3Int right) {
      return new Vector3Int(left.x - right.x, left.y - right.y, left.z - right.z);
    }
    public static Vector3Int operator *(Vector3Int self, int scalar) {
      return new Vector3Int(self.x * scalar, self.y * scalar, self.z * scalar);
    }
    public static Vector3Int operator /(Vector3Int self, int scalar) {
      return new Vector3Int(self.x / scalar, self.y / scalar, self.z / scalar);
    }

    public override bool Equals(object obj) {
      return obj is Vector3Int vec && x == vec.x &&
             y == vec.y && z == vec.z;
    }

    public override int GetHashCode() {
      int hashCode = 373119288;
      hashCode = hashCode * -1521134295 + x.GetHashCode();
      hashCode = hashCode * -1521134295 + y.GetHashCode();
      hashCode = hashCode * -1521134295 + z.GetHashCode();
      return hashCode;
    }
  }
  //############################################################
#pragma warning restore IDE1006 // Naming Styles
#endif

  public enum TriangleWindingOrder {
    Colinear,
    Clockwise,
    CounterClockwise
  }
  

  public static class Vector2Ex {

    public static float MagicCrossProduct(Vector2 v0, Vector2 v1) {
      return (v0.x * v1.y) - (v0.y * v1.x);
    }

    // direction (clockwise or counterclockwise or colinear) swapping between v0 and v2 with minimal angle (<= 180 degree)
    public static TriangleWindingOrder GetTriangleWindingOrder(Vector2 v0, Vector2 v1) {
      var v = MagicCrossProduct(v0, v1);
      if (v > 0) {
        return Geom.TriangleWindingOrder.CounterClockwise;
      }
      else if (v < 0) {
        return Geom.TriangleWindingOrder.Clockwise;
      }
      else
        return Geom.TriangleWindingOrder.Colinear;
    }

    public static TriangleWindingOrder GetTriangleWindingOrder(Vector2 p0, Vector2 p1, Vector2 p2) {
      return GetTriangleWindingOrder(p1 - p0, p2 - p0);
    }

    public static Vector2 Mid(Vector2 left, Vector2 right) {
      return (left + right) * .5f;
    }
    public static Vector2 Blend(Vector2 left, Vector2 right, float progress) {
      return (left * (1f - progress) + right * progress);
    }
  }
  public static class Vector3Ex {

    public static Vector3 PosHalf => new Vector3(+.5f, +.5f, +.5f);
    public static Vector3 NegHalf => new Vector3(-.5f, -.5f, -.5f);


    public static Vector3 Mid(Vector3 left, Vector3 right) {
      return (left + right) * .5f;
    }
    public static Vector3 Blend(Vector3 left, Vector3 right, float progress) {
      return (left * (1f - progress) + right * progress);
    }
  }

  public static class VectorEx {

    //============================================================

    public static Vector2 StackWith(this Vector2 self, Vector2 other, CrossDirection2D direction) {
      if (direction == CrossDirection2D.Horizontal) {
        return new Vector2(self.x + other.x, self.y.Max(other.y));
      }
      else {
        return new Vector2(self.x.Max(other.x), self.y + other.y);
      }
    }
    public static Vector3 StackWith(this Vector3 self, Vector3 other, CrossDirection3D direction) {
      if (direction == CrossDirection3D.Horizontal) {
        return new Vector3(self.x + other.x, self.y.Max(other.y), self.z.Max(other.z));
      }
      else if (direction == CrossDirection3D.Vertical) {
        return new Vector3(self.x.Max(other.x), self.y + other.y, self.z.Max(other.z));
      }
      else {
        return new Vector3(self.x.Max(other.x), self.y.Max(other.y), self.z + other.z);
      }
    }

    //============================================================
    public static Vector2 MaxWith(this Vector2 self, Vector2 other) {
      return new Vector2(self.x.Max(other.x), self.y.Max(other.y));
    }
    public static Vector2 MinWith(this Vector2 self, Vector2 other) {
      return new Vector2(self.x.Min(other.x), self.y.Min(other.y));
    }
    //------------------------------------------------------------
    public static Vector3 MaxWith(this Vector3 self, Vector3 other) {
      return new Vector3(self.x.Max(other.x), self.y.Max(other.y), self.z.Max(other.z));
    }
    public static Vector3 MinWith(this Vector3 self, Vector3 other) {
      return new Vector3(self.x.Min(other.x), self.y.Min(other.y), self.z.Min(other.z));
    }

    //============================================================

    public static float MaxCell(this Vector2 self) {
      return self.x > self.y ? self.x : self.y;
    }
    public static float MinCell(this Vector2 self) {
      return self.x < self.y ? self.x : self.y;
    }
    //------------------------------------------------------------
    public static float MaxCell(this Vector3 self) {
      return self.x > self.y ? self.x : (self.y > self.z ? self.y : self.z);
    }
    public static float MinCell(this Vector3 self) {
      return self.x < self.y ? self.x : (self.y < self.z ? self.y : self.z);
    }
    //============================================================
    public static bool AnyAxisLessThan(this Vector2 self, Vector2 other) {
      return self.x < other.x || self.y < other.y;
    }
    public static bool AnyAxisLessThanOrEqualTo(this Vector2 self, Vector2 other) {
      return self.x <= other.x || self.y <= other.y;
    }
    public static bool AnyAxisLargeThan(this Vector2 self, Vector2 other) {
      return self.x > other.x || self.y > other.y;
    }
    public static bool AnyAxisLargeThanOrEqualTo(this Vector2 self, Vector2 other) {
      return self.x >= other.x || self.y >= other.y;
    }
    //------------------------------------------------------------
    public static bool EachAxisLessThan(this Vector2 self, Vector2 other) {
      return self.x < other.x && self.y < other.y;
    }
    public static bool EachAxisLessThanOrEqualTo(this Vector2 self, Vector2 other) {
      return self.x <= other.x && self.y <= other.y;
    }
    public static bool EachAxisLargeThan(this Vector2 self, Vector2 other) {
      return self.x > other.x && self.y > other.y;
    }
    public static bool EachAxisLargeThanOrEqualTo(this Vector2 self, Vector2 other) {
      return self.x >= other.x && self.y >= other.y;
    }
    //------------------------------------------------------------
    public static bool AnyAxisLessThan(this Vector3 self, Vector3 other) {
      return self.x < other.x || self.y < other.y || self.z < other.z;
    }
    public static bool AnyAxisLessThanOrEqualTo(this Vector3 self, Vector3 other) {
      return self.x <= other.x || self.y <= other.y || self.z <= other.z;
    }
    public static bool AnyAxisLargeThan(this Vector3 self, Vector3 other) {
      return self.x > other.x || self.y > other.y || self.z > other.z;
    }
    public static bool AnyAxisLargeThanOrEqualTo(this Vector3 self, Vector3 other) {
      return self.x >= other.x || self.y >= other.y || self.z >= other.z;
    }
    //------------------------------------------------------------
    public static bool EachAxisLessThan(this Vector3 self, Vector3 other) {
      return self.x < other.x && self.y < other.y && self.z < other.z;
    }
    public static bool EachAxisLessThanOrEqualTo(this Vector3 self, Vector3 other) {
      return self.x <= other.x && self.y <= other.y && self.z <= other.z;
    }
    public static bool EachAxisLargeThan(this Vector3 self, Vector3 other) {
      return self.x > other.x && self.y > other.y && self.z > other.z;
    }
    public static bool EachAxisLargeThanOrEqualTo(this Vector3 self, Vector3 other) {
      return self.x >= other.x && self.y >= other.y && self.z >= other.z;
    }
    //============================================================
    public static Vector2 ToVector2(this float value) {
      return new Vector2(value, value);
    }
    public static Vector3 ToVector3(this float value) {
      return new Vector3(value, value, value);
    }
    public static Vector2 ToVector2With(this float firstValue, Axis2D firsValueAxis, float secondValue) {
      if (firsValueAxis == Axis2D.X)
        return new Vector2(firstValue, secondValue);
      else
        return new Vector2(secondValue, firstValue);
    }
    public static Vector2 ToVector2WithZero(this float value, Axis2D axis) {
      if (axis == Axis2D.X)
        return new Vector2(value, 0.0f);
      else
        return new Vector2(0.0f, value);
    }
    public static Vector2 ToVector2WithOne(this float value, Axis2D axis) {
      if (axis == Axis2D.X)
        return new Vector2(value, 1.0f);
      else
        return new Vector2(1.0f, value);
    }
    //============================================================
    public static float GetAxis(this Vector2 self, Axis2D axis) {
      return axis == Axis2D.X ? self.x : self.y;
    }
    public static float GetAxis(this Vector3 self, Axis3D axis) {
      return axis == Axis3D.X ? self.x : (axis == Axis3D.Y ? self.y : self.z);
    }
    public static int GetAxis(this Vector2Int self, Axis2D axis) {
      return axis == Axis2D.X ? self.x : self.y;
    }
    public static int GetAxis(this Vector3Int self, Axis3D axis) {
      return axis == Axis3D.X ? self.x : (axis == Axis3D.Y ? self.y : self.z);
    }
    //------------------------------------------------------------
    public static Vector2 ZeroExcept(this Vector2 self, Axis2D axis) {
      return axis == Axis2D.X ? new Vector2(self.x, 0.0f) : new Vector2(0.0f, self.y);
    }
    public static Vector3 ZeroExcept(this Vector3 self, Axis3D axis) {
      return axis == Axis3D.X ? new Vector3(self.x, 0.0f, 0.0f) :
        (axis == Axis3D.Y ? new Vector3(0.0f, self.y, 0.0f) : new Vector3(0.0f, 0.0f, self.z));
    }
    public static Vector3Int ZeroExcept(this Vector3Int self, Axis3D axis) {
      return axis == Axis3D.X ? new Vector3Int(self.x, 0, 0) :
        (axis == Axis3D.Y ? new Vector3Int(0, self.y, 0) : new Vector3Int(0, 0, self.z));
    }
    //------------------------------------------------------------
    public static Vector2 OneExcept(this Vector2 self, Axis2D axis) {
      return axis == Axis2D.X ? new Vector2(self.x, 1.0f) : new Vector2(1.0f, self.y);
    }
    public static Vector2 OneExcept(this Vector3 self, Axis2D axis) {
      return axis == Axis2D.X ? new Vector3(self.x, 1.0f, 1.0f) :
        (axis == Axis2D.Y ? new Vector3(1.0f, self.y, 1.0f) : new Vector3(1.0f, 1.0f, self.z));
    }
    //============================================================
    public static Vector2 MixWith(this Vector2 self, Axis2D axis, Vector2 other) {
      if ((axis & Axis2D.X) != 0) {
        self.x = other.x;
      }
      if ((axis & Axis2D.Y) != 0) {
        self.y = other.y;
      }
      return self;
    }
    public static Vector3 MixWith(this Vector3 self, Axis3D axis, Vector3 other) {
      if ((axis & Axis3D.X) != 0) {
        self.x = other.x;
      }
      if ((axis & Axis3D.Y) != 0) {
        self.y = other.y;
      }
      if ((axis & Axis3D.Z) != 0) {
        self.z = other.z;
      }
      return self;
    }
    //============================================================
    public static float GetAspect(this Vector2 vec) {
      return vec.x / vec.y;
    }

    public static Vector2 GetFit(this Vector2 sizeLimit, float wantAspect, FitMode mode) {
      float d1 = sizeLimit.x / wantAspect, d2 = sizeLimit.y;
      float adjust = mode == FitMode.Extented ? d1.Max(d2) : d1.Min(d2);
      bool toAdjustX = d2 == adjust;
      return toAdjustX ? new Vector2(wantAspect * adjust, sizeLimit.y) : new Vector2(sizeLimit.x, adjust);
    }

    public static Vector2 GetFit2(this Vector2 sizeLimit, Vector2 innerSize, FitMode mode) {
      Vector2 scalage = sizeLimit / innerSize;
      float adjust = mode == FitMode.Shrink ? scalage.x.Min(scalage.y) : scalage.x.Max(scalage.y);
      return innerSize * adjust;
    }
    public static Vector2 GetFit2(this Vector2 sizeLimit, float wantAspect, FitMode mode) {
      return sizeLimit.GetFit2(new Vector2(wantAspect, 1.0f), mode);
    }

    //============================================================
    public static bool WithinGUISpace(this Vector2 point) {
      return (point.x >= 0.0f && point.x <= 1.0f
        && point.y >= 0.0f && point.y <= 1.0f);
    }
    //============================================================
    public static int GetOffset(this Ward1D ward) {
      return ward == Ward1D.Forward ? 1 : -1;
    }
    public static Vector2 GetOffset(this Ward2D ward) {
      return new Vector2(
        (ward & Ward2D.Rightward).Flagged() ? 1 : -1,
        (ward & Ward2D.Upward).Flagged() ? 1 : -1
      );
    }
    public static Vector3 GetOffset(this Ward3D ward) {
      return new Vector3((ward & Ward3D.Rightward).Flagged() ? 1 : -1,
        (ward & Ward3D.Upward).Flagged() ? 1 : -1,
        (ward & Ward3D.Forward).Flagged() ? 1 : -1);
    }
    //============================================================
    public static Vector2 Half(this Vector2 point) {
      return new Vector2(point.x * 0.5f, point.y * 0.5f);
    }
    public static Vector3 Half(this Vector3 point) {
      return new Vector3(point.x * 0.5f, point.y * 0.5f, point.z * 0.5f);
    }
    public static Vector2 Twice(this Vector2 point) {
      return new Vector2(point.x * 2.0f, point.y * 2.0f);
    }
    public static Vector3 Twice(this Vector3 point) {
      return new Vector3(point.x * 2.0f, point.y * 2.0f, point.z * 2.0f);
    }
    //============================================================
    public static Vector3 Round(this Vector3 point) {
      return new Vector3((point.x).Round(), (point.y).Round(), (point.z).Round());
    }
    public static Vector3Int RoundToInt(this Vector3 point) {
      return new Vector3Int((point.x).RoundToInt(), (point.y).RoundToInt(), (point.z).RoundToInt());
    }
    //============================================================
    public static Vector2 Floor(this Vector2 vec) {
      return new Vector2((vec.x).Floor(), (vec.y).Floor());
    }
    public static Vector3 Floor(this Vector3 point) {
      return new Vector3((point.x).Floor(), (point.y).Floor(), (point.z).Floor());
    }
    public static Vector2Int FloorToInt(this Vector2 vec) {
      return new Vector2Int((vec.x).FloorToInt(), (vec.y).FloorToInt());
    }
    public static Vector3Int FloorToInt(this Vector3 point) {
      return new Vector3Int((point.x).FloorToInt(), (point.y).FloorToInt(), (point.z).FloorToInt());
    }
    //public static Vector3Int RoundToInt(this Vector3 point) {
    //  return new Vector3Int((point.x).FloorToInt(), (point.y).FloorToInt(), (point.z).FloorToInt());
    //}
    //------------------------------------------------------------
    public static Vector2 Ceil(this Vector2 vec) {
      return new Vector2((vec.x).Ceil(), (vec.y).Ceil());
    }
    public static Vector3 Ceil(this Vector3 point) {
      return new Vector3((point.x).Ceil(), point.y.Ceil(), point.z.Ceil());
    }
    public static Vector3Int CeilToInt(this Vector3 point) {
      return new Vector3Int((point.x).CeilToInt(), (point.y).CeilToInt(), (point.z).CeilToInt());
    }
    public static Vector3 HalfFloor(this Vector3 point) {
      return point.Floor() + new Vector3(0.5f, 0.5f, 0.5f);
    }
    public static Vector3 ElevationHalfFloor(this Vector3 point) {
      return new Vector3(point.x.HalfFloor(), point.y + 0.5f, point.z.HalfFloor());
    }
    //------------------------------------------------------------
    public static Vector2Int CeilToInt(this Vector2 vec) {
      return new Vector2Int((vec.x).CeilToInt(), (vec.y).CeilToInt());
    }
    public static Vector2Int ToIntVector2WithGreaterOrEqualProduct(this Vector2 vec) {
      float originalProduct = (vec.x * vec.y);
      Vector2Int vec0 = vec.FloorToInt();
      if (vec0.Product() >= originalProduct)
        return vec0;
      var vec1 = new Vector2Int(vec.x.CeilToInt(), vec.y.FloorToInt());
      var vec2 = new Vector2Int(vec.x.FloorToInt(), vec.y.CeilToInt());
      if (vec1.Product() >= originalProduct && vec2.Product() >= originalProduct)
        if (vec1.Product() <= vec2.Product())
          return vec1;
        else
          return vec2;
      else if (vec1.Product() >= originalProduct) {
        return vec1;
      }
      else if (vec2.Product() >= originalProduct) {
        return vec2;
      }
      return vec.CeilToInt();
    }
    //============================================================
    public static int Product(this Vector2Int vec) {
      return vec.x * vec.y;
    }
    //============================================================
    public static bool ApproxEqualTo(this Vector2 self, Vector2 other, float tolerance = 0.001f) {
      return self.x.ApproxEqualTo(other.x, tolerance)
        && self.y.ApproxEqualTo(other.y, tolerance);
    }
    public static bool ApproxEqualTo(this Vector3 self, Vector3 other, float tolerance = 0.001f) {
      return self.x.ApproxEqualTo(other.x, tolerance)
        && self.y.ApproxEqualTo(other.y, tolerance)
        && self.z.ApproxEqualTo(other.z, tolerance);
    }
    //============================================================
    public static Vector2Int ToVector2Int(this Vector2 vec) {
      return new Vector2Int((int)vec.x, (int)vec.y);
    }
    public static Vector2 ToVector2(this Vector2Int vec) {
      return new Vector2(vec.x, vec.y);
    }
    public static Vector3Int ToVector2Int(this Vector3 vec) {
      return new Vector3Int((int)vec.x, (int)vec.y, (int)vec.z);
    }
    public static Vector3 ToVector3(this Vector3Int vec) {
      return new Vector3(vec.x, vec.y, vec.z);
    }
    //============================================================
    public static float GetSqrtLength(this Vector2 vec) {
      return vec.x.Pow2() + vec.y.Pow2();
    }
    public static float GetLength(this Vector2 vec) {
      return vec.GetSqrtLength().Sqrt();
    }
    //------------------------------------------------------------
    public static float GetSqrtLength(this Vector3 vec) {
      return vec.x.Pow2() + vec.y.Pow2() + vec.z.Pow2();
    }
    public static float GetLength(this Vector3 vec) {
      return vec.GetSqrtLength().Sqrt();
    }
    //============================================================
    public static float GetDistance(this Vector3 left, Vector3 right) {
      return (right - left).GetLength();
    }
    public static float GetSqrtDistance(this Vector3 left, Vector3 right) {
      return (right - left).GetSqrtLength();
    }
    //============================================================
    public static Vector2 GetNormalized(this Vector2 vec) {
      float l = vec.GetLength();
      return vec / l;
    }
    public static void Normalize(this Vector2 vec) {
      float l = vec.GetLength();
      vec /= l;
    }
    //============================================================
    public static bool IsFraction(this Vector2 vec) {
      return vec.x.IsFraction() && vec.y.IsFraction();
    }
    public static bool IsInNormalizedSpace(this Vector2 vec) {
      return vec.IsFraction();
    }
    //============================================================
    public static Vector2 Clamp(this Vector2 v, Vector2 min, Vector2 max) {
      return new Vector2(
        v.x.Clamp(min.x, max.x),
        v.y.Clamp(min.y, max.y)
        );
    }
    public static Vector3 Clamp(this Vector3 v, Vector3 min, Vector3 max) {
      return new Vector3(
        v.x.Clamp(min.x, max.x),
        v.y.Clamp(min.y, max.y),
        v.z.Clamp(min.z, max.z)
        );
    }
    public static Vector2 ClampLength(this Vector2 self, float minLength, float maxLength) {
      float l = self.GetLength();
      if (l < minLength) {
        return self * (minLength / l);
      }
      else if (l > maxLength) {
        return self * (maxLength / l);
      }
      return self;
    }
    //============================================================
    public static Vector2 CoeffiAdd(this Vector2 left, Vector2 right) {
      return left + right;
    }
    public static Vector3 CoeffiAdd(this Vector3 left, Vector3 right) {
      return left + right;
    }
    public static Vector2 CoeffiSub(this Vector2 left, Vector2 right) {
      return left - right;
    }
    public static Vector3 CoeffiSub(this Vector3 left, Vector3 right) {
      return left - right;
    }
    //------------------------------------------------------------
    public static Vector2 CoeffiMul(this Vector2 left, Vector2 right) {
      return left * right;
    }
    public static Vector3 CoeffiMul(this Vector3 left, Vector3 right) {
      return Vector3.Scale(left, right);
    }
    public static Vector2 CoeffiDiv(this Vector2 self, Vector2 other) {
      return self / other;
    }
    public static Vector3 CoeffiDiv(this Vector3 self, Vector3 other) {
      return new Vector3(self.x / other.x, self.y / other.y, self.z / other.z);
    }
    //------------------------------------------------------------
    public static float DotProduct(this Vector2 left, Vector2 right) {
      return left.x * right.x + left.y * right.y;
    }
    //------------------------------------------------------------
    public static Vector2 UniformAdd(this Vector2 vec, float scalar) {
      return new Vector2(vec.x + scalar, vec.y + scalar);
    }
    public static Vector2 UniformSub(this Vector2 vec, float scalar) {
      return new Vector2(vec.x - scalar, vec.y - scalar);
    }
    public static Vector2 UniformMul(this Vector2 vec, float scalar) {
      return vec * scalar;
    }
    public static Vector2 UniformDiv(this Vector2 vec, float scalar) {
      return vec / scalar;
    }
    //------------------------------------------------------------
    public static Vector3 UniformAdd(this Vector3 vec, float scalar) {
      return new Vector3(vec.x + scalar, vec.y + scalar, vec.z + scalar);
    }
    public static Vector3 UniformSub(this Vector3 vec, float scalar) {
      return new Vector3(vec.x - scalar, vec.y - scalar, vec.z - scalar);
    }
    public static Vector3 UniformMul(this Vector3 vec, float scalar) {
      return vec * scalar;
    }
    public static Vector3 UniformDiv(this Vector3 vec, float scalar) {
      return vec / scalar;
    }
    //============================================================
    public static bool Includes(this Vector2 size, Vector2 point) {
      return point.x >= 0.0f && point.x <= size.x
          && point.y >= 0.0f && point.y <= size.y;
    }
    //============================================================
    public static Vector2 Swap(this Vector2 vec, Axis2D axis) {
      return axis == Axis2D.X ? vec : new Vector2(vec.y, vec.x);
    }
    public static Vector2 Swap(this Vector2Int vec, Axis2D axis) {
      return axis == Axis2D.X ? vec : new Vector2Int(vec.y, vec.x);
    }
    //============================================================
    public static Vector2 GetPaddingOffset(this Vector2 inner, Vector2 outer) {
      return (outer - inner) / 2.0f;
    }
    //============================================================
    public static Vector2 ToVector2((float x, float y) t) {
      return new Vector2(t.x, t.y);
    }
    public static Vector3 ToVector3((float x, float y, float z) t) {
      return new Vector3(t.x, t.y, t.z);
    }
    //============================================================
    public static Vector3 FixUp(this Vector3 point) {
      return point + new Vector3(0.0f, 0.001f, 0.0f);
    }
    public static Vector3 CeilY(this Vector3 point) {
      return new Vector3(point.x, point.y.Ceil(), point.z);
    }
    public static Vector2 XZ(this Vector3 v) {
      return new Vector2(v.x, v.y);
    }
    //============================================================
    public static Vector3Int Half(this Vector3Int vec) {
      return new Vector3Int(vec.x / 2, vec.y / 2, vec.z / 2);
    }

    //============================================================
    public static Vector2 GetOneIfZero(this Vector2 vec) {
      if (vec == Vector2.zero)
        return Vector2.one;
      return vec;
    }
    //============================================================
    public static int CountZeros(this Vector3 vec) {
      int count = 0;
      if (vec.x == 0) { ++count; }
      if (vec.y == 0) { ++count; }
      if (vec.z == 0) { ++count; }
      return count;
    }
    public static int CountNonZeros(this Vector3Int vec) {
      int count = 0;
      if (vec.x != 0) { ++count; }
      if (vec.y != 0) { ++count; }
      if (vec.z != 0) { ++count; }
      return count;
    }
    public static IEnumerable<Vector3Int> EachWithOneAxisZero(this Vector3Int vec) {
      yield return new Vector3Int(0, vec.y, vec.z);
      yield return new Vector3Int(vec.x, 0, vec.z);
      yield return new Vector3Int(vec.x, vec.y, 0);
    }
    public static IEnumerable<Axis3D> EachNonZeroAxis(this Vector3Int vec) {
      if (vec.x != 0)
        yield return Axis3D.X;
      if (vec.y != 0)
        yield return Axis3D.Y;
      if (vec.z != 0)
        yield return Axis3D.Z;
    }
    //============================================================
    public static IEnumerable<Vector3> AllOrthoUnitVectors {
      get {
        yield return Vector3.left;
        yield return Vector3.right;
        yield return Vector3.down;
        yield return Vector3.up;
        yield return Vector3.back;
        yield return Vector3.forward;
      }
    }
    public static IEnumerable<Vector3Int> AllOrthoUnitVectorInts {
      get {
        yield return Vector3Int.left;
        yield return Vector3Int.right;
        yield return Vector3Int.down;
        yield return Vector3Int.up;
        yield return new Vector3Int(0, 0, -1);
        yield return new Vector3Int(0, 0, 1);
      }
    }
    public static IEnumerable<Vector3Int> AllBoundaryVectorInts {
      get {
        yield return Vector3Int.left;
        yield return Vector3Int.right;
        yield return Vector3Int.down;
        yield return Vector3Int.up;
        yield return new Vector3Int(0, 0, -1);
        yield return new Vector3Int(0, 0, 1);

        yield return new Vector3Int(0, -1, -1);
        yield return new Vector3Int(0, -1, +1);
        yield return new Vector3Int(0, +1, -1);
        yield return new Vector3Int(0, +1, +1);

        yield return new Vector3Int(-1, 0, -1);
        yield return new Vector3Int(-1, 0, +1);
        yield return new Vector3Int(+1, 0, -1);
        yield return new Vector3Int(+1, 0, +1);

        yield return new Vector3Int(-1, -1, 0);
        yield return new Vector3Int(-1, +1, 0);
        yield return new Vector3Int(+1, -1, 0);
        yield return new Vector3Int(+1, +1, 0);

        yield return new Vector3Int(-1, -1, -1);
        yield return new Vector3Int(-1, -1, +1);
        yield return new Vector3Int(-1, +1, -1);
        yield return new Vector3Int(-1, +1, +1);
        yield return new Vector3Int(+1, -1, -1);
        yield return new Vector3Int(+1, -1, +1);
        yield return new Vector3Int(+1, +1, -1);
        yield return new Vector3Int(+1, +1, +1);
      }
    }
  } 
}
