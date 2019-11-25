using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AirKuma {


  public class KumaStack<T> : IEnumerable<T> where T : IEquatable<T> {

    private DynamicArr<T> arr = new DynamicArr<T>();

    public KumaStack() { }

    //============================================================
    public ref T Front => ref arr.Front;
    public ref T Back => ref arr.Back;
    public ref T Top => ref Back;

    public int Count => arr.Count;

    public ref T ReverseAt(int index) => ref arr.ReverseAt(index);
    public int GetReverseIndex(int index) => arr.GetReverseIndex(index);

    public ref T this[int index] => ref arr[index];

    public void AddNew(T item) => arr.Add(item);

    public bool Contains(T item) => arr.Contains(item);

    public void RemoveBack() => arr.RemoveBack();
    public void RemoveLast(T item) => arr.RemoveLast(item);

    public void RemoveAt(int index) => arr.RemoveAt(index);

    public void RemoveAll(T item) => arr.RemoveAll(item);

    public void RemoveAfter(int index) => arr.RemoveAfter(index);

    public void Clear() => arr.Clear();

    //============================================================

    public void Push(T item) {
      AddNew(item);
    }

    public void Remove(T item) {
      RemoveLast(item);
    }

    public void Pop() {
      RemoveBack();
    }
    public void PopUntil(T untilItem) {
      for (int i = Count; i != 0; --i) {
        if (arr[i - 1].Equals(untilItem)) { RemoveAfter(i); return; }
      }
      Clear();
    }
    public void PopInclusivelyUntil(T untilItem) {
      for (int i = Count; i != 0; --i) {
        if (arr[i - 1].Equals(untilItem)) { RemoveAfter(i - 1); return; }
      }
      Clear();
    }

    public void MoveToTop(int index) {
      bool alreadyAtTop = index == Count - 1;
      if (alreadyAtTop) {
        return;
      }
      T itemToMove = arr[index];
      for (int i = index; i + 1 != Count; ++i) {
        arr[i] = arr[i + 1];
      }
      arr[Count - 1] = itemToMove;
    }

    //public void FillWithDefault() {
    //  for (int i = 0; i != array.Length; ++i) {
    //    array[i] = default;
    //  }
    //}


    public IEnumerator<T> GetEnumerator() {
      return arr.ReversedItems().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return arr.ReversedItems().GetEnumerator();
    }

    
  }

  public class StateStack<TState> where TState : struct, IEquatable<TState> {

    public struct GuardToRestore : IDisposable {
      public StateStack<TState> obj;

      public void Dispose() {
        obj.Restore();
      }
    }

    private KumaStack<TState> array = new KumaStack<TState>();

    public StateStack() {
      Init(default);
    }
    public StateStack(TState state) {
      Init(state);
    }

    public void Init(TState state) {
      Debug.Assert(Count == 0);
      array.AddNew(state);
    }

    public ref TState Top => ref array.Back;

    public int Count => array.Count;

    public ref TState Save() {
      array.AddNew(Top);
      return ref Top;
    }
    public void Restore() {
      array.RemoveBack();
    }

    public GuardToRestore SaveWithGuard() {
      Save();
      return new GuardToRestore { obj = this };
    }
  }
}