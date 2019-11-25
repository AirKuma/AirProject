using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using AirKuma.UnityCore;

namespace AirKuma.HitTest {

  public static class EditorHitTestEx {
    public static bool RaycastFirstSelected(this Ray ray, out RaycastHit hit, HitScope? scope = null) {
      scope = scope ?? HitScope.AllScopes;
      int n = Physics.RaycastNonAlloc(ray, RayCastBuffer.buffer, Mathf.Infinity, (int)scope.Value.layerMask, scope.Value.QueryTriggerInteraction);
      for (int i = 0; i != n; ++i) {
        if (SelectionManager.Service.Contains(RayCastBuffer.buffer[i].collider.gameObject)) {
          hit = RayCastBuffer.buffer[i];
          return true;
        }
      }
      hit = default;
      return false;
    }

    public static IEnumerable<Collider> SelectiveHitTest(this Bounds self, HitScope? scope = null) {
      foreach (Collider collider in self.GlobalHitTest(scope)) {
        if (UnityEditor.Selection.Contains(collider.gameObject.GetInstanceID()))
          yield return collider;
      }
    }
    public static bool HitsWithAnySelection(this Bounds bounds, HitScope? scope = null) {
      return bounds.SelectiveHitTest(scope).GetEnumerator().MoveNext();
    }

  }
}
