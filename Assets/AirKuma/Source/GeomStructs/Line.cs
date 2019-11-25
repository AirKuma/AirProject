using UnityEngine;
using System;

namespace AirKuma.Geom {

  [Flags]
  public enum LineSide {
    None = 0x0,
    Colinear = 0x1,
    LeftSide = 0x2,
    RightSide = 0x4,
  }
  public static class LineSideEx {
    public static LineSide GetOpposite(this LineSide side) {
      if (side == LineSide.LeftSide)
        return LineSide.RightSide;
      else if (side == LineSide.RightSide)
        return LineSide.LeftSide;
      return LineSide.Colinear;
    }
  }

  // direct line with infinite length
  // use UnityEngine.Ray for 3D Line
  public struct Line {
    public Vector2 start, end;

    public Line(Vector2 start, Vector2 end) {
      this.start = start;
      this.end = end;
    }
    public Line((Vector2 start, Vector2 end) line) {
      this.start = line.start;
      this.end = line.end;
    }

    public float Length => (end - start).magnitude;
    public float SqrtLength => (end - start).sqrMagnitude;
    public Vector2 MidPoint => (start + end) * .5f;

    public LineSide GetSide(Vector2 point) {
      var w = Vector2Ex.GetTriangleWindingOrder(start, end, point);
      if (w == TriangleWindingOrder.CounterClockwise)
        return LineSide.LeftSide;
      else if (w == TriangleWindingOrder.Clockwise)
        return LineSide.RightSide;
      else
        return LineSide.Colinear;
    }

    public bool CrossesWithSegment(Line lineSegment) {
      var startSide = this.GetSide(lineSegment.start);
      var stopSide = this.GetSide(lineSegment.end);
      return startSide != stopSide && startSide != LineSide.Colinear && stopSide != LineSide.Colinear;
    }
  }
}
