using System.Collections;

namespace Algorithms.Lists;

/// <summary>A doubly-linked list with O(1) prepend, append, and end-removal operations.</summary>
/// <typeparam name="T">The element type; must be non-nullable.</typeparam>
/// <remarks>
/// Time:  AddFirst O(1), AddLast O(1), RemoveFirst O(1), RemoveLast O(1),
///        Remove O(n), Contains O(n), Reverse O(n).
/// Space: O(n) for n elements; O(1) auxiliary for all operations.
/// </remarks>
public class DoublyLinkedList<T> : IEnumerable<T> where T : notnull
{
    protected sealed class Node(T value, Node? next = null, Node? previous = null)
    {
        internal T Value = value;
        internal Node? Next = next;
        internal Node? Previous = previous;
    }

    protected Node? _head;
    protected Node? _tail;

    /// <summary>Gets the number of elements in the list.</summary>
    public int Count { get; protected set; }

    /// <summary>Adds <paramref name="value"/> to the front of the list.</summary>
    /// <remarks>Time: O(1).</remarks>
    public void AddFirst(T value)
    {
        var node = new Node(value, next: _head);
        if (_head is null)
        {
            _tail = node;
        }
        else
        {
            _head.Previous = node;
        }
        _head = node;
        Count++;
    }

    /// <summary>Adds <paramref name="value"/> to the end of the list.</summary>
    /// <remarks>Time: O(1) — maintained by a tail pointer.</remarks>
    public void AddLast(T value)
    {
        var node = new Node(value, previous: _tail);
        if (_tail is null)
        {
            _head = node;
        }
        else
        {
            _tail.Next = node;
        }
        _tail = node;
        Count++;
    }

    /// <summary>Removes and returns the value at the front of the list.</summary>
    /// <returns>The value that was at the head of the list.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the list is empty.</exception>
    /// <remarks>Time: O(1).</remarks>
    public T RemoveFirst()
    {
        if (_head is null) throw new InvalidOperationException("List is empty.");

        var value = _head.Value;
        _head = _head.Next;
        if (_head is null)
        {
            _tail = null;
        }
        else
        {
            _head.Previous = null;
        }
        Count--;
        return value;
    }

    /// <summary>Removes and returns the value at the end of the list.</summary>
    /// <returns>The value that was at the tail of the list.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the list is empty.</exception>
    /// <remarks>Time: O(1).</remarks>
    public T RemoveLast()
    {
        if (_tail is null) throw new InvalidOperationException("List is empty.");

        var value = _tail.Value;
        _tail = _tail.Previous;
        if (_tail is null)
        {
            _head = null;
        }
        else
        {
            _tail.Next = null;
        }
        Count--;
        return value;
    }

    /// <summary>Removes the first occurrence of <paramref name="value"/> from the list.</summary>
    /// <returns><c>true</c> if the value was found and removed; otherwise <c>false</c>.</returns>
    /// <remarks>Time: O(n).</remarks>
    public bool Remove(T value)
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

    /// <summary>Reverses the list in place by swapping each node's <c>Next</c> and <c>Previous</c> pointers.</summary>
    /// <remarks>Time: O(n). Space: O(1).</remarks>
    public void Reverse()
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

    // Removes a known node by stitching its neighbours together.
    private void Unlink(Node node)
    {
        if (node.Previous is not null)
            node.Previous.Next = node.Next;
        else
            _head = node.Next;

        if (node.Next is not null)
            node.Next.Previous = node.Previous;
        else
            _tail = node.Previous;
    }

}
