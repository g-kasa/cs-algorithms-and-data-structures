using System.Collections;

namespace Algorithms.Lists;

/// <summary>A singly-linked list with O(1) prepend and O(n) traversal operations.</summary>
/// <typeparam name="T">The element type; must implement <see cref="IComparable{T}"/> to support sorting.</typeparam>
/// <remarks>
/// Time:  AddFirst O(1), AddLast O(n), Remove O(n), Contains O(n), Reverse O(n), Sort O(n²).
/// Space: O(n) for n elements; O(1) auxiliary for all operations.
/// </remarks>
public sealed class SinglyLinkedList<T> : IEnumerable<T> where T : IComparable<T>
{
    private sealed class Node(T value, Node? next = null)
    {
        internal T Value = value;
        internal Node? Next = next;
    }

    private Node? _head;

    /// <summary>Gets the number of elements in the list.</summary>
    public int Count { get; private set; }

    /// <summary>Adds <paramref name="value"/> to the front of the list.</summary>
    /// <remarks>Time: O(1).</remarks>
    public void AddFirst(T value)
    {
        _head = new Node(value, _head);
        Count++;
    }

    /// <summary>Adds <paramref name="value"/> to the end of the list.</summary>
    /// <remarks>Time: O(n) — traverses to the tail.</remarks>
    public void AddLast(T value)
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

    /// <summary>Removes the first occurrence of <paramref name="value"/> from the list.</summary>
    /// <returns><c>true</c> if the value was found and removed; otherwise <c>false</c>.</returns>
    /// <remarks>Time: O(n).</remarks>
    public bool Remove(T value)
    {
        if (_head is null) return false;

        if (_head.Value.CompareTo(value) == 0)
        {
            _head = _head.Next;
            Count--;
            return true;
        }

        var current = _head;
        while (current.Next is not null)
        {
            if (current.Next.Value.CompareTo(value) == 0)
            {
                current.Next = current.Next.Next;
                Count--;
                return true;
            }
            current = current.Next;
        }
        return false;
    }

    /// <summary>Determines whether the list contains <paramref name="value"/>.</summary>
    /// <remarks>Time: O(n).</remarks>
    public bool Contains(T value)
    {
        var current = _head;
        while (current is not null)
        {
            if (current.Value.CompareTo(value) == 0) return true;
            current = current.Next;
        }
        return false;
    }

    /// <summary>Reverses the list in place by relinking all nodes.</summary>
    /// <remarks>Time: O(n). Space: O(1).</remarks>
    public void Reverse()
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

    /// <summary>Sorts the list in ascending order using insertion sort.</summary>
    /// <remarks>
    /// Time: O(n²) average and worst. Space: O(1) — relinks nodes, no extra allocation.
    /// Stable: yes (equal elements preserve their original relative order).
    /// </remarks>
    public void Sort()
    {
        if (_head is null || _head.Next is null) return;

        // Start with the first node as the only "sorted" segment.
        var sorted = _head;
        var unsorted = _head.Next;
        sorted.Next = null;

        while (unsorted is not null)
        {
            var next = unsorted.Next;

            if (unsorted.Value.CompareTo(sorted.Value) <= 0)
            {
                // New node belongs at the front of the sorted segment.
                unsorted.Next = sorted;
                sorted = unsorted;
            }
            else
            {
                // Walk the sorted segment to find the insertion point.
                var current = sorted;
                while (current.Next is not null && current.Next.Value.CompareTo(unsorted.Value) < 0)
                    current = current.Next;
                unsorted.Next = current.Next;
                current.Next = unsorted;
            }

            unsorted = next;
        }

        _head = sorted;
    }

    /// <summary>Returns an enumerator that iterates through the list from head to tail.</summary>
    public IEnumerator<T> GetEnumerator()
    {
        var current = _head;
        while (current is not null)
        {
            yield return current.Value;
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
