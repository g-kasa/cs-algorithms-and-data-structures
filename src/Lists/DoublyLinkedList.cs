using System.Collections;

namespace Algorithms.Lists;

/// <summary>A doubly-linked list with O(1) prepend, append, and end-removal operations.</summary>
/// <typeparam name="T">The element type; must implement <see cref="IComparable{T}"/> to support sorting.</typeparam>
/// <remarks>
/// Time:  AddFirst O(1), AddLast O(1), RemoveFirst O(1), RemoveLast O(1),
///        Remove O(n), Contains O(n), Reverse O(n), Sort O(n log n).
/// Space: O(n) for n elements; O(log n) auxiliary for Sort (recursion stack), O(1) for all other operations.
/// </remarks>
public sealed class DoublyLinkedList<T> : IEnumerable<T> where T : IComparable<T>
{
    private sealed class Node(T value, Node? next = null, Node? previous = null)
    {
        internal T Value = value;
        internal Node? Next = next;
        internal Node? Previous = previous;
    }

    private Node? _head;
    private Node? _tail;

    /// <summary>Gets the number of elements in the list.</summary>
    public int Count { get; private set; }

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
            if (current.Value.CompareTo(value) == 0)
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
            if (current.Value.CompareTo(value) == 0) return true;
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

    /// <summary>Sorts the list in ascending order using merge sort on the node links.</summary>
    /// <remarks>
    /// Time: O(n log n) average and worst. Space: O(log n) — recursion stack depth; no array allocation.
    /// Stable: yes (equal elements preserve their original relative order).
    /// </remarks>
    public void Sort()
    {
        if (_head is null || _head.Next is null) return;

        _head = MergeSort(_head);

        // After relinking, rebuild the tail pointer and restore all Previous links.
        var current = _head;
        _head.Previous = null;
        while (current.Next is not null)
        {
            current.Next.Previous = current;
            current = current.Next;
        }
        _tail = current;
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

    // Returns the head of a sorted list built from the chain starting at <paramref name="head"/>.
    // Previous pointers are not maintained during recursion; Sort() restores them in one pass.
    private static Node MergeSort(Node head)
    {
        if (head.Next is null) return head;

        var middle = FindMiddle(head);
        var secondHalf = middle.Next!;
        // Sever the link so both halves are independent chains.
        middle.Next = null;

        var left = MergeSort(head);
        var right = MergeSort(secondHalf);
        return Merge(left, right);
    }

    // Finds the middle node using slow/fast pointers (floor of n/2 for even-length lists).
    private static Node FindMiddle(Node head)
    {
        var slow = head;
        var fast = head.Next;
        while (fast is not null && fast.Next is not null)
        {
            slow = slow.Next!;
            fast = fast.Next.Next;
        }
        return slow;
    }

    // Merges two sorted chains and returns the head of the merged chain.
    private static Node Merge(Node left, Node right)
    {
        // Use a sentinel to avoid a special case for the first merged node.
        var sentinel = new Node(default!);
        var tail = sentinel;

        while (true)
        {
            if (left.Value.CompareTo(right.Value) <= 0)
            {
                tail.Next = left;
                tail = left;
                left = left.Next!;
                if (left is null) { tail.Next = right; break; }
            }
            else
            {
                tail.Next = right;
                tail = right;
                right = right.Next!;
                if (right is null) { tail.Next = left; break; }
            }
        }

        return sentinel.Next!;
    }
}
