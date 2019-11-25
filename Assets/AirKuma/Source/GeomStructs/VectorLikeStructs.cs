using UnityEngine;

namespace AirKuma.Geom {

  public interface IGeomData2D<TScalar> {
    TScalar X { get; }
    TScalar Y { get; }

  }
  public interface IGeomData3D<TScalar> {
    TScalar X { get; }
    TScalar Y { get; }
    TScalar Z { get; }

  }
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  public readonly struct Factor3 : IGeomData3D<float> {

    public float X { get; }
    public float Y { get; }
    public float Z { get; }

    public Factor3(float x, float y, float z) : this() {
      X = x;
      Y = y;
      Z = z;
    }

    public static implicit operator Vector3(Factor3 self) {
      return new Vector3(self.X, self.Y, self.Z);
    }
    public static implicit operator Factor3(Vector3 vec) {
      return new Factor3(vec.x, vec.y, vec.z);
    }
  }
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  public readonly struct Direction3 : IGeomData3D<float> {

    public float X { get; }
    public float Y { get; }
    public float Z { get; }

    public Direction3(float x, float y, float z) : this() {
      X = x;
      Y = y;
      Z = z;
    }

    public static implicit operator Vector3(Direction3 self) {
      return new Vector3(self.X, self.Y, self.Z);
    }
    public static implicit operator Direction3(Vector3 vec) {
      return new Direction3(vec.x, vec.y, vec.z);
    }
  }
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  public readonly struct Point2 : IGeomData2D<float> {

    public float X { get; }
    public float Y { get; }

    public Point2(float x, float y) : this() {
      X = x;
      Y = y;
    }

    public static implicit operator Vector2(Point2 self) {
      return new Vector2(self.X, self.Y);
    }
    public static implicit operator Point2(Vector2 vec) {
      return new Point2(vec.x, vec.y);
    }
  }
  public readonly struct Point2Int : IGeomData2D<int> {

    public int X { get; }
    public int Y { get; }

    public Point2Int(int x, int y) : this() {
      X = x;
      Y = y;
    }

    public static implicit operator Vector2(Point2Int self) {
      return new Vector2(self.X, self.Y);
    }
    public static implicit operator Point2Int(Vector2Int vec) {
      return new Point2Int(vec.x, vec.y);
    }
    public override string ToString() {
      return $"point({X}, {Y})";
    }
  }
  public readonly struct Point3 : IGeomData3D<float> {

    public float X { get; }
    public float Y { get; }
    public float Z { get; }

    public Point3(float x, float y, float z) : this() {
      X = x;
      Y = y;
      Z = z;
    }

    public static implicit operator Vector3(Point3 self) {
      return new Vector3(self.X, self.Y, self.Z);
    }
    public static implicit operator Point3(Vector3 vec) {
      return new Point3(vec.x, vec.y, vec.z);
    }
  }
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
  public readonly struct Size2 : IGeomData2D<float> {

    public float X { get; }
    public float Y { get; }

    public Size2(float x, float y) : this() {
      X = x;
      Y = y;
    }

    public static implicit operator Vector2(Size2 self) {
      return new Vector2(self.X, self.Y);
    }
    public static implicit operator Size2(Vector2 vec) {
      return new Size2(vec.x, vec.y);
    }
  }
  public readonly struct Size2Int : IGeomData2D<int> {

    public int X { get; }
    public int Y { get; }

    public Size2Int(int x, int y) : this() {
      X = x;
      Y = y;
    }

    public static implicit operator Vector2(Size2Int self) {
      return new Vector2(self.X, self.Y);
    }
    public static implicit operator Size2Int(Vector2Int vec) {
      return new Size2Int(vec.x, vec.y);
    }
  }
  public readonly struct Size3 : IGeomData3D<float> {

    public float X { get; }
    public float Y { get; }
    public float Z { get; }

    public Size3(float x, float y, float z) : this() {
      X = x;
      Y = y;
      Z = z;
    }

    public static implicit operator Vector3(Size3 self) {
      return new Vector3(self.X, self.Y, self.Z);
    }
    public static implicit operator Size3(Vector3 vec) {
      return new Size3(vec.x, vec.y, vec.z);
    }
  }
  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}
