using System;
using System.Collections.Generic;
using System.Text;

namespace AirKuma {

  public interface IIndexable<TItem> {
    TItem this[int index] { get; set; }
    int Count { get; }
    void Add(TItem item);
  }

  //public interface IAirCollection<T> : IEnumerable<T> {

  //  bool Contains(T item);

  //  // add one (useually append one) item
  //  void AddNew(T item);

  //  // remove one (usually the first) item
  //  void Remove(T item);

  //  void RemoveAll(T item);
  //}

  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  public static class CollectionEx {
    public static void AddIfNone<T>(this ICollection<T> collection, T item) {
      if (!collection.Contains(item)) {
        collection.Add(item);
      }
    }
    public static void RemoveIfHas<T>(this ICollection<T> collection, T item) {
      if (collection.Contains(item)) {
        collection.Remove(item);
      }
    }
  }


  public static class EnumerableEx {

    public static bool Has<T>(this IEnumerable<T> e, T item) {
      foreach(T one in e) {
        if (one.Equals(item))
          return true;
      }
      return false;
    }

    public static IEnumerable<(T1, T2)> EachWith<T1, T2>(this IEnumerable<T1> e1, IEnumerable<T2> e2) {
      IEnumerator<T1> i1 = e1.GetEnumerator();
      IEnumerator<T2> i2 = e2.GetEnumerator();
      while (i1.MoveNext() && i2.MoveNext()) {
        yield return (i1.Current, i2.Current);
      }
    }

    // ! perofrmance
    //public static IEnumerator<T> GetIterator<T>(this IEnumerable<T> enumerable) {
    //  IEnumerator<T> iterator = enumerable.GetEnumerator();
    //  iterator.MoveNext();
    //  return iterator;
    //}

    //============================================================

    public static List<TDst> TranslateToList<TDst, TSrc>(this IEnumerable<TSrc> list, Func<TSrc, TDst> translator) {
      var newList = new List<TDst>();
      foreach (TSrc item in list) {
        newList.Add(translator(item));
      }
      return newList;
    }

    //============================================================
    public static int GetIndexOf(this System.Collections.IEnumerable enumerable, object item) {
      int c = 0;
      foreach (object one in enumerable) {
        if (item.Equals(one))
          return c;
        ++c;
      }
      return c;
    }
    public static int GetIndexOf<T>(this IEnumerable<T> enumerable, T item) {
      int c = 0;
      foreach (T one in enumerable) {
        if (item.Equals(one))
          return c;
        ++c;
      }
      return c;
    }

    //============================================================
    public static IEnumerable<TDervied> FilterByType<TDervied>(this System.Collections.IEnumerable enumerable)
        where TDervied : class {
      foreach (object item in enumerable) {
        if (item is TDervied derviedItem)
          yield return derviedItem;
      }
    }

    //============================================================
    public static T GetFirstOne<T>(this IEnumerable<T> enumerable) {
      IEnumerator<T> tor = enumerable.GetEnumerator();
      tor.MoveNext();
      return tor.Current;
    }

    //============================================================
    public static bool IsEachThat<T>(this IEnumerable<T> enumerable, Predicate<T> pred) {
      foreach (T item in enumerable) {
        if (!pred(item)) {
          return false;
        }
      }
      return true;
    }
    public static bool IsAnyThat<T>(this IEnumerable<T> enumerable, Predicate<T> pred) {
      foreach (T item in enumerable) {
        if (pred(item)) {
          return true;
        }
      }
      return false;
    }
    public static bool IsNoneThat<T>(this IEnumerable<T> enumerable, Predicate<T> pred) {
      foreach (T item in enumerable) {
        if (pred(item)) {
          return false;
        }
      }
      return true;
    }
    //============================================================

    public static string ToStringForEach(this System.Collections.IEnumerable enumerable, string head = "Collection") {
      var str = new StringBuilder(head);
      foreach (object item in enumerable) {
        str.Append($" '{item.ToString()}'");
      }
      return str.ToString();
    }
    public static string ToStringForEach<T>(this IEnumerable<T> enumerable, string head = "Collection") {
      var str = new StringBuilder(head);
      foreach (T item in enumerable) {
        str.Append($" '{item.ToString()}'");
      }
      return str.ToString();
    }

    public delegate TDestination Convertor<TSource, TDestination>(TSource source);
    public static IEnumerable<TDestination> ConvertEach<TSource, TDestination>(this IEnumerable<TSource> source, Convertor<TSource, TDestination> convertor) {
      foreach (TSource item in source) {
        yield return convertor(item);
      }
    }

    public static int Count<T>(this IEnumerable<T> enumerable) {
      return enumerable.GetEnumerator().Count();
    }
    public static int Count<T>(this IEnumerator<T> enumerator) {
      int i = 0;
      while (enumerator.MoveNext()) {
        ++i;
      }
      return i;
    }
    //============================================================
    public static TItem YieldItemAt<TItem>(this IEnumerable<TItem> items, int index) {
      if (!index.IsWithinRange(0, items.Count()))
        throw new InvalidOperationException();
      int i = 0;
      foreach (TItem item in items) {
        if (i++ == index) {
          return item;
        }
      }
      throw new Exception();
    }
    public static TItem YieldItemAt<TItem>(this IList<TItem> items, int index) {
      return items[index];
    }
    //============================================================

    // if contains equivalent items, guarantee return indeex to first one
    public static int BinarySearchFirst<TItem>(this List<TItem> list, TItem item, IComparer<TItem> comparer) {
      int i = list.BinarySearch(item, comparer);
      if (~i == list.Count) {
        return ~i;
      }

      if (i < 0) {
        i = ~i;
      }

      TItem eqItem = list[i];
      while (true) {
        if (i - 1 >= 0 && list[i - 1].Equals(eqItem)) {
          --i;
        } else {
          return i;
        }
      }
    }



    public static void RemoveBack(this StringBuilder sb, int count) {
      sb.Remove(sb.Length - count, count);
    }



    public static T GetMax<T>(this IEnumerable<T> enumerable) where T : System.IComparable {
      T max = default;
      foreach (T item in enumerable) {
        if (item.CompareTo(max) > 0) {
          max = item;
        }
      }
      return max;
    }

    public delegate bool FilterPredicate<T>(T item);
    public static IEnumerable<T> KumaFilter<T>(this IEnumerable<T> enumerable, FilterPredicate<T> pred) {
      foreach (T item in enumerable) {
        if (pred(item)) {
          yield return item;
        }
      }
    }

    public static TVal GetOrNewKeyedValue<TKey, TVal>(this Dictionary<TKey, TVal> dict, TKey key) where TVal : new() {
      if (!dict.ContainsKey(key)) {
        dict.Add(key, new TVal());
      }
      return dict[key];
    }

    public static void FillWith<T>(this List<T> list, T fillWithItem) {
      for (int i = 0; i != list.Count; ++i) {
        list[i] = fillWithItem;
      }
    }

    public static List<T> ToList<T>(this IEnumerable<T> enumerable) {
      return new List<T>(enumerable);
    }
    public static List<T> ToList<T>(this IEnumerator<T> etor) {
      List<T> list = new List<T>();
      while (etor.MoveNext()) {
        list.Add(etor.Current);
      }
      return list;
    }

    public static List<T> CopyForModification<T>(this IEnumerable<T> enumerable) {
      return enumerable.ToList();
    }


    public static void RequireSize<T>(this T[] array, int requiredSize) {
      if (array.Length < requiredSize) {
        System.Array.Resize(ref array, requiredSize);
      }
    }
    public static ref T GetFront<T>(this T[] array) {
      return ref array[0];
    }
    public static ref T GetBack<T>(this T[] array) {
      return ref array[array.Length - 1];
    }
    public static void FillWithDefault<T>(this T[] array) {
      for (int i = 0; i != array.Length; ++i) {
        array[i] = default;
      }
    }

    public static void DefaultConstructAtLeast<T>(this List<T> list, int requiredCount) {
      if (list.Capacity < requiredCount) {
        list.Capacity = requiredCount;
      }

      int n = list.Count;
      if (n < requiredCount) {
        for (int i = 0; i != requiredCount - n; ++i) { list.Add(default); }
      }
    }
    public static void DefaultConstruct<T>(this List<T> list, int exactCount) {
      list.Clear();
      for (int i = 0; i != exactCount; ++i) {
        list.Add(default);
      }
    }

    public static void AddUnique<T>(this List<T> list, T item) {
      if (!list.Contains(item)) {
        list.Add(item);
      }
    }

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public static T GetFront<T>(this IList<T> list) {
      return list[0];
    }
    public static T GetBack<T>(this IList<T> list) {
      return list[list.Count - 1];
    }

    public static void RemoveAtIndices<T>(this List<T> list, IEnumerable<int> indicesToRemove) {
      int removedCount = 0;
      foreach (int index in indicesToRemove) {
        list.RemoveAt(index - removedCount++);
      }
    }

    public static IEnumerable<T> GetLinearTimeUniqueItems<T>(this IEnumerable<T> enumerable) {
      var exclusive = new HashSet<T>();
      foreach (T v in enumerable) {
        if (exclusive.Contains(v)) {
          continue;
        }
        exclusive.Add(v);
        yield return v;
      }
    }

    public static IEnumerable<T> EachWithinRange<T>(this List<T> list, int begin, int end) {
      for (int i = begin; i != end; ++i) {
        yield return list[i];
      }
    }

    public static bool ContainsAtRange<T>(this IList<T> list, int start, int length, T item) {
      for (int i = 0; i != length; ++i) {
        if (list[start + i].Equals(item)) {
          return true;
        }
      }
      return false;
    }
    public static IEnumerable<T> ExponentialTimeUniqueItems<T>(this IList<T> list) {
      for (int i = 0; i != list.Count; ++i) {
        if (!list.ContainsAtRange(0, i, list[i])) {
          yield return list[i];
        }
      }
    }
    public static void LinearTimeRemoveDuplicated<T>(this List<T> list) {
      list.RemoveAll((v) => list.Contains(v));
    }

  }

  public static class ArrayEx {

    public static IEnumerable<T> EachReversed<T>(this T[] arr) {
      for (int i = arr.Length; i != 0; --i) {
        yield return arr[i - 1];
      }
    }

    public static void CopyTo<T>(this T[] source, T[] destination) {
      Array.Copy(source, destination, source.Length);
    }
    public static void CopyFrom<T>(this T[] destination, T[] source) {
      Array.Copy(source, destination, source.Length);
    }
  }

  public static class ListEx {

    public static IEnumerable<T> EachReversed<T>(this List<T> list) {
      for (int i = list.Count; i != 0; --i) {
        yield return list[i - 1];
      }
    }


    //============================================================
    public static int LowerBound<TItem>(this List<TItem> list, TItem item, IComparer<TItem> comparer) {
      return list.BinarySearchFirst(item, comparer);
    }
    public static int UpperBound<TItem>(this List<TItem> list, TItem item, IComparer<TItem> comparer) {
      int index = list.BinarySearchFirst(item, comparer);
      return index != list.Count && list[index].Equals(item) ? index + 1 : index;
    }
    public static int LowerBound<T>(this List<T> list, T item) {
      return list.LowerBound(item, Comparer<T>.Default);
    }
    public static int UpperBound<T>(this List<T> list, T item) {
      return list.UpperBound(item, Comparer<T>.Default);
    }
    //============================================================
    public static void RemoveFirst<TItem>(this List<TItem> list, TItem item) {
      for (int i = 0; i != list.Count; ++i) {
        if (list[i].Equals(item)) {
          list.RemoveAt(i);
        }
      }
    }
    public static void RemoveLast<TItem>(this List<TItem> list, TItem item) {
      for (int i = list.Count; i != 0; --i) {
        if (list[i - 1].Equals(item)) {
          list.RemoveAt(i - 1);
        }
      }
    }
    //============================================================

    public static List<T> AddedWithItem<T>(this List<T> list, T item) {
      if (list is null)
        list = new List<T>();
      list.Add(item);
      return list;
    }

    //============================================================
    public static IEnumerable<T> Sublist<T>(this IList<T> list, int begin, int end) {
      for (int i = begin; i != end; ++i) {
        yield return list[i];
      }
    }
  }

  public static class SetEx {

    //public static void ExclusiveUnion<T>(this HashSet<T> self, IEnumerable<T> other) {

    //}

    public static bool GetAddedAndRemoved<T>(this HashSet<T> newSet, HashSet<T> oldSet, out HashSet<T> added, out HashSet<T> removed) {
      added = new HashSet<T>();
      removed = new HashSet<T>();
      foreach (T item in newSet) {
        if (!oldSet.Contains(item)) {
          added.Add(item);
        }
      }
      foreach (T item in oldSet) {
        if (!newSet.Contains(item)) {
          removed.Add(item);
        }
      }
      return added.Count != 0 || removed.Count != 0;
    }

    
    public static void Toggle<T>(this HashSet<T> set, T item) {
      if (set.Contains(item)) {
        set.Remove(item);
      } else {
        set.Add(item);
      }
    }
    public static void Modify<T>(this HashSet<T> set, SetAction action, T item) {
      switch (action) {
        case SetAction.Toggle:
          set.Toggle(item);
          break;
        case SetAction.Add:
          set.AddIfNone(item);
          break;
        case SetAction.Remove:
          set.RemoveIfHas(item);
          break;
      }
    }
  }

  public static class DictionaryEx {

    public static void AddEntryIfNone<TKey, TVal>(this Dictionary<TKey, TVal> dict, TKey key, TVal val) {
      if (!dict.ContainsKey(key)) {
        dict.Add(key, val);
      }
    }
    public static void RemoveEntryIfHas<TKey, TVal>(this Dictionary<TKey, TVal> dict, TKey key) {
      if (dict.ContainsKey(key)) {
        dict.Remove(key);
      }
    }

    //============================================================
    public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value) {
      if (!dict.ContainsKey(key)) {
        dict.Add(key, value);
        return true;
      }
      return false;
    }
    public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) {
      if (dict.ContainsKey(key)) {
        dict.Remove(key);
        return true;
      }
      return false;
    }
    //============================================================
    public static TKey GetOneKeyByValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue value) {
      foreach (KeyValuePair<TKey, TValue> pair in dict) {
        if (pair.Value.Equals(value)) {
          return pair.Key;
        }
      }
      throw new ArgumentException();
    }
    //============================================================
    public static TValue? NullableGet<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        where TValue : struct {
      return dict.TryGetValue(key, out TValue other) ? (TValue?)other : null;
    }

    public static void NullableSet<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue? value)
        where TValue : struct {
      bool b = dict.ContainsKey(key);
      if (value.HasValue) {
        if (b)
          dict[key] = value.Value;
        else
          dict.Add(key, value.Value);
      } else {
        if (b)
          dict.Remove(key);
      }
    }
    //============================================================
    public static void AddOrAssign<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value) {
      if (dict.ContainsKey(key)) {
        dict[key] = value;
      } else {
        dict.Add(key, value);
      }
    }
    public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) {
      return dict.TryGetValue(key, out TValue outValue) ? outValue : (default);
    }
    public static TValue GetOrAddNew<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) where TValue : class, new() {
      if (!dict.TryGetValue(key, out TValue outValue)) {
        outValue = new TValue();
        dict.Add(key, outValue);
      }
      return outValue;
    }
  }


}