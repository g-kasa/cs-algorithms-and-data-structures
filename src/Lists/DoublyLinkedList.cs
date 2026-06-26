using System.Collections;

namespace Algorithms.Lists;

/// <summary>A doubly-linked list with O(1) prepend, append, and end-removal operations.</summary>
/// <typeparam name="T">The element type; must be non-nullable.</typeparam>
/// <remarks>
/// Implements <see cref="IList{T}"/>; index-based operations (<see cref="this[int]"/>,
/// <see cref="Insert(int,T)"/>, <see cref="RemoveAt"/>, <see cref="IndexOf"/>) are O(n)
/// because the linked structure requires traversal to reach a given position.
/// Traversal is optimised: indices in the first half walk forward from the head; indices in the
/// second half walk backward from the tail, so the worst case is O(n/2).
/// All public members are thread-safe: every operation serialises through an internal monitor lock.
/// <see cref="GetEnumerator"/> returns an enumerator over a point-in-time snapshot so callers
/// do not need to hold a lock while iterating.
/// Time:  AddFirst O(1), AddLast O(1), RemoveFirst O(1), RemoveLast O(1),
///        Remove O(n), Contains O(n), Reverse O(n),
///        IndexOf O(n), Insert O(n), RemoveAt O(n), indexer O(n).
/// Space: O(n) for n elements; O(1) auxiliary for all operations except GetEnumerator O(n) snapshot.
/// </remarks>
public class DoublyLinkedList<T> : IList<T> where T : notnull
{
    protected sealed class Node(T value, Node? next = null, Node? previous = null)
    {
        internal T Value = value;
        internal Node? Next = next;
        internal Node? Previous = previous;
    }

    protected Node? _head;
    protected Node? _tail;
    protected readonly object _syncRoot = new();

    /// <summary>Gets the number of elements in the list.</summary>
    public int Count { get; protected set; }

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>Gets or sets the element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is out of range.</exception>
    /// <remarks>Time: O(n) — traverses at most n/2 nodes due to bidirectional search.</remarks>
    public T this[int index]
    {
        get { lock (_syncRoot) { return NodeAt(index).Value; } }
        set { lock (_syncRoot) { NodeAt(index).Value = value; } }
    }

    /// <summary>Adds <paramref name="value"/> to the front of the list.</summary>
    /// <remarks>Time: O(1).</remarks>
    public void AddFirst(T value)
    {
        lock (_syncRoot)
        {
            var node = new Node(value, next: _head);
            if (_head is null) _tail = node;
            else _head.Previous = node;
            _head = node;
            Count++;
        }
    }

    /// <summary>Adds <paramref name="value"/> to the end of the list.</summary>
    /// <remarks>Time: O(1) — maintained by a tail pointer.</remarks>
    public void AddLast(T value)
    {
        lock (_syncRoot)
        {
            var node = new Node(value, previous: _tail);
            if (_tail is null) _head = node;
            else _tail.Next = node;
            _tail = node;
            Count++;
        }
    }

    /// <summary>Appends <paramref name="item"/> to the end of the list.</summary>
    /// <remarks>Time: O(1) — delegates to <see cref="AddLast"/>.</remarks>
    public void Add(T item) => AddLast(item);

    /// <summary>Removes all elements from the list.</summary>
    /// <remarks>Time: O(1).</remarks>
    public void Clear()
    {
        lock (_syncRoot) { _head = null; _tail = null; Count = 0; }
    }

    /// <summary>Removes and returns the value at the front of the list.</summary>
    /// <returns>The value that was at the head of the list.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the list is empty.</exception>
    /// <remarks>Time: O(1).</remarks>
    public T RemoveFirst()
    {
        lock (_syncRoot)
        {
            if (_head is null) throw new InvalidOperationException("List is empty.");
            var value = _head.Value;
            _head = _head.Next;
            if (_head is null) _tail = null;
            else _head.Previous = null;
            Count--;
            return value;
        }
    }

    /// <summary>Removes and returns the value at the end of the list.</summary>
    /// <returns>The value that was at the tail of the list.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the list is empty.</exception>
    /// <remarks>Time: O(1).</remarks>
    public T RemoveLast()
    {
        lock (_syncRoot)
        {
            if (_tail is null) throw new InvalidOperationException("List is empty.");
            var value = _tail.Value;
            _tail = _tail.Previous;
            if (_tail is null) _head = null;
            else _tail.Next = null;
            Count--;
            return value;
        }
    }

    /// <summary>Removes the first occurrence of <paramref name="value"/> from the list.</summary>
    /// <returns><c>true</c> if the value was found and removed; otherwise <c>false</c>.</returns>
    /// <remarks>Time: O(n).</remarks>
    public bool Remove(T value)
    {
        lock (_syncRoot)
        {
            var current = _head;
            while (current is not null)
            {
                if (EqualityComparer<T>.Default.Equals(current.Value, value))
                {
                    Unlink(current);
                    Count--;
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
            var current = _head;
            while (current is not null)
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
            var current = _head;
            while (current is not null)
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
    /// <remarks>Time: O(n) — traverses at most n/2 nodes.</remarks>
    public void Insert(int index, T item)
    {
        lock (_syncRoot)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            var node = new Node(item);

            if (_head is null || index == Count)
            {
                node.Previous = _tail;
                if (_tail is not null) _tail.Next = node;
                else _head = node;
                _tail = node;
            }
            else if (index == 0)
            {
                node.Next = _head;
                _head.Previous = node;
                _head = node;
            }
            else
            {
                var successor = NodeAt(index);
                var predecessor = successor.Previous!;
                node.Next = successor;
                node.Previous = predecessor;
                predecessor.Next = node;
                successor.Previous = node;
            }

            Count++;
        }
    }

    /// <summary>Removes the element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is less than 0 or greater than or equal to <see cref="Count"/>.</exception>
    /// <remarks>Time: O(n) — traverses at most n/2 nodes.</remarks>
    public void RemoveAt(int index)
    {
        lock (_syncRoot)
        {
            Unlink(NodeAt(index));
            Count--;
        }
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
            var current = _head;
            while (current is not null) { array[arrayIndex++] = current.Value; current = current.Next; }
        }
    }

    /// <summary>Reverses the list in place by swapping each node's <c>Next</c> and <c>Previous</c> pointers.</summary>
    /// <remarks>Time: O(n). Space: O(1).</remarks>
    public void Reverse()
    {
        lock (_syncRoot)
        {
            var current = _head;
            _head = _tail;
            _tail = current;

            while (current is not null)
            {
                var next = current.Next;
                current.Next = current.Previous;
                current.Previous = next;
                current = next;
            }
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
            var current = _head;
            while (current is not null) { snapshot[i++] = current.Value; current = current.Next; }
        }
        return ((IEnumerable<T>)snapshot).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // Removes a known node by stitching its neighbours together. Does not update Count.
    private void Unlink(Node node)
    {
        if (node.Previous is not null) node.Previous.Next = node.Next;
        else _head = node.Next;

        if (node.Next is not null) node.Next.Previous = node.Previous;
        else _tail = node.Previous;
    }

    // Traverses to the node at `index` using bidirectional search. Must be called within a locked section.
    private Node NodeAt(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (index <= Count / 2)
        {
            var current = _head;
            for (int i = 0; i < index; i++) current = current!.Next;
            return current!;
        }
        else
        {
            var current = _tail;
            for (int i = Count - 1; i > index; i--) current = current!.Previous;
            return current!;
        }
    }
}
