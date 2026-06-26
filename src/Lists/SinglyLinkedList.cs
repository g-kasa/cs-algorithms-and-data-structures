using System.Collections;

namespace Algorithms.Lists;

/// <summary>A singly-linked list with O(1) prepend and O(n) traversal operations.</summary>
/// <typeparam name="T">The element type; must be non-nullable.</typeparam>
/// <remarks>
/// Time:  AddFirst O(1), AddLast O(n), Remove O(n), Contains O(n), Reverse O(n).
/// Space: O(n) for n elements; O(1) auxiliary for all operations.
/// </remarks>
public class SinglyLinkedList<T> : IEnumerable<T> where T : notnull
{
    protected sealed class Node(T value, Node? next = null)
    {
        internal T Value = value;
        internal Node? Next = next;
    }

    protected Node? _head;

    /// <summary>Gets the number of elements in the list.</summary>
    public int Count { get; protected set; }

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

    /// <summary>Determines whether the list contains <paramref name="value"/>.</summary>
    /// <remarks>Time: O(n).</remarks>
    public bool Contains(T value)
    {
        var current = _head;
        while (current is not null)
        {
            if (EqualityComparer<T>.Default.Equals(current.Value, value)) return true;
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
