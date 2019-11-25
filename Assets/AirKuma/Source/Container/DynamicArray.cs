using System;
using System.Collections;
using System.Collections.Generic;

namespace AirKuma {

  [Serializable]
  public class DynamicArr<T> : IEnumerable<T> where T : IEquatable<T> {

    [UnityEngine.SerializeField]
    private T[] arr;

    [UnityEngine.SerializeField]
    private int next = 0;

    public DynamicArr() {
      arr = null;
    }
    public DynamicArr(int initCapacity) {
      arr = new T[initCapacity];
    }

    #region size related functions

    private int NextIndex {
      get => next;
      set {
        next = value;
        if (next is 0)
          arr = null;
      }
    }

    public int Length {
      get => NextIndex;
      set {
        if (value != NextIndex) {
          if (value > NextIndex) {
            Reserve(value);
          } else {
            for (int i = value; i != NextIndex; ++i)
              arr[i] = default;
          }
          NextIndex = value;
        }
      }
    }
    public int Count => Length;

    public int Capacity {
      get => arr?.Length ?? 0;
      set {
        if (value != Capacity) {
          if (value < Length)
            throw new ArgumentException("new capacity must be greater or equal to current length capacity");
          if (arr is null) {
            arr = new T[value];
          } else {
            var newArr = new T[value];
            for (int i = 0; i != NextIndex; ++i) {
              newArr[i] = arr[i];
            }
            arr = newArr;
          }
        }
      }
    }

    public void Reserve(int requiredSize) {
      if (Capacity < requiredSize) {
        Capacity = requiredSize << 1;
      }
    }

    public bool Empty => NextIndex is 0;

    #endregion 
    //============================================================

    public ref T Front {
      get {
        if (NextIndex is 0)
          throw new InvalidOperationException();
        return ref arr[0];
      }
    }
    public ref T Back {
      get {
        if (NextIndex is 0)
          throw new InvalidOperationException();
        return ref arr[NextIndex - 1];
      }
    }

    public ref T this[int index] {
      get {
        if (!IsIndexValid(index))
          throw new IndexOutOfRangeException();
        return ref arr[index];
      }
    }
    public ref T ReverseAt(int reverseIndex) {
      if (!IsIndexValid(reverseIndex))
        throw new IndexOutOfRangeException();
      return ref arr[GetReverseIndex(reverseIndex)];
    }

    public int GetReverseIndex(int index) {
      return NextIndex - index - 1;
    }

    public IEnumerable<T> ReversedItems() {
      for (int i = NextIndex; i != 0; --i) {
        yield return arr[i - 1];
      }
    }

    public bool Contains(T item) {
      for (int i = 0; i != NextIndex; ++i) {
        if (arr[i].Equals(item))
          return true;
      }
      return false;
    }

    public bool IsIndexValid(int index) {
      return index >= 0 && index < NextIndex;
    }

    
    #region adding functions

    public void InsertAt(int index, T item) {
      Reserve(NextIndex + 1);
      for (int i = index; i != NextIndex; ++i) {
        arr[i + 1] = arr[i];
      }
      arr[index] = item;
      ++NextIndex;
    }

    public void AddFront(T item) {
      InsertAt(0, item);
    }

    public void AddBack(T item) {
      Reserve(NextIndex + 1);
      arr[NextIndex++] = item;
    }
    public void Add(T item) {
      AddBack(item);
    }

    public void Push(T item) {
      AddBack(item);
    }

    #endregion
    
    #region removal functions

    public void RemoveAt(int index) {
      if (!IsIndexValid(index))
        throw new ArgumentException();
      for (int i = index + 1; i != NextIndex; ++i) {
        arr[i - 1] = arr[i];
      }
      arr[--NextIndex] = default;
    }

    public void RemoveFront() {
      if (NextIndex is 0)
        throw new InvalidOperationException();
      RemoveAt(0);
    }
    public void RemoveBack() {
      if (NextIndex is 0)
        throw new InvalidOperationException();
      arr[--NextIndex] = default;
    }

    // remove the first item found
    public void RemoveFirst(T item) {
      for (int i = 0; i < NextIndex; ++i) {
        if (arr[i].Equals(item)) { RemoveAt(i); return; }
      }
      throw new ArgumentException();
    }
    public void Remove(T item) {
      RemoveFirst(item);
    }

    public void RemoveLast(T item) {
      for (int i = Count; i != 0; --i) {
        if (arr[i - 1].Equals(item)) { RemoveAt(i - 1); return; }
      }
      throw new ArgumentException();
    }

    public void RemoveAll(T item) {
      throw new NotImplementedException();
    }

    public void RemoveBefore(int end) {
      throw new NotImplementedException();
    }
    public void RemoveAfter(int start) {
      for (int i = start; i != NextIndex; ++i)
        arr[i] = default;
      NextIndex = start;
    }

    // without freeing memory
    public void RemoveAll() {
      for (int i = 0; i != next; ++i)
        arr[i] = default;
      next = 0;
    }
    public void Clear() {
      NextIndex = 0;
      arr = null;
    }

    #endregion
    
    IEnumerator<T> EachItem() {
      for (int i = 0; i != NextIndex; ++i) {
        yield return arr[i];
      }
    }
    public IEnumerator<T> GetEnumerator() {
      return EachItem();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return EachItem();
    }
    public List<T> Items => EachItem().ToList();
    
    #region ordered functions

    public void Sort() {
      System.Array.Sort(arr);
    }

    // if contains equivalent items, guarantee return indeex to first one
    private int BinarySearchFirst(T item) {
      int i = Array.BinarySearch(arr, item);
      if (~i == NextIndex) {
        return ~i;
      }
      if (i < 0) {
        i = ~i;
      }
      T eqItem = arr[i];
      while (true) {
        if (i - 1 >= 0 && arr[i - 1].Equals(eqItem)) {
          --i;
        } else {
          return i;
        }
      }
    }
    internal int LowerBound(T item) {
      return BinarySearchFirst(item);
    }

    internal int UpperBound(T item) {
      int index = BinarySearchFirst(item);
      if (index != NextIndex && arr[index].Equals(item)) {
        return index + 1;
      }
      return index;
    }
    #endregion
    //============================================================
    public override string ToString() {
      return this.ToStringForEach("dynamic-array"); 
    } 
     
    internal static void Test() {
      DynamicArr<int> arr = new DynamicArr<int>();
      for (int i = 0; i != 1000; i++) {
        int n = IntEx.Random(0, 300);
        List<int> candidates = new List<int>();
        for (int j = 0; j != n; ++j)
          candidates.Add(IntEx.Random(0, 200));
        DebugEx.Assert(arr.Count == 0);
        foreach (int num in candidates)
          arr.Add(num);
        DebugEx.Assert(arr.Count == candidates.Count);
        foreach (int num in candidates)
          arr.Remove(num);
        DebugEx.Assert(arr.Count == 0);
      }
      DebugEx.Log("ok");
    }
  }

  
}
