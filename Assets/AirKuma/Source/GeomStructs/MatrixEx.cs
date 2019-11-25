using UnityEngine;


namespace AirKuma.Geom {
  public static class MatrixEx {
    public static Matrix4x4 MakeCopy(this Matrix4x4 mtx) {
      var copied = new Matrix4x4();
      for (int i = 0; i != 4; ++i) {
        for (int j = 0; j != 4; ++j) {
          copied[i, j] = mtx[i, j];
        }
      }
      return copied;
    }
  }
}
