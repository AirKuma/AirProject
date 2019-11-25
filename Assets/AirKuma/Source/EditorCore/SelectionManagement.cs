using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace AirKuma.UnityCore {

  //[ExecuteAlways]
  public class SelectionManager : ServiceProvider<SelectionManager> {

    //============================================================


    // ancestor: only top-level ones: there is parent, there is no child
    // individual: parent and child may be occurs together
    // descendant: selecting parent introduces all its children and descendants

#pragma warning disable RCS1130 // Bitwise operation on enum without Flags attribute.

    public Transform[] SelectedIndividualTrfs => Selection.GetFiltered<Transform>(
            /*SelectionMode.TopLevel |*/ SelectionMode.ExcludePrefab);
    public Transform[] SelectedAncestorTrfs => Selection.GetFiltered<Transform>(
            SelectionMode.TopLevel | SelectionMode.ExcludePrefab);

    public Transform[] SelectedDescendantTrfs => Selection.GetFiltered<Transform>(
            SelectionMode.Deep | SelectionMode.ExcludePrefab);

#pragma warning restore RCS1130 // Bitwise operation on enum without Flags attribute.

    public string[] SelectedIndividualAssetGuids => Selection.assetGUIDs;

    //============================================================

    public delegate void OnSelectionChangeDelegate(HashSet<Transform> curSelection, HashSet<Transform> newlyAdded, HashSet<Transform> newlyRemoved);
    public event OnSelectionChangeDelegate OnAncestorSelectionChange;
    //public event Action<IEnumerable<Transform>> OnIndividualSelectionChange;
    //public event Action<IEnumerable<Transform>> OnDescendantSelectionChange;

    // note: directory itself is a assset 
    // note: selection in first column in Two Column Mode are not supported
    public event Action<string[]> OnIndividualAssetSelectionChanage;
        
    //============================================================
    [NonSerialized]
    private HashSet<Transform> prevSelectedAncestors;
    [NonSerialized]
    private HashSet<Transform> curSelectedAncestors;
    [NonSerialized]
    private HashSet<Transform> newlyAddedAncestors;
    [NonSerialized]
    private HashSet<Transform> newlyRemovedAncestors;

    public SelectionManager() {
      newlyRemovedAncestors = new HashSet<Transform>();
      newlyAddedAncestors = new HashSet<Transform>();
      curSelectedAncestors = new HashSet<Transform>();
      UpdateSet();
      Selection.selectionChanged += HandleOnSelectionChanged;
    }

    public override void Dispose() {
      Selection.selectionChanged -= HandleOnSelectionChanged;

      prevSelectedAncestors = null;
      curSelectedAncestors = null;
      newlyAddedAncestors = null;
      newlyRemovedAncestors = null;
    }
    //============================================================
    //[NonSerialized]
    //private static readonly UnityEngine.Object[] ToRestore = null;

    //public void SaveSelection() {
    //  ToRestore.CopyFrom(Selection.objects);
    //}
    //public void RestoreSelection() {
    //  Selection.objects = ToRestore;
    //}

    public void ClearSelection() {
      Selection.objects = new UnityEngine.Object[0];
    }

    //============================================================

    //private Transform[] oldTopLevelTransforms = new Transform[0];
    private string[] oldAssetGuids = new string[0];

    public List<string> SortedIndividualAssetPaths {
      get {
        var list = new List<string>();
        foreach (string guid in Selection.assetGUIDs) {
          list.Add(AssetDatabase.GUIDToAssetPath(guid));
        }
        list.Sort();
        return list;
      }
    }

    //public event Action<Uin[]> OnIndividualAssetSelectionChanage;
    //public event Action<string[]> OnDescendantAssetSelectionChanage;


    public bool Contains(UnityEngine.Object obj) {
      return Selection.Contains(obj);
    }
    public bool Contains(GameObject obj) {
      return Selection.Contains(obj.GetInstanceID());
    }

    private void UpdateSet() {
      prevSelectedAncestors = new HashSet<Transform>(curSelectedAncestors);
      curSelectedAncestors.Clear();
      foreach (Transform trf in SelectedAncestorTrfs) {
        curSelectedAncestors.Add(trf);
      }
      if (curSelectedAncestors.GetAddedAndRemoved(prevSelectedAncestors, out newlyAddedAncestors, out newlyRemovedAncestors))
        OnAncestorSelectionChange?.Invoke(curSelectedAncestors, newlyAddedAncestors, newlyRemovedAncestors);
    }
    public void HandleOnSelectionChanged() {
      throw new Exception();
      //if (Selection.assetGUIDs.NotSequenceEqual(oldAssetGuids)) {
      //  OnIndividualAssetSelectionChanage?.Invoke(Selection.assetGUIDs);
      //  oldAssetGuids = Selection.assetGUIDs;
      //}
      //UpdateSet();
    }
     
    public Bounds? SelectedColliderBounds {
      get {
        Bounds? bounds = default;
        foreach (Transform trf in SelectedAncestorTrfs) {
          if (trf.gameObject.GetComponent<Collider>() != null) {
            if (!bounds.HasValue)
              bounds = trf.gameObject.GetComponent<Collider>().bounds;
            else
              bounds.Value.Encapsulate(trf.gameObject.GetComponent<Collider>().bounds);
          }
        }
        return bounds;
      }
    }
  }
}