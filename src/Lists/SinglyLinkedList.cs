using System.Collections;

namespace Algorithms.Lists;

/// <summary>A singly-linked list with O(1) prepend and O(n) traversal operations.</summary>
/// <typeparam name="T">The element type; must be non-nullable.</typeparam>
/// <remarks>
/// Implements <see cref="IList{T}"/>; index-based operations (<see cref="this[int]"/>,
/// <see cref="Insert(int,T)"/>, <see cref="RemoveAt"/>, <see cref="IndexOf"/>) are O(n)
/// because the linked structure requires traversal to reach a given position.
/// All public members are thread-safe: every operation serialises through an internal monitor lock.
/// <see cref="GetEnumerator"/> returns an enumerator over a point-in-time snapshot so callers
/// do not need to hold a lock while iterating.
/// Time:  AddFirst O(1), AddLast O(n), Remove O(n), Contains O(n), Reverse O(n),
///        IndexOf O(n), Insert O(n), RemoveAt O(n), indexer O(n).
/// Space: O(n) for n elements; O(1) auxiliary for all operations except GetEnumerator O(n) snapshot.
/// </remarks>
public class SinglyLinkedList<T> : IList<T> where T : notnull
{
    protected sealed class Node(T value, Node? next = null)
    {
        internal T Value = value;
        internal Node? Next = next;
    }

    protected Node? _head;
    protected readonly object _syncRoot = new();

    private int _count;

    /// <summary>Gets the number of elements in the list.</summary>
    public int Count
    {
        get => Volatile.Read(ref _count);
        protected set => Volatile.Write(ref _count, value);
    }

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>Gets or sets the element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is out of range.</exception>
    /// <remarks>Time: O(n) — traverses to the node at the given position.</remarks>
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
            _head = new Node(value, _head);
            Count++;
        }
    }

    /// <summary>Adds <paramref name="value"/> to the end of the list.</summary>
    /// <remarks>Time: O(n) — traverses to the tail.</remarks>
    public void AddLast(T value)
    {
        lock (_syncRoot)
        {
            var node = new Node(value);
            if (_head is null)
            {
                _head = node;
            }
            else
            {
                var current = _head;
                while (current.Next is not null)
                    current = current.Next;
                current.Next = node;
            }
            Count++;
        }
    }

    /// <summary>Appends <paramref name="item"/> to the end of the list.</summary>
    /// <remarks>Time: O(n) — delegates to <see cref="AddLast"/>.</remarks>
    public void Add(T item) => AddLast(item);

    /// <summary>Removes all elements from the list.</summary>
    /// <remarks>Time: O(1).</remarks>
    public void Clear()
    {
        lock (_syncRoot) { _head = null; Count = 0; }
    }

    /// <summary>Removes the first occurrence of <paramref name="value"/> from the list.</summary>
    /// <returns><c>true</c> if the value was found and removed; otherwise <c>false</c>.</returns>
    /// <remarks>Time: O(n).</remarks>
    public bool Remove(T value)
    {
        lock (_syncRoot)
        {
            if (_head is null) return false;

            if (EqualityComparer<T>.Default.Equals(_head.Value, value))
            {
                _head = _head.Next;
                Count--;
                return true;
            }

            var current = _head;
            while (current.Next is not null)
            {
                if (EqualityComparer<T>.Default.Equals(current.Next.Value, value))
                {
                    current.Next = current.Next.Next;
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
    /// <remarks>Time: O(n) — traverses to the insertion point.</remarks>
    public void Insert(int index, T item)
    {
        lock (_syncRoot)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (index == 0) { _head = new Node(item, _head); Count++; return; }
            var current = _head;
            for (int i = 0; i < index - 1; i++) current = current!.Next;
            current!.Next = new Node(item, current.Next);
            Count++;
        }
    }

    /// <summary>Removes the element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is less than 0 or greater than or equal to <see cref="Count"/>.</exception>
    /// <remarks>Time: O(n) — traverses to the removal point.</remarks>
    public void RemoveAt(int index)
    {
        lock (_syncRoot)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (index == 0) { _head = _head!.Next; Count--; return; }
            var current = _head;
            for (int i = 0; i < index - 1; i++) current = current!.Next;
            current!.Next = current.Next!.Next;
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

    /// <summary>Reverses the list in place by relinking all nodes.</summary>
    /// <remarks>Time: O(n). Space: O(1).</remarks>
    public void Reverse()
    {
        lock (_syncRoot)
        {
            Node? previous = null;
            var current = _head;
            while (current is not null)
            {
                var next = current.Next;
                current.Next = previous;
                previous = current;
                current = next;
            }
            _head = previous;
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

    // Traverses to the node at `index`. Must be called from within a locked section.
    private Node NodeAt(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index));
        var current = _head;
        for (int i = 0; i < index; i++) current = current!.Next;
        return current!;
    }
}
