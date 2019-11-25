using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AirKuma.Geom {

  public static class PlaneEx {
    public static Plane Ground {
      get {
        return new Plane(Vector3.up, Vector3.zero);
      }
    }
  }
}
