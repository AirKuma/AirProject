using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace AirKuma {

  public static class AppEnv {
    public static bool UsingUnity { get; set; } = true;
  }

  public static class AttributedTypeCache {
    public static UnorderedMap<TypeKey, UnorderedSet<TypeKey>> catalog = new UnorderedMap<TypeKey, UnorderedSet<TypeKey>>(1024);

    static AttributedTypeCache() {
      foreach (var type in Meta.kumaAsms.AllTypes()) {
        foreach (var attr in type.GetCustomAttributes()) {
          if (!catalog.TryGetValue(attr.GetType(), out UnorderedSet<TypeKey> types)) {
            types = new UnorderedSet<TypeKey>(4);
            catalog.Add(attr.GetType(), types);
          }
          types.AddIfNone(type);
        }
      }
    }
  }

  public static class Meta {

    public static T New<T>(this Type type) where T : class {
      return Activator.CreateInstance(type) as T ?? throw new InvalidOperationException();
    }

    public static IEnumerable<Type> SelfAndBaseTypes(this Type type) {
      yield return type;
      if (type.BaseType != null) {
        foreach (Type t in type.BaseType.SelfAndBaseTypes())
          yield return t;
      }
    }

    public static Assembly[] kumaAsms;
    static Meta() {
      if (AppEnv.UsingUnity) {
        kumaAsms = new Assembly[] {

        //Assembly.Load("Assembly-CSharp"),
        Assembly.Load("CoreLib"),
        Assembly.Load("EngineCoreLib"),
        Assembly.Load("GuiLib"),
        Assembly.Load("ModLib"),
        Assembly.Load("GamePlayLib"),

#if UNITY_EDITOR
        //Assembly.Load("Assembly-CSharp-Editor"),
        Assembly.Load("EditorCoreLib"),
        Assembly.Load("EditorGuiLib"),
        Assembly.Load("EditorUtilsLib"),
        Assembly.Load("EditorModLib"),
        Assembly.Load("GameEditLib"),
        Assembly.Load("HkbdLib"),
        Assembly.Load("MiscLib"),
#endif

      };
      }
    }

    //public static Assembly KumaAssembly => Assembly.Load("CoreLib");
    //public static Assembly KumaAssembly => typeof(Meta).Assembly;

  }

  public static class TypeResolver {
    static TypeResolver() { }
    //============================================================
    public static IEnumerable<Type> AllTypes(this Assembly assembly, Type withAttr = null, bool concreateOnly = false, Type derivedFrom = null) {
      foreach (Type type in assembly.GetTypes()) {
        if ((withAttr is null || type.HasAttr(withAttr))
          && (!concreateOnly || !type.IsAbstract)
          && (derivedFrom is null || type.IsSubclassOf(derivedFrom))) {
          yield return (type);
        }
      }
    }
    public static IEnumerable<Type> AllTypes(this Assembly[] asms, Type withAttr = null, bool concreateOnly = false, Type derivedFrom = null) {
      foreach (Assembly asm in asms)
        foreach (Type type in asm.AllTypes(withAttr, concreateOnly, derivedFrom))
          yield return type;
    }
    //============================================================
    public static Type FindType(this Assembly asm, string qualifiedName) {
      return asm.GetType(qualifiedName, true);
    }
    public static Type FindType(this Assembly[] asms, string qualifiedName) {
      foreach (Assembly asm in asms) {
        Type type = asm.GetType(qualifiedName, false);
        if (type != null)
          return type;
      }
      throw new ArgumentException("not found");
    }
    //============================================================
    public static List<Type> AllDerivedTypes<TBase>(this Assembly assembly) where TBase : class {
      var result = new List<Type>();
      foreach (Type type in assembly.GetTypes()) {
        if (type.IsSubclassOf(typeof(TBase))) {
          result.Add(type);
        }
      }
      return result;
    }
  }

  public interface IAttrWithArguments {
    object[] Arguments { get; }
  }

  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
  public abstract class AttrWithArguments : Attribute, IAttrWithArguments {

    public object[] Arguments { get; }

    public AttrWithArguments(params object[] testArguments) {
      Arguments = testArguments;
    }
  }

  public static class MetaExtensions {


    public static string GetFullName(this Type type) {
      return (type.DeclaringType?.GetFullName().Concate(".").Concate(type.Name))
          ?? ((type.Namespace ?? "").Concate("::").Concate(type.Name));
    }
    public static string GetFullName(this MemberInfo member) {
      return member.DeclaringType.GetFullName() + "." + member.Name;
    }

    //============================================================
    public static bool HasAttr<TAttr>(this MemberInfo member) {
      return Attribute.IsDefined(member, typeof(TAttr));
    }
    public static bool HasAttr(this MemberInfo member, Type attrType) {
      return Attribute.IsDefined(member, attrType);
    }
    //------------------------------------------------------------
    public static TAttr GetAttr<TAttr>(this MemberInfo member, bool inherit = true) where TAttr : Attribute {
      return (TAttr)member.GetCustomAttribute(typeof(TAttr), inherit);
    }

    //============================================================
    public static IEnumerable<MethodInfo> AllMethods(this Assembly assembly, bool staticOnly = false, Type withAttr = null) {

      BindingFlags wantStaticMethod = BindingFlags.Public
              | BindingFlags.NonPublic
              | (staticOnly ? BindingFlags.Static : (BindingFlags)0)
              | BindingFlags.FlattenHierarchy;

      foreach (Type type in assembly.GetTypes()) {
        foreach (MethodInfo method in type.GetMethods(
              wantStaticMethod)) {
          if (withAttr is null || method.HasAttr(withAttr)) {
            yield return method;
          }
        }
      }
    }
    public static IEnumerable<MethodInfo> AllMethods(this Assembly[] asms, bool staticOnly = false, Type withAttr = null) {
      foreach (Assembly asm in asms)
        foreach (MethodInfo method in asm.AllMethods(staticOnly, withAttr))
          yield return method;
    }
    //============================================================

    public static string GetSignature(this MethodInfo method, object[] arguments) {
      if (method.IsStatic) {
        var sb = new StringBuilder(method.GetFullName());
        sb.Append('(');
        for (int i = 0; i != arguments.Length; ++i) {
          sb.Append(arguments[i].About());
          if (i != arguments.Length - 1) {
            sb.Append(", ");
          }
        }
        sb.Append(')');
        return sb.ToString();
      } else {
        throw new NotImplementedException();
      }
    }

    public static string InvokeWithSummary(this MethodInfo method, object[] arguments) {
      Debug.Assert(method.IsStatic);
      var summary = new StringBuilder();
      summary.AppendLine($"invoke '{method.GetFullName()}'");
      summary.AppendLine("arguments:");
      foreach (object arg in arguments) {
        summary.AppendLine($"\t{arg.About(1)}");
      }
      if (method.ReturnType == typeof(void)) {
        method.Invoke(null, arguments);
      } else {
        object result = method.Invoke(null, arguments);
        summary.AppendLine($"result:\n\t{result.About()}");
      }
      return summary.ToString();
    }
    //============================================================
  }
}
