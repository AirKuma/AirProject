using System;
using System.Collections.Generic;
using AirKuma.UnityCore;
using UnityEngine;

namespace AirKuma.HitTest {

  

  [Serializable]
  public class ColliderSet : AirSet<Collider> { }

  public struct HitScope {
    public static HitScope AllScopes => new HitScope(LayerMaskProxy.AllLayer, true);
    public LayerMaskProxy layerMask;
    public bool includesTrigger;

    public HitScope(LayerMaskProxy? mask = null, bool includesTrigger = true) {
      layerMask = mask ?? LayerMaskProxy.AllLayer;
      this.includesTrigger = includesTrigger;
    }

    public QueryTriggerInteraction QueryTriggerInteraction {
      get => includesTrigger ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;
    }
  }
  public static class HitTestEx {
    public static bool PointHitTest(this Vector2 point, out RaycastHit hit) {
      return UnityEngine.Physics.Raycast(point, Vector3.zero, out hit, 0);
    }
    public static int GlobalHitTest(this Bounds self, Collider[] result, HitScope scope) {
      return Physics.OverlapBoxNonAlloc(self.center,
        self.extents,
        result,
        Quaternion.identity,
        (LayerMask)scope.layerMask,
        scope.QueryTriggerInteraction);
    }
    public static bool HitsGlobally(this Bounds bounds, HitScope? scope = null) {
      int r = GlobalHitTest(bounds, HitTestBuffer.buffer, scope ?? HitScope.AllScopes);
      return r != 0;
    }
    
    public static bool HitsWith(this Bounds bounds, Collider collider, HitScope? scope = null) {
      int n = GlobalHitTest(bounds, HitTestBuffer.buffer, scope ?? HitScope.AllScopes);
      for (int i = 0; i != n; ++i) {
        if (HitTestBuffer.buffer[i] == collider)
          return true;
      }
      return false; 
    }
    public static Collider[] GlobalHitTest(this Bounds self, HitScope? scope = null) {
      scope = scope ?? HitScope.AllScopes;
      return Physics.OverlapBox(self.center,
        self.extents,
        Quaternion.identity,
        (LayerMask)(scope.Value).layerMask,
        scope.Value.QueryTriggerInteraction);
    }
  }

  public static class RayCastBuffer {
    public static readonly RaycastHit[] buffer;
    static RayCastBuffer() {
      buffer = new RaycastHit[256];
    }
  }

  public static class RayEx {


    public static Vector3 RayCastOntoPlane(this Ray ray, Plane plane) {
      plane.Raycast(ray, out float distance);
      return ray.GetPoint(distance);
    }
    public static GameObject RayCastFirst(this Ray ray) {
      if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Collide)) {
        return null;
      }

      return hit.collider.gameObject;
    }
    public static bool RayCastFirst(this Ray ray, out RaycastHit hit) {
      return Physics.Raycast(ray, out hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Collide);
    }

  }

  public static class HitTestBuffer {

    private static int bufferSize = 256;
    public static Collider[] buffer = new Collider[bufferSize];

    public static void Reserve(int requiredBufferSize) {
      if (bufferSize < requiredBufferSize) {
        bufferSize = requiredBufferSize;
        buffer = new Collider[bufferSize];
      }
    }
  }
}
