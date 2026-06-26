namespace Algorithms.Trees;

/// <summary>
/// A binary search tree that maintains the BST invariant: every left child is smaller
/// than its parent and every right child is larger.
/// </summary>
/// <typeparam name="T">The element type; must implement <see cref="IComparable{T}"/>.</typeparam>
/// <remarks>
/// Time:  Insert O(log n) average, O(n) worst (degenerate tree).
///        Contains O(log n) average, O(n) worst.
///        Remove O(log n) average, O(n) worst.
///        InOrder / PreOrder / PostOrder / LevelOrder O(n).
/// Space: O(n) for n elements; O(h) auxiliary stack depth where h is the tree height.
/// </remarks>
public sealed class BinarySearchTree<T> where T : IComparable<T>
{
    private sealed class Node(T value)
    {
        internal T Value = value;
        internal Node? Left;
        internal Node? Right;
    }

    private Node? _root;

    /// <summary>Gets the number of elements in the tree.</summary>
    public int Count { get; private set; }

    /// <summary>Inserts <paramref name="value"/> into the tree. Duplicates are ignored.</summary>
    public void Insert(T value)
    {
        if (_root is null)
        {
            _root = new Node(value);
            Count++;
            return;
        }

        var current = _root;
        while (true)
        {
            var comparison = value.CompareTo(current.Value);
            if (comparison < 0)
            {
                if (current.Left is null) { current.Left = new Node(value); Count++; return; }
                current = current.Left;
            }
            else if (comparison > 0)
            {
                if (current.Right is null) { current.Right = new Node(value); Count++; return; }
                current = current.Right;
            }
            else
            {
                return; // duplicate — BST stores each value once
            }
        }
    }

    /// <summary>Determines whether the tree contains <paramref name="value"/>.</summary>
    public bool Contains(T value)
    {
        var current = _root;
        while (current is not null)
        {
            var comparison = value.CompareTo(current.Value);
            if (comparison < 0) current = current.Left;
            else if (comparison > 0) current = current.Right;
            else return true;
        }
        return false;
    }

    /// <summary>Removes <paramref name="value"/> from the tree.</summary>
    /// <returns><c>true</c> if the value was found and removed; otherwise <c>false</c>.</returns>
    public bool Remove(T value)
    {
        _root = RemoveFrom(_root, value, out bool removed);
        if (removed) Count--;
        return removed;
    }

    private static Node? RemoveFrom(Node? node, T value, out bool removed)
    {
        if (node is null) { removed = false; return null; }

        var comparison = value.CompareTo(node.Value);

        if (comparison < 0)
        {
            node.Left = RemoveFrom(node.Left, value, out removed);
        }
        else if (comparison > 0)
        {
            node.Right = RemoveFrom(node.Right, value, out removed);
        }
        else
        {
            removed = true;

            // Node with zero or one child — just replace with the non-null child (or null).
            if (node.Left is null) return node.Right;
            if (node.Right is null) return node.Left;

            // Node with two children: replace value with the in-order successor
            // (smallest node in the right subtree), then delete the successor.
            var successor = node.Right;
            while (successor.Left is not null)
                successor = successor.Left;

            node.Value = successor.Value;
            node.Right = RemoveFrom(node.Right, successor.Value, out _);
        }

        return node;
    }

    /// <summary>Visits every element in ascending order (left → root → right).</summary>
    public IEnumerable<T> InOrder() => Traverse(_root, Order.In);

    /// <summary>Visits the root before its subtrees (root → left → right).</summary>
    public IEnumerable<T> PreOrder() => Traverse(_root, Order.Pre);

    /// <summary>Visits the root after its subtrees (left → right → root).</summary>
    public IEnumerable<T> PostOrder() => Traverse(_root, Order.Post);

    /// <summary>Visits every element level by level, top to bottom (breadth-first).</summary>
    public IEnumerable<T> LevelOrder()
    {
        if (_root is null) yield break;

        var queue = new Queue<Node>();
        queue.Enqueue(_root);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            yield return node.Value;
            if (node.Left is not null) queue.Enqueue(node.Left);
            if (node.Right is not null) queue.Enqueue(node.Right);
        }
    }

    private enum Order { Pre, In, Post }

    private static IEnumerable<T> Traverse(Node? node, Order order)
    {
        if (node is null) yield break;
        if (order == Order.Pre) yield return node.Value;
        foreach (var value in Traverse(node.Left, order)) yield return value;
        if (order == Order.In) yield return node.Value;
        foreach (var value in Traverse(node.Right, order)) yield return value;
        if (order == Order.Post) yield return node.Value;
    }
}
