//using System;
//using System.Collections.Generic;
//using System.Text;
//using AirKuma.AdjList;

//namespace AirKuma {

//  public class AVLTree<T> where T : struct, IComparable<T> {

//    public struct Node : AdjList.ITypedNode<T> {

//      readonly AVLTree<T> Tree;
//      public T key;

//      public Node(AVLTree<T> tree, T key) {
//        Tree = tree;
//        this.key = key;
//      }

//      T ITypedNode<T>.Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
//      object INode.Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

//      IEnumerable<ITypedNode<T>> ITypedNode<T>.EachChild => throw new NotImplementedException();

//      IEnumerable<INode> INode.EachChild => throw new NotImplementedException();

//    }

//    protected Dictionary<T, T> LeftChild { get; set; } = new Dictionary<T, T>();
//    protected Dictionary<T, T> RightChild { get; set; } = new Dictionary<T, T>();
//    protected Dictionary<T, int> Height { get; set; } = new Dictionary<T, int>();

//    protected T rootKey;

//    public Node RootNode => new Node(this, rootKey);

//    public AVLTree(T root) {
//      rootKey = root;
//    }
//    public AVLTree(AVLTree<T> other) {
//      rootKey = other.rootKey;
//      Height = new Dictionary<T, int>(other.Height);
//      LeftChild = new Dictionary<T, T>(other.LeftChild);
//      RightChild = new Dictionary<T, T>(other.RightChild);
//    }
//    //============================================================
//    public IEnumerable<T> EachChildOf(T node) {
//      if (LeftChild.TryGetValue(node, out T lc)) {
//        yield return lc;
//      }
//      if (RightChild.TryGetValue(node, out T rc)) {
//        yield return rc;
//      }
//    }
//    //============================================================
//    int GetHeight(T? node) {
//      if (node.HasValue) {
//        if (Height.TryGetValue(node.Value, out int h)) {
//          return h;
//        }
//      }
//      return 0;
//    }

//    void SetHeight(T node, int value) {
//      if (Height.ContainsKey(node)) {
//        Height[node] = value;
//      }
//      else {
//        Height.Add(node, value);
//      }
//    }

//    void UpdateHeight(T node) {
//      SetHeight(node, GetHeight(LeftChild.NullableGet(node)).Max(GetHeight(RightChild.NullableGet(node))).Add(1));
//    }

//    int GetBalanceFactor(T node) {
//      return GetHeight(RightChild.NullableGet(node)) - GetHeight(LeftChild.NullableGet(node));
//    }
//    int GetBalanceFactor(T? node) {
//      if (node.HasValue)
//        return GetHeight(RightChild.NullableGet(node.Value)) - GetHeight(LeftChild.NullableGet(node.Value));
//      return 0;
//    }

//    //============================================================
//    T NewLeafNode(T key) {
//      Height.Add(key, 1);
//      return key;
//    }
//    //============================================================
//    T RightRotate(T pivot) {
//      T pivotPar = LeftChild.NullableGet(pivot) ?? throw new Exception();
//      T? pivotLeftChild = RightChild.NullableGet(pivotPar);
//      RightChild.NullableSet(pivotPar, pivot);
//      LeftChild.NullableSet(pivot, pivotLeftChild);
//      UpdateHeight(pivot);
//      UpdateHeight(pivotPar);
//      return pivotPar;
//    }
//    T LeftRotate(T pivot) {
//      T pivotPar = RightChild.NullableGet(pivot) ?? throw new Exception();
//      T? pivotRightChild = LeftChild.NullableGet(pivotPar);
//      LeftChild.NullableSet(pivotPar, pivot);
//      RightChild.NullableSet(pivot, pivotRightChild);
//      UpdateHeight(pivot);
//      UpdateHeight(pivotPar);
//      return pivotPar;
//    }
//    //============================================================
//    public bool Contains(T underNode, T node) {
//      int cmp = node.CompareTo(underNode);
//      if (cmp < 0) {
//        if (LeftChild.TryGetValue(underNode, out T lc)) {
//          return Contains(lc, node);
//        }
//        return false;
//      }
//      else if (cmp > 0) {
//        if (RightChild.TryGetValue(underNode, out T rc)) {
//          return Contains(rc, node);
//        }
//        return false;
//      }
//      return true;
//    }
//    public bool Contains(T node) {
//      return Contains(rootKey);
//    }
//    //------------------------------------------------------------
//    public int NodeCountUnder(T underNode) {
//      int c = 1;
//      if (LeftChild.TryGetValue(underNode, out T lc)) {
//        c += NodeCountUnder(lc);
//      }
//      if (RightChild.TryGetValue(underNode, out T rc)) {
//        c += NodeCountUnder(rc);
//      }
//      return c;
//    }
//    public int NodeCount => NodeCountUnder(rootKey);
//    //============================================================
//    public T MinUnder(T underNode) {
//      if (LeftChild.TryGetValue(underNode, out T lc)) {
//        return MinUnder(lc);
//      }
//      return underNode;
//    }
//    public T Min => MinUnder(rootKey);
//    //------------------------------------------------------------
//    public T MaxUnder(T underNode) {
//      if (RightChild.TryGetValue(underNode, out T rc)) {
//        return MaxUnder(rc);
//      }
//      return underNode;
//    }
//    public T Max => MaxUnder(rootKey);
//    //============================================================
//    // returns null if the key is large than all the existing nodes'
//    public T? LowerBound(T? underNode, T node) {
//      if (!underNode.HasValue) {
//        return null;
//      }

//      int cmp = node.CompareTo(underNode.Value);
//      if (cmp < 0) {
//        return LowerBound(LeftChild.NullableGet(underNode.Value), node)
//          ?? underNode;
//      }
//      else if (cmp > 0) {
//        return LowerBound(RightChild.NullableGet(underNode.Value), node);
//      }
//      else {
//        return underNode;
//      }
//    }
//    public T? LowerBound(T node) {
//      return LowerBound(rootKey, node);
//    }
//    //------------------------------------------------------------
//    public T? UpperBound(T? underNode, T node) {
//      if (!underNode.HasValue) {
//        return null;
//      }

//      int cmp = node.CompareTo(underNode.Value);
//      if (cmp < 0) {
//        return UpperBound(LeftChild.NullableGet(underNode.Value), node)
//          ?? underNode;
//      }
//      else {
//        return UpperBound(RightChild.NullableGet(underNode.Value), node);
//      }
//    }
//    public T? UpperBound(T node) {
//      return UpperBound(rootKey, node);
//    }
//    //------------------------------------------------------------
//    public (T? lower, T? upper) LowerAndUpperBound(T node) {
//      return (LowerBound(rootKey, node), UpperBound(rootKey, node));
//    }
//    //============================================================
//    enum BalancingCase {
//      Balanced,
//      LeftLeft,
//      LeftRight,
//      RightLeft,
//      RightRight,
//    }

//    BalancingCase GetInsertionBalancingCase(T node, T grandchild) {
//      int bf = GetBalanceFactor(node);
//      if (bf < -1) {
//        if (grandchild.CompareTo(LeftChild[node]) < 0) {
//          return BalancingCase.LeftLeft;
//        }
//        else {
//          return BalancingCase.LeftRight;
//        }
//      }
//      else if (bf > 1) {
//        if (grandchild.CompareTo(RightChild[node]) < 0) {
//          return BalancingCase.RightLeft;
//        }
//        else {
//          return BalancingCase.RightRight;
//        }
//      }
//      else {
//        return BalancingCase.Balanced;
//      }
//    }

//    T InsertImpl(T? underNode, T theNewNode, bool throwIfKeyIsDuplicated) {
//      if (underNode.HasValue.Not()) {
//        return NewLeafNode(theNewNode);
//      }

//      int cmp = theNewNode.CompareTo(underNode.Value);
//      if (cmp < 0) {
//        LeftChild.AddOrAssign(underNode.Value, InsertImpl(LeftChild.NullableGet(underNode.Value), theNewNode, throwIfKeyIsDuplicated));
//      }
//      else if (cmp > 0) {
//        RightChild.AddOrAssign(underNode.Value, InsertImpl(RightChild.NullableGet(underNode.Value), theNewNode, throwIfKeyIsDuplicated));
//      }
//      else {
//        if (throwIfKeyIsDuplicated) {
//          throw new Exception("the inserting key is already existing");
//        }

//        return underNode.Value;
//      }
//      UpdateHeight(underNode.Value);
//      BalancingCase bCase = GetInsertionBalancingCase(underNode.Value, theNewNode);
//      switch (bCase) {
//        case BalancingCase.LeftLeft:
//          return RightRotate(underNode.Value);
//        case BalancingCase.LeftRight:
//          LeftChild[underNode.Value] = LeftRotate(LeftChild[underNode.Value]);
//          return RightRotate(underNode.Value);
//        case BalancingCase.RightLeft:
//          RightChild[underNode.Value] = RightRotate(RightChild[underNode.Value]);
//          return LeftRotate(underNode.Value);
//        case BalancingCase.RightRight:
//          return LeftRotate(underNode.Value);
//        default:
//          return underNode.Value;
//      }
//    }
//    //public void InsertUnder(T underNode, T theNewNode, bool throwIfKeyIsDuplicated = true) {
//    //  = InsertImpl(underNode, theNewNode, throwIfKeyIsDuplicated);
//    //}
//    public void Insert(T theNewNode, bool throwIfKeyIsDuplicated = true) {
//      rootKey = InsertImpl(rootKey, theNewNode, throwIfKeyIsDuplicated);
//    }

//    //============================================================
//    BalancingCase GetDeletionBalancingCase(T node) {
//      int bf = GetBalanceFactor(node);
//      if (bf < -1) {
//        if (GetBalanceFactor(LeftChild.NullableGet(node)) <= 0) {
//          return BalancingCase.LeftLeft;
//        }
//        else {
//          return BalancingCase.LeftRight;
//        }
//      }
//      else if (bf > 1) {
//        if (GetBalanceFactor(LeftChild.NullableGet(node)) > 0) {
//          return BalancingCase.RightLeft;
//        }
//        else {
//          return BalancingCase.RightRight;
//        }
//      }
//      else {
//        return BalancingCase.Balanced;
//      }
//    }

//    T? UpdateAndRotateForDeletion(T? underNode) {
//      UpdateHeight(underNode.Value);
//      BalancingCase bCase = GetDeletionBalancingCase(underNode.Value);
//      switch (bCase) {
//        case BalancingCase.LeftLeft:
//          return RightRotate(underNode.Value);
//        case BalancingCase.LeftRight:
//          LeftChild[underNode.Value] = LeftRotate(LeftChild[underNode.Value]);
//          return RightRotate(underNode.Value);
//        case BalancingCase.RightLeft:
//          RightChild[underNode.Value] = RightRotate(RightChild[underNode.Value]);
//          return LeftRotate(underNode.Value);
//        case BalancingCase.RightRight:
//          return LeftRotate(underNode.Value);
//        default:
//          return underNode;
//      }
//    }
//    T? DeleteImpl(T? underNode, T existingNode, bool throwIfDeletingNonExistingNode) {
//      if (underNode.HasValue.Not()) {
//        if (throwIfDeletingNonExistingNode)
//          throw new ArgumentException($"attempt to delete a non-existing node {existingNode}");
//        else
//          return null;
//      }
//      int cmp = existingNode.CompareTo(underNode.Value);
//      if (cmp < 0) {
//        LeftChild.NullableSet(underNode.Value, DeleteImpl(LeftChild.NullableGet(underNode.Value), existingNode, throwIfDeletingNonExistingNode));
//        return UpdateAndRotateForDeletion(underNode);
//      }
//      else if (cmp > 0) {
//        RightChild.NullableSet(underNode.Value, DeleteImpl(RightChild.NullableGet(underNode.Value), existingNode, throwIfDeletingNonExistingNode));
//        return UpdateAndRotateForDeletion(underNode);
//      }
//      else {
//        bool hasLeft = LeftChild.TryGetValue(underNode.Value, out T lc);
//        bool hasRight = RightChild.TryGetValue(underNode.Value, out T rc);
//        if (hasLeft && hasRight) {
//          T rightMin = MinUnder(rc);
//          T? rightTreeWithoutRightMin = DeleteImpl(RightChild[underNode.Value], rightMin, throwIfDeletingNonExistingNode);
//          RightChild.NullableSet(rightMin, rightTreeWithoutRightMin);
//          LeftChild.NullableSet(rightMin, LeftChild.NullableGet(underNode.Value));
//          Height.Remove(existingNode);
//          LeftChild.Remove(existingNode);
//          RightChild.Remove(existingNode);
//          return UpdateAndRotateForDeletion(rightMin);
//        }
//        else if (hasLeft || hasRight) {
//          T theOnlyChild = hasLeft ? lc : rc;
//          Height.Remove(existingNode);
//          if (hasLeft)
//            LeftChild.Remove(existingNode);
//          else
//            RightChild.Remove(existingNode);
//          return theOnlyChild;
//        }
//        else {
//          Height.Remove(existingNode);
//          return null;
//        }
//      }
//    }
//    //public void DeleteUnder(T underNode, T existingNodeToDelete) {
//    //  DeleteImpl(underNode, existingNodeToDelete, true);
//    //}
//    public void Delete(T existingNodeToDelete, bool throwIfDeletingNonExistingNode = true) {
//      T? node = DeleteImpl(rootKey, existingNodeToDelete, throwIfDeletingNonExistingNode);
//      node.HasValue.Assert("should has at least one node as root node");
//      rootKey = node.Value;
//    }
//    //============================================================
//    public IEnumerable<T> PreOrderTraverse(T underNode) {
//      yield return underNode;
//      if (LeftChild.TryGetValue(underNode, out T lc)) {
//        foreach (T one in PreOrderTraverse(lc)) {
//          yield return one;
//        }
//      }
//      if (RightChild.TryGetValue(underNode, out T rc)) {
//        foreach (T one in PreOrderTraverse(rc)) {
//          yield return one;
//        }
//      }
//    }
//    public IEnumerable<T> PreOrderTraverse() { return PreOrderTraverse(rootKey); }
//    //------------------------------------------------------------
//    void InOrderListImpl(T underNode, List<T> result) {
//      if (LeftChild.TryGetValue(underNode, out T lc))
//        InOrderListImpl(lc, result);
//      result.Add(underNode);
//      if (RightChild.TryGetValue(underNode, out T rc))
//        InOrderListImpl(rc, result);
//    }
//    public List<T> InOrderList {
//      get {
//        var result = new List<T>();
//        InOrderListImpl(rootKey, result);
//        return result;
//      }
//    }
//    //------------------------------------------------------------
//    public IEnumerable<T> DepthFirstTraverse() {
//      foreach (INode node in RootNode.DepthFirstTraverse()) {
//        yield return ((Node)node).key;
//      }
//    }

//    //============================================================

//    public static AVLTree<char> Test() {
//      var tree = new AVLTree<char>('m');

//      tree.Insert('a');
//      tree.Insert('w');
//      tree.Insert('q');
//      tree.Insert('d');
//      tree.Insert('h');
//      tree.Insert('i');
//      tree.Insert('j');
//      tree.Insert('k');
//      tree.Insert('c');
//      tree.Insert('e');

//      (char? u1, char? u2) = tree.LowerAndUpperBound('j');
//      (char? v1, char? v2) = tree.LowerAndUpperBound('l');

//      return tree;

//    }
//    public static void Test2() {
//      AVLTree<char> tree = Test();

//      var tmp = new AVLTree<char>(tree);
//      tree.Delete('e');
//      tmp = new AVLTree<char>(tree);
//      tree.Delete('h');
//      tmp = new AVLTree<char>(tree);
//      tree.Delete('d');
//      tmp = new AVLTree<char>(tree);
//      tree.Delete('j');
//      tmp = new AVLTree<char>(tree);
//      tree.Delete('a');
//      tmp = new AVLTree<char>(tree);
//      tree.Delete('c');
//      tmp = new AVLTree<char>(tree);
//      tree.Delete('m');
//      tmp = new AVLTree<char>(tree);
//      tree.Delete('i');
//      tmp = new AVLTree<char>(tree);
//      tree.Delete('q');
//      tmp = new AVLTree<char>(tree);
//      tree.Delete('w');

//    }

//    public int BreadthAtDepth(int depth) {
//      depth.GE(0).Assert();
//      return (2).Pow(depth);
//    }
//    public int CountNodeAboveDepth(int exclusivelyAboveDepth) {
//      int c = 0;
//      for (int i = 0; i != exclusivelyAboveDepth; ++i) {
//        c += BreadthAtDepth(i);
//      }
//      return c;
//    }

//    void BracketedRepr(StringBuilder sb, T node) {
//      sb.Append("(");
//      if (LeftChild.TryGetValue(node, out T lc)) {
//        sb.Append(lc.About());
//        BracketedRepr(sb, lc);
//      }
//      else
//        sb.Append("null");
//      sb.Append(",");
//      if (RightChild.TryGetValue(node, out T rc)) {
//        sb.Append(rc.About());
//        BracketedRepr(sb, rc);
//      }
//      else
//        sb.Append("null");
//      sb.Append(')');
//    }
//    public string BracketedRepr() {
//      var sb = new StringBuilder(nameof(AVLTree<T>));
//      BracketedRepr(sb, rootKey);
//      return sb.ToString();
//    }
//    //============================================================
//    public class NodeInfo {

//      public int depth;
//      public int localIndexPerLayer;
//      public int comparisonIndex;

//      public int CompareByDepth(NodeInfo other) {
//        if (depth < other.depth) {
//          return -1;
//        }
//        else if (depth > other.depth) {
//          return 1;
//        }
//        else {
//          if (localIndexPerLayer < other.localIndexPerLayer)
//            return -1;
//          else if (localIndexPerLayer > other.localIndexPerLayer)
//            return 1;
//          else
//            return 0;
//        }
//      }
//    }
//    void NodeInfoMaker(T node, int depth, int localIndex, Dictionary<T, NodeInfo> dict) {
//      dict.Add(node, new NodeInfo {
//        depth = depth,
//        localIndexPerLayer = localIndex,
//      });
//      if (LeftChild.TryGetValue(node, out T lc)) {
//        NodeInfoMaker(lc, depth + 1, localIndex * 2 + 0, dict);
//      }
//      if (RightChild.TryGetValue(node, out T rc)) {
//        NodeInfoMaker(rc, depth + 1, localIndex * 2 + 1, dict);
//      }
//    }
//    public Dictionary<T, NodeInfo> NodeInfoDict {
//      get {
//        var infoDict = new Dictionary<T, NodeInfo>();
//        NodeInfoMaker(rootKey, 0, 0, infoDict);
//        int i = 0;
//        foreach (T node in InOrderList) {
//          infoDict[node].comparisonIndex = i++;
//        }
//        return infoDict;
//      }
//    }
//    public string PrettyTreePrepr {
//      get {
//        Dictionary<T, NodeInfo> infoDict = NodeInfoDict;
//        var str = new StringBuilder("tree");
//        foreach (T node in InOrderList.EachReversed()) {
//          str.AppendLine(infoDict[node].depth.GetIndentationString() + node.About());
//        }
//        //var table = new EasyArr<EasyArr<CharArr>>();
//        //foreach (KeyValuePair<T, NodeInfo> info in NodeInfos)
//        //  table[info.Value.comparisonIndex][info.Value.depth] = info.Key.About();
//        //foreach (EasyArr<CharArr> row in table) {
//        //  foreach (CharArr cell in row) {
//        //    if (cell.Empty)
//        //      str.Append("\t");
//        //    else
//        //      str.Append(cell.ToString());
//        //  }
//        //  str.Append(";\n");
//        //}
//        return str.ToString();
//      }
//    }
//    //============================================================
//    public override string ToString() {
//      return PrettyTreePrepr;
//    }
//  }

//  public class OffsetableAVLTree<T> : AVLTree<T> where T : struct, IOffsetableValue<T> {

//    public OffsetableAVLTree(T root) : base(root) {
//    }

//    public OffsetableAVLTree(AVLTree<T> other) : base(other) {
//    }

//    static bool ShouldOffset(T keyOrVal, T midStart, LeftOrRight leftOrRight) {
//      return keyOrVal.CompareTo(midStart) < 0 && leftOrRight == LeftOrRight.Left
//        || keyOrVal.CompareTo(midStart) > 0 && leftOrRight == LeftOrRight.Right
//        || keyOrVal.CompareTo(midStart) == 0 && leftOrRight == LeftOrRight.Right;
//    }
//    static Dictionary<T, T> OffsetForLeftOrRigtChildDict(Dictionary<T, T> dict, T midStart, LeftOrRight leftOrRight, T offset) {
//      var newDict = new Dictionary<T, T>();
//      foreach (KeyValuePair<T, T> pair in dict) {
//        T newKey = ShouldOffset(pair.Key, midStart, leftOrRight) ? pair.Key.Add(offset) : pair.Key;
//        T newValue = ShouldOffset(pair.Value, midStart, leftOrRight) ? pair.Value.Add(offset) : pair.Value;
//        newDict.Add(newKey, newValue);
//      }
//      return newDict;
//    }
//    static Dictionary<T, int> OffsetForHeightDict(Dictionary<T, int> heightDict, T midStart, LeftOrRight leftOrRight, T offset) {
//      var newDict = new Dictionary<T, int>();
//      foreach (KeyValuePair<T, int> pair in heightDict) {
//        T newKey = ShouldOffset(pair.Key, midStart, leftOrRight) ? pair.Key.Add(offset) : pair.Key;
//        newDict.Add(newKey, pair.Value);
//      }
//      return newDict;
//    }
//    // this invalidates all struct Node
//    public void OffsetUniformly(T midStart, LeftOrRight leftOrRight, T offset) {
//      LeftChild = OffsetForLeftOrRigtChildDict(LeftChild, midStart, leftOrRight, offset);
//      RightChild = OffsetForLeftOrRigtChildDict(RightChild, midStart, leftOrRight, offset);
//      Height = OffsetForHeightDict(Height, midStart, leftOrRight, offset);
//      rootKey = ShouldOffset(rootKey, midStart, leftOrRight) ? rootKey.Add(offset) : rootKey;
//    }

//    public static void Test3() {
//      var tree = new OffsetableAVLTree<IntIndex>(0);
//      tree.Insert(3);
//      tree.Insert(4);
//      tree.Insert(5);
//      tree.Insert(6);
//      tree.Insert(7);
//      tree.OffsetUniformly(3, LeftOrRight.Right, 10);
//    }
//  }

//}