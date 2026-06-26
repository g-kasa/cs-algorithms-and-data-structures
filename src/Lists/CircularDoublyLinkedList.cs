using System.Collections;

namespace Algorithms.Lists;

/// <summary>
/// A circular doubly-linked list backed by a sentinel head node that keeps the ring
/// permanently intact, eliminating null checks in all structural operations.
/// </summary>
/// <typeparam name="T">The element type; must be non-nullable.</typeparam>
/// <remarks>
/// Implements <see cref="IList{T}"/>; index-based operations (<see cref="this[int]"/>,
/// <see cref="Insert(int,T)"/>, <see cref="RemoveAt"/>, <see cref="IndexOf"/>) are O(n)
/// because the linked structure requires traversal to reach a given position.
/// All public members are thread-safe: every operation serialises through an internal monitor lock.
/// <see cref="GetEnumerator"/> returns an enumerator over a point-in-time snapshot so callers
/// do not need to hold a lock while iterating.
/// Time:  AddFirst O(1), AddLast O(1), RemoveFirst O(1), RemoveLast O(1),
///        Remove O(n), Contains O(n), Rotate O(steps mod n),
///        IndexOf O(n), Insert O(n), RemoveAt O(n), indexer O(n).
/// Space: O(n) for n elements; O(1) auxiliary for all operations except GetEnumerator O(n) snapshot.
/// </remarks>
public class CircularDoublyLinkedList<T> : IList<T> where T : notnull
{
    protected sealed class Node(T value)
    {
        internal T Value = value;
        internal Node Next = null!;
        internal Node Prev = null!;
    }

    // The sentinel is never yielded to callers. Its Value is irrelevant;
    // default(T)! suppresses the nullable warning without a real allocation concern.
    protected readonly Node _sentinel = new(default(T)!);
    protected readonly object _syncRoot = new();

    /// <summary>Gets the number of elements in the list.</summary>
    public int Count { get; protected set; }

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>Initializes an empty list, wiring the sentinel to itself to form a valid empty ring.</summary>
    public CircularDoublyLinkedList()
    {
        _sentinel.Next = _sentinel;
        _sentinel.Prev = _sentinel;
    }

    /// <summary>Gets or sets the element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is out of range.</exception>
    /// <remarks>Time: O(n) — traverses to the node at the given position.</remarks>
    public T this[int index]
    {
        get { lock (_syncRoot) { return NodeAt(index).Value; } }
        set { lock (_syncRoot) { NodeAt(index).Value = value; } }
    }

    // Rewires four pointers to place `node` immediately after `anchor` in the ring.
    // Does not touch Count — callers that deal with real elements manage Count themselves.
    protected static void LinkAfter(Node anchor, Node node)
    {
        node.Next = anchor.Next;
        node.Prev = anchor;
        anchor.Next.Prev = node;
        anchor.Next = node;
    }

    // Rewires two pointers to remove `node` from the ring.
    // Does not touch Count — callers that deal with real elements manage Count themselves.
    protected static void Unlink(Node node)
    {
        node.Prev.Next = node.Next;
        node.Next.Prev = node.Prev;
    }

    // Inserts a real element node immediately after anchor and increments Count.
    // Must be called within a locked section.
    protected void InsertAfter(Node anchor, Node node)
    {
        LinkAfter(anchor, node);
        Count++;
    }

    // Removes a real element node from the ring and decrements Count.
    // The caller guarantees node is never the sentinel.
    // Must be called within a locked section.
    protected void Splice(Node node)
    {
        Unlink(node);
        Count--;
    }

    /// <summary>Adds <paramref name="value"/> to the front of the list (immediately after the sentinel).</summary>
    /// <remarks>Time: O(1).</remarks>
    public void AddFirst(T value)
    {
        lock (_syncRoot) { InsertAfter(_sentinel, new Node(value)); }
    }

    /// <summary>Adds <paramref name="value"/> to the end of the list (immediately before the sentinel).</summary>
    /// <remarks>Time: O(1) — the sentinel's Prev link is a direct reference to the tail.</remarks>
    public void AddLast(T value)
    {
        lock (_syncRoot) { InsertAfter(_sentinel.Prev, new Node(value)); }
    }

    /// <summary>Appends <paramref name="item"/> to the end of the list.</summary>
    /// <remarks>Time: O(1) — delegates to <see cref="AddLast"/>.</remarks>
    public void Add(T item) => AddLast(item);

    /// <summary>Removes all elements from the list.</summary>
    /// <remarks>Time: O(1) — rewires the sentinel to itself.</remarks>
    public void Clear()
    {
        lock (_syncRoot)
        {
            _sentinel.Next = _sentinel;
            _sentinel.Prev = _sentinel;
            Count = 0;
        }
    }

    /// <summary>Removes and returns the value of the first element in the list.</summary>
    /// <returns>The value that was at the front of the list.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the list is empty.</exception>
    /// <remarks>Time: O(1).</remarks>
    public T RemoveFirst()
    {
        lock (_syncRoot)
        {
            if (Count == 0) throw new InvalidOperationException("List is empty.");
            var first = _sentinel.Next;
            Splice(first);
            return first.Value;
        }
    }

    /// <summary>Removes and returns the value of the last element in the list.</summary>
    /// <returns>The value that was at the end of the list.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the list is empty.</exception>
    /// <remarks>Time: O(1).</remarks>
    public T RemoveLast()
    {
        lock (_syncRoot)
        {
            if (Count == 0) throw new InvalidOperationException("List is empty.");
            var last = _sentinel.Prev;
            Splice(last);
            return last.Value;
        }
    }

    /// <summary>Removes the first occurrence of <paramref name="value"/> from the list.</summary>
    /// <returns><c>true</c> if the value was found and removed; otherwise <c>false</c>.</returns>
    /// <remarks>Time: O(n).</remarks>
    public bool Remove(T value)
    {
        lock (_syncRoot)
        {
            var current = _sentinel.Next;
            while (current != _sentinel)
            {
                if (EqualityComparer<T>.Default.Equals(current.Value, value))
                {
                    Splice(current);
                    return true;
                }
                current = current.Next;
            }
            return false;
        }
    }

    /// <summary>Determines whether the list contains <paramref name="value"/>.</summary>
    /// <remarks>Time: O(n).</remarks>
    public bool Contains(T value)
    {
        lock (_syncRoot)
        {
            var current = _sentinel.Next;
            while (current != _sentinel)
            {
                if (EqualityComparer<T>.Default.Equals(current.Value, value)) return true;
                current = current.Next;
            }
            return false;
        }
    }

    /// <summary>Returns the zero-based index of the first occurrence of <paramref name="item"/>, or -1 if not found.</summary>
    /// <remarks>Time: O(n).</remarks>
    public int IndexOf(T item)
    {
        lock (_syncRoot)
        {
            int index = 0;
            var current = _sentinel.Next;
            while (current != _sentinel)
            {
                if (EqualityComparer<T>.Default.Equals(current.Value, item)) return index;
                current = current.Next;
                index++;
            }
            return -1;
        }
    }

    /// <summary>Inserts <paramref name="item"/> at position <paramref name="index"/>, shifting all subsequent elements right.</summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="index"/> is less than 0 or greater than <see cref="Count"/>.
    /// Inserting at <see cref="Count"/> is valid and equivalent to <see cref="Add"/>.
    /// </exception>
    /// <remarks>Time: O(n) — traverses to the insertion point.</remarks>
    public void Insert(int index, T item)
    {
        lock (_syncRoot)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            // Determine the anchor: insert after anchor places the new node at `index`.
            Node anchor;
            if (index == 0) anchor = _sentinel;
            else if (index == Count) anchor = _sentinel.Prev;
            else anchor = NodeAt(index - 1);
            InsertAfter(anchor, new Node(item));
        }
    }

    /// <summary>Removes the element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is less than 0 or greater than or equal to <see cref="Count"/>.</exception>
    /// <remarks>Time: O(n) — traverses to the removal point.</remarks>
    public void RemoveAt(int index)
    {
        lock (_syncRoot) { Splice(NodeAt(index)); }
    }

    /// <summary>Copies all elements to <paramref name="array"/> starting at <paramref name="arrayIndex"/>.</summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="arrayIndex"/> is negative.</exception>
    /// <exception cref="ArgumentException">Thrown when the destination array is too small.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);

        lock (_syncRoot)
        {
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("Destination array is too small.", nameof(array));
            var current = _sentinel.Next;
            while (current != _sentinel) { array[arrayIndex++] = current.Value; current = current.Next; }
        }
    }

    /// <summary>
    /// Rotates the list left by <paramref name="steps"/> positions.
    /// A negative value rotates right by the absolute number of steps.
    /// </summary>
    /// <remarks>
    /// Time: O(steps mod n) — at most n pointer hops regardless of the sign of <paramref name="steps"/>.
    /// A no-op when the list has fewer than two elements or when steps is a multiple of Count.
    /// </remarks>
    public void Rotate(int steps)
    {
        lock (_syncRoot)
        {
            if (Count < 2) return;
            int normalised = steps % Count;
            if (normalised < 0) normalised += Count;
            if (normalised == 0) return;

            var newLast = _sentinel.Next;
            for (int i = 1; i < normalised; i++) newLast = newLast.Next;

            Unlink(_sentinel);
            LinkAfter(newLast, _sentinel);
        }
    }

    /// <summary>Returns an enumerator over a point-in-time snapshot of the list.</summary>
    public IEnumerator<T> GetEnumerator()
    {
        T[] snapshot;
        lock (_syncRoot)
        {
            snapshot = new T[Count];
            int i = 0;
            var current = _sentinel.Next;
            while (current != _sentinel) { snapshot[i++] = current.Value; current = current.Next; }
        }
        return ((IEnumerable<T>)snapshot).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // Traverses to the real node at `index`. Must be called within a locked section.
    private Node NodeAt(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index));
        var current = _sentinel.Next;
        for (int i = 0; i < index; i++) current = current.Next;
        return current;
    }
}
