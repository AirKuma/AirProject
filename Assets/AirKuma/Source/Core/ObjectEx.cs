using System.Collections.Generic;
using System.Text;
using System.Reflection;
using UnityEngine;
using System;

namespace AirKuma {


  public static class ObjectEx {


    public static string About<TKey, TValue>(this Dictionary<TKey, TValue> dict) {
      var str = new StringBuilder("Dictionary\n");
      foreach (var pair in dict) {
        str.AppendLine($"\t{pair.Key.About()}: {pair.Value.About()}");
      }
      return str.ToString();
    }

    public static string About(this object obj, int indentationLevel = 0) {
      switch (obj) {
        case System.Type typeObject:
          return $"<{typeObject.Name}>";
        case Transform trf:
          return trf.name;
        case GameObject gameObject:
          return $"{gameObject.name} (id: {gameObject.GetInstanceID()})";
        case char chr:
          return $"'{chr}'";
        case string str:
          return $"\"{str}\"";
        case int integer:
          return integer.ToString();
        case float floatNumber:
          return floatNumber.ToString() + "f";
        case double doubleNumber:
          return doubleNumber.ToString();
        case System.Collections.IEnumerable enumerable: {
            string indStr_ = (indentationLevel + 1).GetIndentationString();
            var result = new StringBuilder($"Enumerable");
            int i = 0;
            foreach (object item in enumerable) {
              result.Append($"\n{indStr_}[{i++}] {item.About(indentationLevel + 2)}");
            }
            return result.ToString();
          }
        default:
          return obj.ToString();
      }
    }

    //public static string AboutOld(this object obj, int indentationLevel = 0) {
    //  var sb = new StringBuilder($"{obj.GetType().Name}\n");
    //  string indStr = (indentationLevel + 1).GetIndentationString();
    //  foreach (MemberInfo info in obj.GetType().GetMembers()) {
    //    if (info is PropertyInfo pInfo) {
    //      sb.Append($"{indStr}@ {pInfo.Name} = {pInfo.GetValue(obj).About(indentationLevel + 1)}\n");
    //    }
    //    else if (info is FieldInfo fInfo) {
    //      object subObj = fInfo.GetValue(obj);
    //      if (subObj.GetType().IsValueType) {
    //        sb.Append($"{indStr}= {fInfo.Name} = {subObj.About(indentationLevel + 1)}\n");
    //      }
    //      else {
    //        sb.Append($"{indStr}& {fInfo.Name} = {subObj.About(indentationLevel + 1)}\n");
    //      }
    //    }
    //  }
    //  return sb.ToString();
    //}

    //============================================================
  }

  //  //string String(int indentationCount = 0) {
  //  //  StringBuilder sb = new StringBuilder();
  //  //  sb.Append(this.head.ToString());
  //  //  if (this.tail != null) {
  //  //    foreach (ObjInfo a in this.tail) {
  //  //      sb.Append('\n');
  //  //      for (int i = 0; i != indentationCount + 1; ++i) {
  //  //        sb.Append('\t');
  //  //      }
  //  //      sb.Append(a.String(indentationCount + 1));
  //  //    }
  //  //  }
  //  //  sb.Append("!!!");
  //  //  return sb.ToString();
  //  //}

}
