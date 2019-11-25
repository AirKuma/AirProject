using System;
using UnityEngine;
using AirKuma.Geom;

namespace AirKuma {

  public static class CameraEx {

    //    public static void ZoomAlongRay(Kuma.Viewport view, Ray ray, float distance) {
    //      view.Position += ray.direction * distance;
    //      //view.Look(ray.direction * distance, Space.World);
    //    }

    public static Vector2 GetResolution(this Camera camera) {
      // todo: should it use scaledPixel...
      return new Vector2(camera.pixelWidth, camera.pixelHeight);
    }
    //============================================================
    public static float PerspectiveFrustumHeight(this Camera camera, float distanceFromCamera) {
      //  return distanceFromCamera * camera.PerspectiveHalfViewAngle() * 2.0f;
      return distanceFromCamera * Mathf.Tan(camera.PerspectiveHalfViewAngle()) * 2.0f;
    }
    public static float PerspectiveDistanceFromCamera(this Camera camera, float frustumHeight) {
      return (frustumHeight * 0.5f) / camera.PerspectiveHalfViewAngle().Tan();
    }
    public static float PerspectiveHalfViewAngle(this Camera camera) {
      return (camera.fieldOfView * Mathf.Deg2Rad) * 0.5f;
    }
    //============================================================
    public static readonly Quaternion IsometricViewAngle = Quaternion.Euler(35.264f, 45.0f, 0.0f);
    public const float VerticalLengthFactor = FloatEx.Sqrt3;
    // uses the rotation x as down angle
    public static float DownAngle(this Camera camera) {
      return camera.transform.eulerAngles.x.ToRad();
    }
    public static Bounds IsometricClipBounds(this Camera self) {
      float len = (self.nearClipPlane - self.farClipPlane).Abs();
      Vector2 size = self.IsometricCameraPlaneSize();
      var min = new Vector3(-size.x / 2, -size.y / 2, self.nearClipPlane);
      var max = new Vector3(+size.x / 2, +size.y / 2, self.farClipPlane);
      return BoundsEx.NewByMinAndMax(min, max);
    }
    //============================================================
    public static Vector2 OrthographicElevationViewSize(this Camera camera) {
      Vector2 size = camera.IsometricCameraPlaneSize();
      return new Vector2(size.x, size.y * VerticalLengthFactor);
    }
    public static Vector2 IsometricCameraPlaneSize(this Camera camera) {
      return new Vector2(camera.orthographicSize * 2 * camera.aspect, camera.orthographicSize * 2);
    }
    //============================================================
    public static Ray MouseRay(this Camera camera) {
      Ray ray = camera.ScreenPointToRay(Input.mousePosition);
      return ray;
    }
    public static Ray CenterRay(this Camera camera) {
      Ray ray = camera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
      return ray;
    }
    //============================================================
    public static void ZoomAlongRay(Camera camera, Ray ray, float distance) {
      camera.transform.Translate(ray.direction * distance, Space.World);
    }
    public static void ZoomCenteredAt(Camera camera, Vector3 position, float distance) {
      Ray ray = camera.ViewportPointToRay(position);
      ZoomAlongRay(camera, ray, distance);
    }
    public static void ZoomCenteredAtMouse(Camera camera, float distance) {
      Ray ray = camera.ScreenPointToRay(Input.mousePosition);
      ZoomAlongRay(camera, ray, distance);
    }
    //============================================================
    // negative: down/left
    // zero: no-obl
    // positive: up/right
    public static void SetFrustumObliqueness(Camera camera, float horizObl, float vertObl) {
      Matrix4x4 mat = camera.projectionMatrix;
      mat[0, 2] = horizObl;
      mat[1, 2] = vertObl;
      camera.projectionMatrix = mat;
    }
    //============================================================
    public static void FitCamera(this Camera camera, Rect screenPixelRegion, float wantViewportAspect) {
      Rect fitRect = screenPixelRegion.GetFit2(wantViewportAspect, FitMode.Shrink);
      camera.pixelRect = fitRect;
    }
    //============================================================
  }
}