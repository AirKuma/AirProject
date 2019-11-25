using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AirKuma {


  public static class OpenAddressHash {

    public static float EvalLoadingFactorLimit(int currentCapacity) {
      return (4f / Mathf.Log(currentCapacity, 2)).Clamp(1f / 16f, 1f / 2f);
      // C++:  return clamp(4. / std::log2(_capacity), 1. / 16., 1. / 2.);
    }

    public static bool IsLoadingFactorExceeded(int capacity, int count) {
      float loadingFactcorLimit = EvalLoadingFactorLimit(capacity);
      return count > Mathf.FloorToInt(capacity * loadingFactcorLimit);
    }

    public static bool Probe<TKey>(bool[] flags, TKey[] keys, TKey key, out int index)
         where TKey : IEquatable<TKey> {
      if (flags == null) {
        index = default;
        return false;
      }
      int hashCode = key.GetHashCode();
      index = (hashCode < 0 ? -hashCode : hashCode) % flags.Length;
      int originalIndex = index;
      while (true) {
        if (!flags[index]) {
          return false; 
        }
        if (keys[index].Equals(key)) {
          return true;
        }
        index = ((index + 1) % flags.Length);
        DebugEx.Assert(index != originalIndex);
      } 
    }
    public static void EnsureHasEnoughCapacity<TKey, TValue>(ref bool[] flags, ref TKey[] keys, ref TValue[] values, int count)
        where TKey : IEquatable<TKey> {

      int oldCapacity = keys == null ? 0 : keys.Length;
      int newCapacity = oldCapacity;
      while (true) {
        if (IsLoadingFactorExceeded(newCapacity, count))
          newCapacity = (newCapacity + 1).GetCeilOfPowerOf2();
        else
          break;
      }

      if (newCapacity == oldCapacity)
        return;

      bool[] oldFlags = flags;
      TKey[] oldKeys = keys;
      TValue[] oldValues = values;

      flags = new bool[newCapacity];
      keys = new TKey[newCapacity];
      values = new TValue[newCapacity];

      for (int oldIndex = 0; oldIndex != oldCapacity; ++oldIndex) {
        if (oldFlags[oldIndex]) {
          Probe(flags, keys, oldKeys[oldIndex], out int newIndex);
          flags[newIndex] = true;
          keys[newIndex] = oldKeys[oldIndex];
          values[newIndex] = oldValues[oldIndex];
        }
      }
    }

    public static void EnsureHasEnoughCapacity<TKey>(ref bool[] flags, ref TKey[] keys, int count)
    where TKey : IEquatable<TKey> {

      int oldCapacity = keys == null ? 0 : keys.Length;
      int newCapacity = oldCapacity;
      while (true) {
        if (IsLoadingFactorExceeded(newCapacity, count))
          newCapacity = (newCapacity + 1).GetCeilOfPowerOf2();
        else
          break;
      }

      if (newCapacity == oldCapacity)
        return;

      bool[] oldFlags = flags;
      TKey[] oldKeys = keys;

      flags = new bool[newCapacity];
      keys = new TKey[newCapacity];

      for (int oldIndex = 0; oldIndex != oldCapacity; ++oldIndex) {
        if (oldFlags[oldIndex]) {
          Probe(flags, keys, oldKeys[oldIndex], out int newIndex);
          flags[newIndex] = true;
          keys[newIndex] = oldKeys[oldIndex];
        }
      }
    }

  }

  // note: value type should implement IEquatable for performance
  //    see: https://github.com/dotnet/coreclr/issues/1341#issuecomment-131070210
  //    (using of Object.Equals requires a boxing, thus use IEquatable<TKey> constraint here)
  // (using of GetHashCode here for value type seems not requiring/causing a boxing)
  [Serializable]
  public class UnorderedMap<TKey, TValue> : IEnumerable<(TKey key, TValue value)>
      where TKey : IEquatable<TKey> {

    // uses individual lists instead of array of bucket struct here for Unity serialization
    [UnityEngine.SerializeField]
    bool[] flags;
    [UnityEngine.SerializeField]
    TKey[] keys;
    [UnityEngine.SerializeField]
    TValue[] values;

    [UnityEngine.SerializeField]
    public int Count;

    public UnorderedMap() {
      flags = null;
      keys = null;
      values = null;
      Count = 0;
    }
    public UnorderedMap(int initBucketCapacity) {
      flags = new bool[initBucketCapacity];
      keys = new TKey[initBucketCapacity];
      values = new TValue[initBucketCapacity];
      Count = 0;
    }

    public int Capacity => keys == null ? 0 : keys.Length;

    bool Probe(TKey key, out int index) {
      if (this.keys == null) {
        index = default;
        return false;
      }
      return OpenAddressHash.Probe(flags, keys, key, out index);
    }

    public bool TryGetValue(TKey key, out TValue value) {
      if (Probe(key, out int index)) {
        value = values[index];
        return true;
      }
      value = default;
      return false;
    }

    public bool ContainsKey(TKey key) {
      return Probe(key, out int dummy);
    }

    IEnumerator<(TKey key, TValue value)> EachKeyValuePairs() {
      for (int i = 0; i != Capacity; ++i) {
        if (flags[i]) {
          yield return (keys[i], values[i]);
        }
      }
    }
    public List<(TKey key, TValue value)> KeyValuePairs => EachKeyValuePairs().ToList();

    public IEnumerator<(TKey key, TValue value)> GetEnumerator() {
      return EachKeyValuePairs();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return EachKeyValuePairs();
    }

    public ref TValue this[TKey key] {
      get {
        if (Probe(key, out int index)) {
          return ref values[index];
        }
        throw new ArgumentException("count not find the key");
      }
    }

    void EnsureLoadingFactorIsOkAfterAddingNewItem() {
      OpenAddressHash.EnsureHasEnoughCapacity(ref this.flags, ref this.keys, ref this.values, this.Count + 1);
    }

    void InternalAddNewItem(int index, TKey key, TValue value) {
      flags[index] = true;
      keys[index] = key;
      values[index] = value;
      ++this.Count;
    }

    void InternalRemoveItem(int index) {
      flags[index] = false;
      keys[index] = default;
      values[index] = default;
      --this.Count;
    }

    public void AssignOrAdd(TKey key, TValue value) {
      EnsureLoadingFactorIsOkAfterAddingNewItem();
      bool found = Probe(key, out int index);
      if (found) {
        values[index] = value;
      } else {
        InternalAddNewItem(index, key, value);
      }
    }

    public void Add(TKey key, TValue value) {
      EnsureLoadingFactorIsOkAfterAddingNewItem();
      if (Probe(key, out int index)) {
        throw new ArgumentException("already contains same key");
      } else {
        InternalAddNewItem(index, key, value);
      }
    }
    public void AddIfNone(TKey key, TValue value) {
      EnsureLoadingFactorIsOkAfterAddingNewItem();
      if (!this.Probe(key, out int index)) {
        InternalAddNewItem(index, key, value);
      }
    }

    void Reinsert(int index) {
      while (true) {
        if (!flags[index])
          return;
        if (!Probe(keys[index], out int new_index)) {
          (new_index != index).Assert();
          flags[new_index] = true;
          keys[new_index] = keys[index];
          values[new_index] = values[index];
          flags[index] = false;
          keys[index] = default;
          values[index] = default;
        } else
          (new_index == index).Assert();
        index = (index + 1) % Capacity;
      }   
    }

    //void remove(const key_type &key) {
    //  size_t index;
    //bool r = probe(key, index);
    //must(r); // it does not contains the key to remove
    //_buckets[index].erase();
    //reinsert((index + 1) % _capacity);
    //  --this->_count;
    //}

    public void Remove(TKey key) {
      if (Probe(key, out int index)) {
        InternalRemoveItem(index);
        this.Reinsert((index + 1) % Capacity);
      } else {
        throw new ArgumentException("can not find the key to remove");
      }
    }
    public void RemoveIfAny(TKey key) {
      if (Probe(key, out int index)) {
        InternalRemoveItem(index);
        this.Reinsert((index + 1) % Capacity);
      }
    }

    /// <summary>
    /// remove all key-value pairs without releasing memory
    /// </summary>
    public void RemoveAll() {
      for (int i = 0; i != Capacity; ++i) {
        flags[i] = false;
        keys[i] = default;
        values[i] = default;
      }
      this.Count = 0;
    }

    /// <summary>
    ///  RemoveAll() and release memory
    /// </summary>
    public void Clear() {
      RemoveAll();
      flags = null;
      keys = null;
      values = null;
    }

    public override string ToString() {
      return this.ToStringForEach("unordered-map");
    }
    public void Validate() {
      if (keys == null || values == null) {
        Debug.Assert(keys == null && values == null);
      } else {
        int count = 0;
        for (int i = 0; i != Capacity; ++i) {
          if (flags[i])
            ++count;
        }
        (count == this.Count).Assert();
      }
    }

    internal static void Test() {

      for (int i = 0; i != 100; ++i) {
        UnorderedMap<int, float> map = new UnorderedMap<int, float>();

        int n = IntEx.Random(0, 10000);
        List<int> list = new List<int>();
        for (int j = 0; j != n; ++j) {
          list.Add(IntEx.Random(0, 256));
        }
        foreach (int x in list) {
          map.AddIfNone(x, (float)x);
          map.Validate();
        }

        foreach (int x in list) {
          map.RemoveIfAny(x);
          map.Validate();
        }
       (map.Count == 0).Assert();
      }
      Debug.Log("ok 4");
    }

    internal static void Profile() {
      int item_count = 1000000;
      {
        UnorderedMap<int, float> map = new UnorderedMap<int, float>();


        for (int i = 0; i != item_count; ++i) {
          map.Add(i, (float)i);
        }


        var now = (float)EditorApplication.timeSinceStartup;


        for (int i = 0; i != item_count; ++i) {
          map.ContainsKey(i);
        }


        Debug.Log((float)EditorApplication.timeSinceStartup - now);
      }
      //{
      //  Dictionary<int, float> map = new Dictionary<int, float>();


      //  for (int i = 0; i != item_count; ++i) {
      //    map.Add(i, (float)i);
      //  }


      //  var now = (float)EditorApplication.timeSinceStartup;

      //  for (int j = 0; j != 1000; ++j) {
      //    for (int i = 0; i != item_count; ++i) {
      //      map.ContainsKey(i);
      //    }
      //  }

      //  Debug.Log((float)EditorApplication.timeSinceStartup - now);
      //}
    }
  }

  [Serializable]
  public class UnorderedSet<T> : IEnumerable<T>
    where T : IEquatable<T> {

    [UnityEngine.SerializeField]
    bool[] flags;
    [UnityEngine.SerializeField]
    T[] items;

    [UnityEngine.SerializeField]
    int Count;

    public UnorderedSet() {
      flags = null;
      items = null;
      Count = 0;
    }
    public UnorderedSet(int initCapacity) {
      flags = new bool[initCapacity];
      items = new T[initCapacity];
      Count = 0;
    }

    public int Capacity => items.Length;

    bool Probe(T item, out int index) {
      return OpenAddressHash.Probe(flags, items, item, out index);
    }

    IEnumerator<T> EachItem() {
      for (int i = 0; i != Capacity; ++i) {
        if (flags[i]) {
          yield return items[i];
        }
      }
    }
    public IEnumerator<T> GetEnumerator() {
      return EachItem();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return EachItem();
    }
    public List<T> Items => EachItem().ToList();

    void EnsureLoadingFactorIsOkAfterAddingNewItem() {
      OpenAddressHash.EnsureHasEnoughCapacity(ref this.flags, ref this.items, this.Count + 1);
    }
    void InternalAddItem(int index, T item) {
      flags[index] = true;
      items[index] = item;
      ++this.Count;
    }
    void InternalRemoveItem(int index) {
      flags[index] = false;
      items[index] = default;
      --this.Count;
    }

    public bool Contains(T item) {
      return Probe(item, out int dummy);
    }

    public void Add(T item) {
      EnsureLoadingFactorIsOkAfterAddingNewItem();
      if (Probe(item, out int index))
        throw new ArgumentException("already contains the same item");
      InternalAddItem(index, item);
    }
    public void AddIfNone(T item) {
      EnsureLoadingFactorIsOkAfterAddingNewItem();
      if (!this.Probe(item, out int index)) {
        InternalAddItem(index, item);
      }
    }

    public void Remove(T item) {
      if (Probe(item, out int index)) {
        InternalRemoveItem(index);
      } else {
        throw new ArgumentException("can not find the item to remove");
      }
    }
    public void RemoveIfAny(T item) {
      if (Probe(item, out int index)) {
        InternalRemoveItem(index);
      }
    }

    public void RemoveAll() {
      for (int i = 0; i != Capacity; ++i) {
        this.flags[i] = false;
        this.items[i] = default;
      }
      this.Count = 0;
    }
    public void Clear() {
      RemoveAll();
      this.flags = null;
      this.items = null;
    }

    public override string ToString() {
      return this.ToStringForEach("unordered-set");
    }

    public void UnionExclusivelyWith(UnorderedSet<T> other) {
      foreach (T item in other) {
        if (this.Contains(item))
          throw new Exception();
        this.Add(item);
      }
    }
    public static UnorderedSet<T> operator |(UnorderedSet<T> left, UnorderedSet<T> right) {
      var result = new UnorderedSet<T>();
      result.UnionExclusivelyWith(left);
      result.UnionExclusivelyWith(right);
      return result;
    }
  }

  public class DualUnorderedMap<TKey, TValue>
      where TKey : IEquatable<TKey>
      where TValue : IEquatable<TValue> {

    UnorderedMap<TKey, TValue> keyToValue;
    UnorderedMap<TValue, TKey> valueToKey;

    public DualUnorderedMap() {
      keyToValue = new UnorderedMap<TKey, TValue>();
      valueToKey = new UnorderedMap<TValue, TKey>();
    }
    public DualUnorderedMap(int initialCapacity) {
      keyToValue = new UnorderedMap<TKey, TValue>(initialCapacity);
      valueToKey = new UnorderedMap<TValue, TKey>(initialCapacity);
    }


    public void Add(TKey key, TValue value) {
      if (!keyToValue.ContainsKey(key))
        keyToValue.Add(key, value);
      if (!valueToKey.ContainsKey(value))
        valueToKey.Add(value, key);
    }
    public void Remove(TKey key, TValue value) {
      if (keyToValue.ContainsKey(key))
        keyToValue.Remove(key);
      if (valueToKey.ContainsKey(value))
        valueToKey.Remove(value);
    }

    public TValue GetValue(TKey key) {
      return keyToValue[key];
    }

    public TKey GetKey(TValue value) {
      return valueToKey[value];
    }

    public bool TryGetValue(TKey key, out TValue value) {
      return keyToValue.TryGetValue(key, out value);
    }
    public bool TryGetKey(TValue value, out TKey key) {
      return valueToKey.TryGetValue(value, out key);
    }

    public void Clear() {
      keyToValue.Clear();
      valueToKey.Clear();
    }


  }

  static class TestMap {
    [MenuItem("AirKuma/Profile")]
    static void ProfileMap() {
      UnorderedMap<int, float>.Profile();
    }
  }

}