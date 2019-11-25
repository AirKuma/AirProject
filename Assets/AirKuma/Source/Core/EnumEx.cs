using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AirKuma {

  [AttributeUsage(AttributeTargets.Field)]
  public class IterableEnumVal : Attribute {
  }

  public static class EnumIteration {
    private static Dictionary<Type, List<int>> dict;
    static EnumIteration() {
      dict = new Dictionary<Type, List<int>>();
    }

    public static IEnumerable<int> IterateEnumIntVals(Type enumType) {
      if (!dict.TryGetValue(enumType, out List<int> values)) {
        values = new List<int>();
        foreach (FieldInfo fieldInfo in enumType.GetFields(BindingFlags.Public | BindingFlags.Static)) {
          if (fieldInfo.HasAttr<IterableEnumVal>())
            values.Add((int)fieldInfo.GetValue(null));
        }
        if (values.Count == 0)
          throw new InvalidOperationException("the enum vals has no attribute IteratorEnumVal");
        dict.Add(enumType, values);
      }
      foreach (int val in values) {
        yield return val;
      }
    }
    public static IEnumerable<T> IterateEnumVals<T>() where T : Enum {
      return IterateEnumIntVals(typeof(T)).Cast<T>();
    }

    public static IEnumerable<T> GetDefinedEnumVals<T>() where T : System.Enum {
      return System.Enum.GetValues(typeof(T)).Cast<T>();
    }
  }

  public static class EnumNameValMapping {
    private static Dictionary<Type, DualUnorderedMap<string, int>> dict;
    static EnumNameValMapping() {
      dict = new Dictionary<Type, DualUnorderedMap<string, int>>();
    }

    private static DualUnorderedMap<string, int> BuildMap(Type enumType) {
      dict.Add(enumType, new DualUnorderedMap<string, int>());
      DualUnorderedMap<string, int> map = dict[enumType];
      foreach ((string, int) pair in EnumUtils.EachEnumNameValPair(enumType)) {
        map.Add(pair.Item1, pair.Item2);
      }
      return map;
    }
    public static string EnumValToName(Type enumType, int enumVal) {
      if (!dict.TryGetValue(enumType, out DualUnorderedMap<string, int> map)) {
        map = BuildMap(enumType);
      }
      return map.GetKey((int)enumVal);
    }
    public static int EnumNameToVal(Type enumType, string enumName) {
      if (!dict.TryGetValue(enumType, out DualUnorderedMap<string, int> map)) {
        map = BuildMap(enumType);
      }
      return map.GetValue(enumName);
    }
  }

  public static class EnumUtils {

    public static bool Flagged<T>(this T enumeration) where T : System.Enum, new() {
      return !enumeration.Equals(new T());
    }

    public static Dictionary<string, TEnum> EnumStringToValue<TEnum>() {
      var dict = new Dictionary<string, TEnum>();
      foreach (object value in Enum.GetValues(typeof(TEnum))) {
        dict.Add(value.ToString(), (TEnum)value);
      }
      return dict;
    }
    // todo: cache
    public static IEnumerable<int> EachFlaggedFlag(this int e) {
      for (int i = 0; i != 32; ++i) {
        if ((e & (2 << i)) != 0)
          yield return 2 << i;
      }
    }
    public static IEnumerable<(string enumName, int enumVal)> EachEnumNameValPair(Type enumType) {
      string[] names = Enum.GetNames(enumType);
      Array vals = Enum.GetValues(enumType);
      if (names.Length != vals.Length)
        throw new InvalidOperationException();
      int n = vals.Length;
      for (int i = 0; i != n; ++i) {
        yield return (names[i], (int)vals.GetValue(i));
      }
    }
  }

}
