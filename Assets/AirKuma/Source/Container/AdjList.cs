//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace AirKuma.AdjList {

//  public interface INode {
//    object Value { get; set; }
//    IEnumerable<INode> EachChild { get; }
//  }
//  public interface ITypedNode<T> : INode {
//    new T Value { get; set; }
//    new IEnumerable<ITypedNode<T>> EachChild { get; }
//  }
//  public static class TreeEx {
//    //============================================================
//    public static int GetChildCount(this INode node) {
//      return node.EachChild.Count();
//    }
//    public static int GetDescendantCount(this INode node) {
//      int c = 0;
//      foreach (INode child in node.EachChild) {
//        c += child.GetDescendantCount();
//      }
//      return c + 1;
//    }
//    //============================================================
//    public static IEnumerable<INode> PreOrderTraverse(this INode node) {
//      yield return node;
//      foreach (INode chi in node.EachChild) {
//        foreach (INode dsc in node.PreOrderTraverse()) {
//          yield return dsc;
//        }
//      }
//    }
//    //============================================================
//    public static IEnumerable<INode> DepthFirstTraverse(this INode node, CircularBuffer<INode> buf) {
//      yield return node;
//      foreach (INode child in node.EachChild) {
//        buf.Write(child);
//      }
//      while (!buf.Empty) {
//        foreach (INode n in buf.Read().DepthFirstTraverse(buf)) {
//          yield return n;
//        }
//      }
//    }
//    public static IEnumerable<INode> DepthFirstTraverse(this INode node, int? bufSize = null) {
//      var buf = new CircularBuffer<INode>(bufSize ?? node.GetDescendantCount(), allowsOverwriting: false);
//      foreach (INode n in node.DepthFirstTraverse(buf)) {
//        yield return n;
//      }
//    }

//    //============================================================
//    private static void PreOrderReprWithIndentations(this INode node, int depth, StringBuilder sb) {
//      sb.AppendLine(depth.GetIndentationString() + node.About());
//      foreach (INode child in node.EachChild) {
//        sb.AppendLine(depth.GetIndentationString() + node.About());
//      }
//    }
//    public static string PreOrderReprWithIndentations(this INode node) {
//      var sb = new StringBuilder();
//      sb.AppendLine("Tree");
//      PreOrderReprWithIndentations(node, 0, sb);
//      return sb.ToString();
//    }
//  }

//  public interface IAdjList {

//    IEnumerable<int> AllNodes { get; }

//    int AddNode();
//    bool ContainsNode(int node);

//    bool HasChild(int node);
//    bool HasParent(int node);

//    void BreakAndRemoveNode(int node);

//    void Parent(int node, int par);
//    void Unparent(int node, int par);
//  }
//  public static class AdjListEx {

//    public static void AddChild(this IAdjList adjList, int node, int child) {
//      adjList.Parent(child, node);
//    }
//    //============================================================
//    public static IEnumerable<int> GetRootNodes(this IAdjList adjList) {
//      foreach (int node in adjList.AllNodes) {
//        if (!adjList.HasParent(node)) {
//          yield return node;
//        }
//      }
//    }
//    //public static IEnumerable<int> LeafNodes(this IAdjList adjList, int node) {
//    //  IEnumerable<int> DuplicatedLeafNodes(int n) {
//    //    if (adjList.HasChild(n))
//    //      foreach (int chi in adjList.EachChildOf(n)) {
//    //        foreach (int leaf in LeafNodes(chi)) {
//    //          yield return leaf;
//    //        }
//    //      }
//    //    else {
//    //      yield return n;
//    //    }
//    //  }
//    //  return DuplicatedLeafNodes(node).GetLinearTimeUniqueItems();
//    //}
//    //============================================================
//  }

//  public interface ISingleParentAdjList : IAdjList {

//    int GetParentOf(int node);

//    bool TryGetParent(int node, out int par);
//  }

//  public interface IMultipleParentAdjList : IAdjList {

//    IEnumerable<int> EachParentOf(int node);

//  }

//  // provides constant time inserting and removing nodes
//  // provides fast horizontal (sibling) traversal (e.g., traversal of leaf nodes)
//  public class LinkedAdjList : ISingleParentAdjList {

//    public int NodeCount { get; private set; } = 0;

//    private HashSet<int> AllNodeSet { get; } = new HashSet<int>();
//    public IEnumerable<int> AllNodes => AllNodeSet;

//    private Dictionary<int, int> parents = new Dictionary<int, int>();
//    private Dictionary<int, (int, int)> boundaryChildPairs = new Dictionary<int, (int, int)>();
//    private Dictionary<int, int> prevSiblings = new Dictionary<int, int>();
//    private Dictionary<int, int> nextSiblings = new Dictionary<int, int>();

//    //============================================================
//    public int AddNode() {
//      AllNodeSet.Add(NodeCount);
//      return NodeCount++;
//    }
//    public bool ContainsNode(int node) {
//      return AllNodeSet.Contains(node);
//    }
//    //============================================================
//    public bool HasParent(int node) {
//      return parents.ContainsKey(node);
//    }
//    public bool HasChild(int node) {
//      return boundaryChildPairs.ContainsKey(node);
//    }
//    //------------------------------------------------------------
//    public int GetParentOf(int node) {
//      return parents[node];
//    }
//    public int GetFirstChild(int node) {
//      return boundaryChildPairs[node].Item1;
//    }
//    public int GetLastChild(int node) {
//      return boundaryChildPairs[node].Item2;
//    }
//    //------------------------------------------------------------
//    public bool TryGetParent(int node, out int parent) {
//      return parents.TryGetValue(node, out parent);
//    }
//    public bool TryGetBoundaryChildPair(int node, out (int, int) boundaryChildPair) {
//      return boundaryChildPairs.TryGetValue(node, out boundaryChildPair);
//    }
//    //============================================================
//    public bool HasPrevSibling(int node) {
//      return prevSiblings.ContainsKey(node);
//    }
//    public bool HasNextSibling(int node) {
//      return nextSiblings.ContainsKey(node);
//    }
//    //------------------------------------------------------------
//    public int GetPrevSiblingOf(int node) {
//      return prevSiblings[node];
//    }
//    public int GetNextSiblingOf(int node) {
//      return nextSiblings[node];
//    }
//    //------------------------------------------------------------
//    public bool TryGetPrevSiblingOf(int node, out int prevNode) {
//      return prevSiblings.TryGetValue(node, out prevNode);
//    }
//    public bool TryGetNextSiblingOf(int node, out int nextNode) {
//      return nextSiblings.TryGetValue(node, out nextNode);
//    }
//    //============================================================
//    public void PrependChild(int node, int newFirstChildNode) {
//      if (TryGetBoundaryChildPair(node, out (int, int) boundaryChildPair)) {
//        prevSiblings.AddOrAssign(boundaryChildPair.Item1, newFirstChildNode);
//        nextSiblings.Add(newFirstChildNode, boundaryChildPair.Item1);
//        boundaryChildPairs[node] = (newFirstChildNode, boundaryChildPair.Item2);
//      }
//      else {
//        boundaryChildPairs.Add(node, (newFirstChildNode, newFirstChildNode));
//      }
//      parents.Add(newFirstChildNode, node);
//    }
//    public void AppendChild(int node, int newLastChildNode) {
//      if (TryGetBoundaryChildPair(node, out (int, int) boundaryChildPair)) {
//        nextSiblings.AddOrAssign(boundaryChildPair.Item2, newLastChildNode);
//        prevSiblings.Add(newLastChildNode, boundaryChildPair.Item2);
//        boundaryChildPairs[node] = (boundaryChildPair.Item1, newLastChildNode);
//      }
//      else {
//        boundaryChildPairs.Add(node, (newLastChildNode, newLastChildNode));
//      }
//      parents.Add(newLastChildNode, node);
//    }
//    //------------------------------------------------------------
//    public void Parent(int node, int par) {
//      AppendChild(par, node);
//    }
//    public void Unparent(int node, int par) {
//      BreakParentsAndSiblingsFor(node);
//    }
//    //------------------------------------------------------------
//    public void BreakParentsAndSiblingsFor(int node) {
//      if (TryGetParent(node, out int par)) {
//        (int, int) boundarySiblingPair = boundaryChildPairs[par];
//        bool isTheOnlyChild = boundarySiblingPair.Item1 == boundarySiblingPair.Item2;
//        if (isTheOnlyChild) {
//          boundaryChildPairs.Remove(par);
//        }
//        else {
//          bool isTheFirstSibling = boundarySiblingPair.Item1 == node;
//          bool isTheLastSibling = boundarySiblingPair.Item2 == node;
//          if (isTheFirstSibling || isTheLastSibling) {
//            (int, int) newBoundarySiblingPair = default;
//            if (isTheFirstSibling) {
//              newBoundarySiblingPair.Item1 = nextSiblings[node];
//            }
//            else if (isTheLastSibling) {
//              newBoundarySiblingPair.Item2 = prevSiblings[node];
//            }
//            boundaryChildPairs[par] = newBoundarySiblingPair;
//          }
//          bool hasPrev = TryGetPrevSiblingOf(node, out int prev);
//          bool hasNext = TryGetNextSiblingOf(node, out int next);
//          bool isMiddleChild = hasPrev && hasNext;
//          if (isMiddleChild) {
//            nextSiblings[prev] = next;
//            prevSiblings[next] = prev;
//          }
//          else if (hasPrev) {
//            nextSiblings.Remove(prev);
//          }
//          else if (hasNext) {
//            prevSiblings.Remove(next);
//          }
//        }
//      }
//    }
//    public void BreakChildrenFor(int node) {
//      if (TryGetBoundaryChildPair(node, out (int, int) boundaryChildPair)) {
//        parents.Remove(boundaryChildPair.Item1);
//        if (boundaryChildPair.Item1 != boundaryChildPair.Item2)
//          parents.Remove(boundaryChildPair.Item2);
//        boundaryChildPairs.Remove(node);
//      }
//    }
//    public void BreakAndRemoveNode(int node) {
//      BreakParentsAndSiblingsFor(node);
//      BreakChildrenFor(node);
//      AllNodeSet.Remove(node);
//    }
//    //============================================================
//    public void InsertPrevSibling(int node, int prevSiblingNode) {
//      HasParent(node).Assert();
//      if (TryGetPrevSiblingOf(node, out int prevPrev)) {
//        nextSiblings[prevPrev] = prevSiblingNode;
//        prevSiblings.Add(prevSiblingNode, prevPrev);
//        prevSiblings[node] = prevSiblingNode;
//        nextSiblings.Add(prevSiblingNode, node);
//      }
//      else {
//        (int, int) oldBoundarySiblingPair = boundaryChildPairs[parents[node]];
//        boundaryChildPairs[parents[node]] = (prevSiblingNode, oldBoundarySiblingPair.Item1);
//        prevSiblings.Add(node, prevSiblingNode);
//        nextSiblings.Add(prevSiblingNode, node);
//      }
//      parents.Add(prevSiblingNode, parents[node]);
//    }
//    public void InsertNextSibling(int node, int nextSiblingNode) {
//      HasParent(node).Assert();
//      if (TryGetNextSiblingOf(node, out int nextNext)) {
//        prevSiblings[nextNext] = nextSiblingNode;
//        nextSiblings.Add(nextSiblingNode, nextNext);
//        nextSiblings[node] = nextSiblingNode;
//        prevSiblings.Add(nextSiblingNode, node);
//      }
//      else {
//        (int, int) oldBoundarySiblingPair = boundaryChildPairs[parents[node]];
//        boundaryChildPairs[parents[node]] = (oldBoundarySiblingPair.Item1, nextSiblingNode);
//        nextSiblings.Add(node, nextSiblingNode);
//        prevSiblings.Add(nextSiblingNode, node);
//      }
//      parents.Add(nextSiblingNode, parents[node]);
//    }
//    //============================================================
//    public IEnumerable<int> EachChildOf(int node) {
//      if (TryGetBoundaryChildPair(node, out (int, int) boundaryChildPair)) {
//        int cur = boundaryChildPair.Item1;
//        do {
//          yield return cur;
//        } while (TryGetNextSiblingOf(cur, out cur));
//      }
//    }
//    //============================================================
//  }

//  [Serializable]
//  public class ArrayedAdjList<TKey> {

//    //[UnityEngine.SerializeField]
//    //private SerializableHashSet<TKey> AllNodeSet { get; set; } = new SerializableHashSet<TKey>();

//    [UnityEngine.SerializeField]
//    private Map<TKey, List<TKey>> childList = new Map<TKey, List<TKey>>();
//    [UnityEngine.SerializeField]
//    private Map<TKey, List<TKey>> parentList = new Map<TKey, List<TKey>>();

//    //============================================================
//    //public void AddNode(TKey newKey) {
//    //  AllNodeSet.Add(newKey);
//    //}

//    //public IEnumerable<TKey> AllNodes => AllNodeSet;

//    //public bool ContainsNode(TKey node) {
//    //  return AllNodeSet.Contains(node);
//    //}

//    //============================================================
//    public bool HasParent(TKey child) {
//      return parentList.ContainsKey(child);
//    }
//    public bool HasChild(TKey parent) {
//      return childList.ContainsKey(parent);
//    }

//    public int ParentCountOf(TKey child) {
//      if (parentList.ContainsKey(child)) {
//        return parentList[child].Count;
//      }

//      return 0;
//    }
//    public int ChildCountOf(TKey parent) {
//      if (childList.ContainsKey(parent)) {
//        return childList[parent].Count;
//      }

//      return 0;
//    }
//    //============================================================
//    //------------------------------------------------------------
//    public List<TKey> ParentListOf(TKey child) {
//      if (parentList.TryGetValue(child, out List<TKey> parents)) {
//        return parents;
//      }
//      return null;
//    }
//    public IEnumerable<TKey> EachParentOf(TKey child) {
//      return ParentListOf(child);
//    }
//    //------------------------------------------------------------
//    public List<TKey> ChildListOf(TKey parent) {
//      if (childList.TryGetValue(parent, out List<TKey> children)) {
//        return children;
//      }
//      return null;
//    }
//    public IEnumerable<TKey> EachChildOf(TKey child) {
//      return ChildListOf(child);
//    }
//    //============================================================
//    public TKey LastChildOf(TKey parent) {
//      if (childList.TryGetValue(parent, out List<TKey> children)) {
//        return children.GetBack();
//      }
//      throw new Exception();
//    }
//    //============================================================
//    public void Parent(TKey child, TKey parent) {
//      childList.GetOrNewKeyedValue(parent).Add(child);
//      parentList.GetOrNewKeyedValue(child).Add(parent);
//    }
//    public void Unparent(TKey child, TKey parent) {
//      childList[parent].Remove(child);
//      parentList[child].Remove(parent);
//      if (childList[parent].Count == 0) {
//        childList.Remove(parent);
//      }
//      if (parentList[child].Count == 0) {
//        parentList.Remove(child);
//      }
//    }
//    //============================================================
//    public void BreakParentsFor(TKey node) {
//      if (parentList.ContainsKey(node)) {
//        foreach (TKey parent in parentList[node]) {
//          childList[parent].Remove(node);
//        }
//      }
//      parentList.Remove(node);
//    }
//    public void BreakChildrenFor(TKey node) {
//      if (childList.ContainsKey(node)) {
//        foreach (TKey child in childList[node]) {
//          parentList[child].Remove(node);
//        }
//      }
//      childList.Remove(node);
//    }
//    public void BreakNode(TKey node) {
//      BreakParentsFor(node);
//      BreakChildrenFor(node);
//    }
//    //public void BreakAndRemoveNode(TKey node) {
//    //  BreakNode(node);
//    //  AllNodeSet.Remove(node);
//    //}
//    //============================================================
//    public static void Test(IMultipleParentAdjList adjList) {
//      {
//        //      0            0            0      
//        //     / \          / \          / \     
//        //    1   2        1   1        1   2    
//        //   / \   \      / \   \      / \   \   
//        //  3   4   |    2   2   |    3   4   |  
//        //   \ / \ /      \ / \ /      \ / \ /   
//        //    5   6        3   6        5   6    
//        //   / \ / \      / \ / \      / \ / \   
//        //  9   7   |    9   7   |    9   7   |  
//        // / \   \ /    / \   \ /    / \   \ /   
//        //10 11   8    10 11   8    10 11   8    
//        int[] nodes = new int[12];
//        for (int i = 0; i != nodes.Length; ++i) {
//          nodes[i] = adjList.AddNode();
//        }
//        adjList.Parent(nodes[1], nodes[0]);
//        adjList.Parent(nodes[2], nodes[0]);
//        adjList.Parent(nodes[3], nodes[1]);
//        adjList.Parent(nodes[4], nodes[1]);
//        adjList.Parent(nodes[5], nodes[3]);
//        adjList.Parent(nodes[5], nodes[4]);
//        adjList.Parent(nodes[6], nodes[4]);
//        adjList.Parent(nodes[6], nodes[2]);
//        adjList.Parent(nodes[7], nodes[5]);
//        adjList.Parent(nodes[7], nodes[6]);
//        adjList.Parent(nodes[8], nodes[7]);
//        adjList.Parent(nodes[8], nodes[6]);
//        adjList.Parent(nodes[9], nodes[5]);
//        adjList.Parent(nodes[10], nodes[9]);
//        adjList.Parent(nodes[11], nodes[9]);

//        //IEnumerable<int> preOrder = adjList.PreOrderTraverse(nodes[0]);
//        //IEnumerable<int> postOrder = adjList.PostOrderTraverse(nodes[0]);

//      }
//    }
//  }

//  //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//  public class AirGraph<TValue> : LinkedAdjList {

//    Dictionary<int, TValue> propertyMap;
//    public List<int> rootNodes;

//    public AirGraph() { }

//    public TValue this[int nodeId] {
//      get => propertyMap[nodeId];
//      set => propertyMap.AddOrAssign(nodeId, value);
//    }
//  }

//  public class AdjacentListWithProperty<TValue> {
//    protected LinkedAdjList adjList = new LinkedAdjList();
//    protected Dictionary<int, TValue> propertyMap;


//    public TValue this[int node] {
//      get => propertyMap[node];
//      set => propertyMap.AddOrAssign(node, value);
//    }
//  }

//  public class AirTree<TValue> : AdjacentListWithProperty<TValue> {

//    public struct Node : IEnumerable<Node>, ITypedNode<TValue> {
//      public AirTree<TValue> tree;
//      public int id;

//      public Node(AirTree<TValue> tree, int id) {
//        this.tree = tree ?? throw new ArgumentNullException(nameof(tree));
//        this.id = id;
//      }

//      private IEnumerable<Node> EachChild {
//        get {
//          foreach (int child in tree.adjList.EachChildOf(id)) {
//            yield return new Node(tree, child);
//          }
//        }
//      }
//      IEnumerable<INode> INode.EachChild => EachChild.AsEnumerable().Cast<INode>();
//      IEnumerable<ITypedNode<TValue>> ITypedNode<TValue>.EachChild => EachChild.AsEnumerable().Cast<ITypedNode<TValue>>();

//      public IEnumerator<Node> GetEnumerator() {
//        return EachChild.GetEnumerator();
//      }
//      IEnumerator IEnumerable.GetEnumerator() {
//        return EachChild.GetEnumerator();
//      }

//      public TValue Value { get => tree[id]; set => tree[id] = value; }
//      TValue ITypedNode<TValue>.Value { get => Value; set => Value = value; }
//      object INode.Value { get => Value; set => Value = (TValue)value; }


//      //============================================================
//      public override string ToString() {
//        return $"node of id {id} of value '{Value}'";
//      }
//      public string About() {
//        var str = new StringBuilder();
//        NestedToString(str, 0);
//        return str.ToString();
//      }
//      private void NestedToString(StringBuilder str, int indentationLevel, bool showId = false) {
//        if (showId) {
//          str.Append($"{indentationLevel.GetIndentationString()}{id}: {Value}\n");
//        }
//        else {
//          str.Append($"{indentationLevel.GetIndentationString()}{Value}\n");
//        }

//        foreach (Node child in EachChild) {
//          child.NestedToString(str, indentationLevel + 1);
//        }
//      }
//      //============================================================

//      public void AddChild(Node newLastChild) {
//        AppendChild(newLastChild);
//      }
//      public void AppendChild(Node newLastChild) {
//        tree.adjList.AppendChild(id, newLastChild.id);
//      }
//      public void PrependChild(Node newFirstChild) {
//        tree.adjList.PrependChild(id, newFirstChild.id);
//      }
//      public void BreakAndRemoveSelf() {
//        tree.adjList.BreakAndRemoveNode(id);
//      }

//      public Node AddChild(TValue value) {
//        int newChildNodeId = tree.adjList.AddNode();
//        tree[newChildNodeId] = value;
//        tree.adjList.Parent(newChildNodeId, id);
//        return new Node(tree, newChildNodeId);
//      }
//      public void RemoveChild(Node childNode) {
//        tree.adjList.Unparent(childNode.id, id);
//      }
//      //============================================================
//      public override int GetHashCode() {
//        return id.GetHashCode();
//      }
//      public override bool Equals(object obj) {
//        return obj is Node node && this.id == node.id;
//      }
//      //============================================================
//    }


//    public Node root;

//    public AirTree(TValue rootValue) {
//      root = new Node(this, adjList.AddNode());
//      propertyMap = new Dictionary<int, TValue> {
//        [root.id] = rootValue
//      };
//    }

//    public override string ToString() {
//      return root.ToString();
//    }

//    //============================================================
//    public IEnumerable<TValue> PreOrderTraverse() => from node in root.PreOrderTraverse()
//                                                     select ((Node)node).Value;

//    public IEnumerable<TValue> DepthFirstTraverse() => from node in root.DepthFirstTraverse()
//                                                       select ((Node)node).Value;
//    //============================================================
//    public static void Test() {
//      var tree = new AirTree<float>(0.0f);

//      AirTree<float>.Node n1 = tree.root.AddChild(1.0f);
//      AirTree<float>.Node n2 = tree.root.AddChild(2.0f);
//      n1.AddChild(11.0f);
//      AirTree<float>.Node n3 = n2.AddChild(3.0f);
//      n2.AddChild(4.0f);
//      AirTree<float>.Node n5 = n2.AddChild(5.0f);
//      AirTree<float>.Node n6 = n3.AddChild(6.0f);
//      n6.AddChild(7.0f);
//      n5.AddChild(8.0f);
//      n5.AddChild(9.0f);
//      n5.AddChild(10.0f);

//      IEnumerable<float> depthFirstResult = tree.DepthFirstTraverse();

//      //IEnumerable<int> preOrder = tree.adjList.PreOrderTraverse(tree.root.id);
//      //IEnumerable<int> postOrder = tree.adjList.PostOrderTraverse(tree.root.id);
//    }
//  }

  

//}



