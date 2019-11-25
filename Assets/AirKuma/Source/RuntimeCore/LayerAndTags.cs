using System;
using System.Collections.Generic;
using System.Text;
using AirKuma;
using UnityEditor;
using UnityEngine;

namespace AirKuma.UnityCore {

  public struct LayerMaskProxy {

    public static LayerMaskProxy AllLayer => new LayerMaskProxy(~0);

    private int mask;
    private LayerMaskProxy(int mask) {
      this.mask = mask;
    }
    public LayerMaskProxy Union(LayerMaskProxy other) {
      return new LayerMaskProxy(mask | other.mask);
    }
    public static implicit operator LayerMask(LayerMaskProxy proxy) => proxy.mask;
    public static explicit operator int(LayerMaskProxy proxy) => proxy.mask;

    public static implicit operator LayerMaskProxy(LayerIndexProxy indexProxy) => new LayerMaskProxy(indexProxy.MaskFlag);

    public bool HasLayer(LayerIndexProxy index) {
      return (mask & index.MaskFlag) != 0;
    }
  }

  public struct LayerIndexProxy {

    // layerIndex is from 0 to 31
    public LayerIndexProxy(int layerIndex) {
      Index = layerIndex;
    }
    public LayerIndexProxy(string layerName) {
      Index = LayerMask.NameToLayer(layerName);
      Debug.Assert(Index != -1);
    }

    public static explicit operator int(LayerIndexProxy proxy) => proxy.Index;

    private int Index { get; }
    public int MaskFlag => 1 << Index;

    public string Name => LayerMask.LayerToName(MaskFlag);

  }
}
