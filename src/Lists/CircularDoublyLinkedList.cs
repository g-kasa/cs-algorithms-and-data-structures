using System.Collections;

namespace Algorithms.Lists;

/// <summary>
/// A circular doubly-linked list backed by a sentinel head node that keeps the ring
/// permanently intact, eliminating null checks in all structural operations.
/// </summary>
/// <typeparam name="T">The element type; must be non-nullable.</typeparam>
/// <remarks>
/// Time:  AddFirst O(1), AddLast O(1), RemoveFirst O(1), RemoveLast O(1),
///        Remove O(n), Contains O(n), Rotate O(steps mod n).
/// Space: O(n) for n elements; O(1) auxiliary for all operations.
/// </remarks>
public class CircularDoublyLinkedList<T> : IEnumerable<T> where T : notnull
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

    /// <summary>Gets the number of elements in the list.</summary>
    public int Count { get; protected set; }

    /// <summary>Initializes an empty list, wiring the sentinel to itself to form a valid empty ring.</summary>
    public CircularDoublyLinkedList()
    {
        _sentinel.Next = _sentinel;
        _sentinel.Prev = _sentinel;
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
    protected void InsertAfter(Node anchor, Node node)
    {
        LinkAfter(anchor, node);
        Count++;
    }

    // Removes a real element node from the ring and decrements Count.
    // The caller guarantees node is never the sentinel.
    protected void Splice(Node node)
    {
        Unlink(node);
        Count--;
    }

    /// <summary>Adds <paramref name="value"/> to the front of the list (immediately after the sentinel).</summary>
    /// <remarks>Time: O(1).</remarks>
    public void AddFirst(T value) => InsertAfter(_sentinel, new Node(value));

    /// <summary>Adds <paramref name="value"/> to the end of the list (immediately before the sentinel).</summary>
    /// <remarks>Time: O(1) — the sentinel's Prev link is a direct reference to the tail.</remarks>
    public void AddLast(T value) => InsertAfter(_sentinel.Prev, new Node(value));

    /// <summary>Removes and returns the value of the first element in the list.</summary>
    /// <returns>The value that was at the front of the list.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the list is empty.</exception>
    /// <remarks>Time: O(1).</remarks>
    public T RemoveFirst()
    {
        if (Count == 0) throw new InvalidOperationException("List is empty.");
        var first = _sentinel.Next;
        Splice(first);
        return first.Value;
    }

    /// <summary>Removes and returns the value of the last element in the list.</summary>
    /// <returns>The value that was at the end of the list.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the list is empty.</exception>
    /// <remarks>Time: O(1).</remarks>
    public T RemoveLast()
    {
        if (Count == 0) throw new InvalidOperationException("List is empty.");
        var last = _sentinel.Prev;
        Splice(last);
        return last.Value;
    }

    /// <summary>Removes the first occurrence of <paramref name="value"/> from the list.</summary>
    /// <returns><c>true</c> if the value was found and removed; otherwise <c>false</c>.</returns>
    /// <remarks>Time: O(n).</remarks>
    public bool Remove(T value)
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

    /// <summary>Determines whether the list contains <paramref name="value"/>.</summary>
    /// <remarks>Time: O(n).</remarks>
    public bool Contains(T value)
    {
        var current = _sentinel.Next;
        while (current != _sentinel)
        {
            if (EqualityComparer<T>.Default.Equals(current.Value, value)) return true;
            current = current.Next;
        }
        return false;
    }

    /// <summary>
    /// Rotates the list left by <paramref name="steps"/> positions.
    /// A negative value rotates right by the absolute number of steps.
    /// The sentinel node is repositioned so that the logical element order as seen
    /// by <see cref="GetEnumerator"/> reflects the rotation.
    /// </summary>
    /// <remarks>
    /// Time: O(steps mod n) — at most n pointer hops regardless of the sign of <paramref name="steps"/>.
    /// A no-op when the list has fewer than two elements or when steps is a multiple of Count.
    /// </remarks>
    public void Rotate(int steps)
    {
        if (Count < 2) return;

        // Normalise into the range [0, Count) so we never walk more than n hops.
        int normalised = steps % Count;
        if (normalised < 0) normalised += Count;
        if (normalised == 0) return;

        // Walk forward to find the node that will sit immediately before the sentinel
        // after the rotation.  After a left-rotation by k the new last element is the
        // one currently at position k-1 (0-indexed from the current first element).
        var newLast = _sentinel.Next;
        for (int i = 1; i < normalised; i++)
            newLast = newLast.Next;

        // Reposition the sentinel: remove it from its current place in the ring (using
        // the raw Unlink helper so Count is unaffected) and splice it back in after newLast.
        Unlink(_sentinel);
        LinkAfter(newLast, _sentinel);
    }

    /// <summary>Returns an enumerator that iterates from the first real element to the last, in forward order.</summary>
    public IEnumerator<T> GetEnumerator()
    {
        var current = _sentinel.Next;
        while (current != _sentinel)
        {
            yield return current.Value;
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
