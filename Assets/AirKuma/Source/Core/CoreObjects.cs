using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AirKuma {

  public interface IDataView<TItem> : IEnumerable<TItem> {
    TItem this[int index] { get; }
    int Begin { get; }
    int End { get; }
    int Length { get; }
  }
  //public readonly struct ListView  {

  //}


  public readonly struct ListView<T> : IDataView<T> {

    private readonly List<T> List;

    public ListView(List<T> list, int begin, int end) {
      List = list ?? throw new ArgumentNullException(nameof(list));
      Begin = begin;
      End = end;
    }

    public T this[int index] {
      get => List[Begin + index];
      set => List[Begin + index] = value;
    }

    public int Begin { get; }

    public int End { get; }

    public int Length => End - Begin;

    public IEnumerator<T> GetEnumerator() {
      for (int i = Begin; i != End; ++i) {
        yield return List[i];
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      for (int i = Begin; i != End; ++i) {
        yield return List[i];
      }
    }
  }

  public class AirTestAttribute : AttrWithArguments {
    public AirTestAttribute(params object[] testArguments) : base(testArguments) {
    }
  }
  public class AirTaskAttribute : AttrWithArguments {
    public AirTaskAttribute(params object[] testArguments) : base(testArguments) {
    }
  }

  public static class Service {

    private static Dictionary<Type, object> serviceDict;

    static Service() {
      serviceDict = new Dictionary<Type, object>();
    }

    public static T Get<T>() where T : class {
      return (T)Get(typeof(T));
    }
    public static object Get(Type type) {
      if (!serviceDict.TryGetValue(type, out object value)) {
        value = Activator.CreateInstance(type);
        serviceDict.Add(type, value);
      }
      return value;
    }
    public static void Activate(Type type) {
      serviceDict.Add(type, Activator.CreateInstance(type));
    }
    public static void Inactivate(Type type) {
      ((IDisposable)serviceDict[type]).Dispose();
      serviceDict.Remove(type);
    }
    public static bool IsActive(Type type) {
      return serviceDict.ContainsKey(type);
    }
    public static void EnsureActivity(Type type, bool active) {
      if (active != IsActive(type)) {
        if (active)
          Activate(type);
        else
          Inactivate(type);
      }
    }
  }

  public abstract class ServiceProvider<TService> : IDisposable
      where TService : ServiceProvider<TService>, new() {

    private static TService _service;
    public static TService Service {
      get => (TService)AirKuma.Service.Get(typeof(TService));
      //get {
      //  if (_service is null)
      //    _service = new TService();
      //  return _service;
      //}
    }


    public static bool Active {
      get => AirKuma.Service.IsActive(typeof(TService));
      set => AirKuma.Service.EnsureActivity(typeof(TService), value);
      //get => _service != null;
      //set {
      //  if (value != Active) {
      //    if (value)
      //      _service = new TService();
      //    else {
      //      _service.Dispose();
      //    }
      //  }
      //  else
      //    throw new InvalidOperationException("attempt to modify already set state");
      //}
    }
    public static void SyncActivity(bool active) {
      AirKuma.Service.EnsureActivity(typeof(TService), active);
      //if (active != Active)
      //  Active = active;
    }

    public virtual void Dispose() { }
  }

  //[Serializable]
  //public class PersistentServiceProvider<T> : ISerializationCallbackReceiver where T : PersistentServiceProvider<T>, new() {



  //  [SerializeField]
  //  private bool serializedState;
  //  [NonSerialized]
  //  private IService obj;
  //  [NonSerialized]
  //  private readonly Type type;

  //  public void SyncActivity() {
  //    if (serializedState && obj is null)
  //      Enable();
  //  }

  //  private void Enable() {
  //    obj = Activator.CreateInstance(type) as IService;
  //    obj.Enable();
  //    serializedState = true;
  //  }
  //  private void Disable() {
  //    obj.Disable();
  //    obj = null;
  //    serializedState = false;
  //  }

  //  public void OnBeforeSerialize() {
  //    serializedState = obj != null;
  //  }

  //  public void OnAfterDeserialize() {}

  //  public IService Value {
  //    get {
  //      if (!serializedState) {
  //        Enable();
  //      }
  //      return obj;
  //    }
  //  }

  //  public bool Active {
  //    get {
  //      return serializedState;
  //    }

  //    set {
  //      if (value) {
  //        if (!serializedState)
  //          Enable();
  //      }
  //      else {
  //        if (serializedState)
  //          Disable();
  //      }
  //    }
  //  }
  //}




  [Serializable]
  public struct PolymorphismConstructor<TBase> where TBase : class {
    [SerializeField]
    public Type type;
    [NonSerialized]
    private TBase obj;

    public bool HasValue => type != null;

    public PolymorphismConstructor(Type derived) {
      type = derived ?? throw new ArgumentNullException(nameof(derived));
      obj = null;
    }

    public TBase Service {
      get {
        if (obj is null || obj.GetType() != type)
          obj = Activator.CreateInstance(type) as TBase ?? throw new InvalidOperationException();
        return obj;
      }
    }
  }

  public readonly struct GuidUtils {
    public const string GuidRegex = @"[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12}";
  }

  public interface ICopiable<T> {
    T MakeCopy();
  }

  public interface IKeyedWith<TKey> {
    TKey GetKey();
  }
  public interface IConditionFor<TSituation> {
    bool Satisfies(TSituation condition);
  }

  public struct Delegation {
    public Action caller;
    public string callee;

    public Delegation(Action caller, string callee) {
      this.caller = caller ?? throw new ArgumentNullException(nameof(caller));
      this.callee = callee ?? throw new ArgumentNullException(nameof(callee));
    }

    public static implicit operator Delegation((Action caller, string callee) t) {
      return new Delegation(t.caller, t.callee);
    }
  }

  public abstract class NamedInfo<TEntryInfo> where TEntryInfo : struct {
    public static Dictionary<string, TEntryInfo> infoDict;
    public string name;
    protected NamedInfo(string name) {
      if (infoDict == null) {
        throw new Exception("infoDict should be initialized by derived static constructor");
      }
      this.name = name;
    }
    public TEntryInfo Info => infoDict[name];
  }

  public struct DefaultConstructor<T> where T : class, new() {
    private T value;
    public T Value {
      get {
        if (value is null) {
          value = new T();
        }
        return value;
      }
    }
  }


  public interface IAbility {
    void Enable();
    void Disable();
  }

  public abstract class EventFilter<T> : IAbility {

    protected T Target { get; }

    public abstract void Disable();

    public abstract void Enable();
  }

  public struct SafeVariableWatcher<T> {
    public SafeVariableWatcher(T content) : this() {
      this.content = content;
    }
    private T content;
    public Action<T> onChange;
    public T Content {
      get => content;
      set {
        if (!value.Equals(content)) {
          onChange?.Invoke(value);
          content = value;
        }
      }
    }
  }

  public struct ConstructOnAccess<T> where T : class, new() {
    private T obj;
    public T Value => obj ?? (obj = new T());
  }

  public interface IOffsetableValue<T> : IComparable<T> {
    T Add(T other);
    T Sub(T other);
    bool Negative { get; }
    bool Positive { get; }
  }

  public struct IntIndex : IOffsetableValue<IntIndex> {
    private int indexNumber;

    public IntIndex(int indexNumber) {
      this.indexNumber = indexNumber;
    }

    public override string ToString() {
      return indexNumber.ToString();
    }

    public bool Negative => indexNumber < 0;

    public bool Positive => indexNumber > 0;

    public IntIndex Add(IntIndex other) {
      return new IntIndex(indexNumber + other.indexNumber);
    }

    public int CompareTo(IntIndex other) {
      return indexNumber.CompareTo(other.indexNumber);
    }

    public IntIndex Sub(IntIndex other) {
      return new IntIndex(indexNumber - other.indexNumber);
    }

    public static implicit operator IntIndex(int indexNumber) {
      return new IntIndex(indexNumber);
    }
    public static implicit operator int(IntIndex intIndex) {
      return intIndex.indexNumber;
    }
  }

  public interface IIndexRange {

    int Begin { get; set; }
    int End { get; set; }

  }

  public struct IndexRange : IIndexRange {

    public int Begin { get; set; }
    public int End { get; set; }

    public int Length => End - Begin;

    public IndexRange(int begin, int end) {
      Begin = begin;
      End = end;
    }
  }

  public interface IIndexedMutableData<T> {

    T this[int index] { get; set; }

    void Insert(int index, T item);
    void Detete(int index);
  }

  public class IndexedMutableString : IIndexedMutableData<char> {

    private StringBuilder builder;

    public IndexedMutableString(string str = "") {
      builder = new StringBuilder(str);
    }

    public char this[int index] {
      get => builder[index];
      set => builder[index] = value;
    }

    public void Detete(int index) {
      builder.Remove(index, 1);
    }

    public void Insert(int index, char item) {
      builder.Insert(index, item);
    }
  }


  public struct TypeKey : IEquatable<TypeKey> {
    public Type type;

    public TypeKey(Type type) {
      this.type = type ?? throw new ArgumentNullException(nameof(type));
    }

    public static implicit operator TypeKey(Type type) => new TypeKey(type);

    public bool Equals(TypeKey other) {
      return type.Equals(other.type);
    }

    public override int GetHashCode() {
      return type.GetHashCode();
    }

    public override string ToString() {
      return type.Name;
    }
  }

  public struct EnumKey<T> : IEquatable<EnumKey<T>> where T : Enum {

    T enumVal;
    public EnumKey(T enumVal) { this.enumVal = enumVal; }

    [Todo(TodoGoal.CheckPerformanceIssue)]
    public bool Equals(EnumKey<T> other) {
      return this.enumVal.Equals(other.enumVal);
    }

    public override int GetHashCode() {
      return this.enumVal.GetHashCode();
    }

    public override string ToString() {
      return this.enumVal.ToString();
    }
  }
}
namespace AirKuma {
  //protected override Delegation[] Delegations => new Delegation[] {
  //  (Selection.selectionChanged, nameof(this.TheFn))
  //};

  //protected override (Action, Action)[] DelegateToRegister {
  //  get {
  //    return new (Action, Action)[] {
  //      (Selection.selectionChanged, HandleOnSelectionChanged )
  //    };
  //  }
  //}

  //var dele = Delegate.CreateDelegate(typeof(Action), this, nameof(TheFn));
  //Selection.selectionChanged = Delegate.Combine(Selection.selectionChanged,
  //  Delegate.CreateDelegate(typeof(int), typeof(int), "")) as Action;

  //var dele = Delegate.CreateDelegate(typeof(Action), typeof(SelectionManager), "TheFn", false, true);
  //Selection.selectionChanged += (Action)dele;
  //Selection.selectionChanged += Delegate.Combine(Selection.selectionChanged, dele);
  //this.Update.

  //MethodInfo[] methodInfos = Type.GetType(selectedObjcClass)
  //                         .GetMethods(BindingFlags.Public | BindingFlags.Instance);

}