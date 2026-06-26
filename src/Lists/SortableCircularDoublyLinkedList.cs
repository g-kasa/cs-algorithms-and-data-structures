namespace Algorithms.Lists;

/// <summary>
/// A circular doubly-linked list that adds three classic in-place sort algorithms.
/// </summary>
/// <typeparam name="T">Element type; must implement <see cref="IComparable{T}"/>.</typeparam>
/// <remarks>
/// Inherits all O(1)/O(n) operations from <see cref="CircularDoublyLinkedList{T}"/>.
/// Time:  SelectionSort O(n²), InsertionSort O(n²), MergeSort O(n log n).
/// Space: O(1) auxiliary for SelectionSort and InsertionSort.
///        O(log n) call-stack depth for MergeSort.
/// </remarks>
public sealed class SortableCircularDoublyLinkedList<T> : CircularDoublyLinkedList<T> where T : IComparable<T>
{
    /// <summary>
    /// Sorts the list in ascending order using the selection sort algorithm.
    /// On each step the minimum value in the unsorted suffix of the ring is found and
    /// swapped into the current position by exchanging node values.
    /// </summary>
    /// <remarks>Time: O(n²). Space: O(1).</remarks>
    public void SelectionSort()
    {
        var outer = _sentinel.Next;
        while (outer != _sentinel)
        {
            var minimum = outer;
            var inner = outer.Next;
            while (inner != _sentinel)
            {
                if (inner.Value.CompareTo(minimum.Value) < 0)
                    minimum = inner;
                inner = inner.Next;
            }

            if (minimum != outer)
                (outer.Value, minimum.Value) = (minimum.Value, outer.Value);

            outer = outer.Next;
        }
    }

    /// <summary>
    /// Sorts the list in ascending order using the insertion sort algorithm.
    /// Each unsorted element is walked backwards through the already-sorted prefix
    /// using the <c>Prev</c> links; sorted values are shifted right by value-copy
    /// until the correct gap is found.
    /// </summary>
    /// <remarks>Time: O(n²) average and worst. O(n) best when already sorted. Space: O(1).</remarks>
    public void InsertionSort()
    {
        // When Count <= 1, _sentinel.Next.Next == _sentinel, so the outer loop never runs.
        var cursor = _sentinel.Next.Next;
        while (cursor != _sentinel)
        {
            var next = cursor.Next;
            var key = cursor.Value;

            // Walk backwards through the sorted prefix, shifting each value one position
            // to the right, until we find a node whose value is <= key or we hit the sentinel.
            var scan = cursor.Prev;
            while (scan != _sentinel && scan.Value.CompareTo(key) > 0)
            {
                scan.Next.Value = scan.Value;
                scan = scan.Prev;
            }

            // scan.Next is the gap: either the sentinel's successor, or the node
            // immediately after the last sorted value that was <= key.
            scan.Next.Value = key;

            cursor = next;
        }
    }

    /// <summary>
    /// Sorts the list in ascending order using the merge sort algorithm.
    /// The sentinel is temporarily detached to produce a linear doubly-linked chain,
    /// the chain is sorted recursively, then the sentinel is re-wired to close the ring.
    /// </summary>
    /// <remarks>Time: O(n log n). Space: O(log n) call-stack depth.</remarks>
    public void MergeSort()
    {
        if (Count < 2) return;

        // Step 1: Detach the sentinel to produce a linear doubly-linked chain.
        var first = _sentinel.Next;
        var last = _sentinel.Prev;
        first.Prev = null!;
        last.Next = null!;

        // Step 2: Sort the linear chain.
        first = MergeSortCore(first);

        // Step 3: Re-circularize by wiring the sentinel back as the head/tail anchor.
        _sentinel.Next = first;
        first.Prev = _sentinel;

        // Walk to the sorted tail and fix every Prev pointer that MergeSortCore left
        // pointing to the old linear-chain neighbour rather than the newly merged node.
        var current = first;
        while (current.Next is not null)
        {
            current.Next.Prev = current;
            current = current.Next;
        }

        // current is now the sorted tail.
        _sentinel.Prev = current;
        current.Next = _sentinel;
    }

    // Recursively splits, sorts, and merges the linear subchain rooted at head.
    // head.Prev is assumed to be null (caller has already severed the back-link).
    private static Node MergeSortCore(Node head)
    {
        // A single-node chain is already sorted.
        if (head.Next is null) return head;

        var middle = FindMiddle(head);
        var rightHead = middle.Next!;
        middle.Next = null!; // sever the two halves into independent linear chains

        var sortedLeft = MergeSortCore(head);
        var sortedRight = MergeSortCore(rightHead);

        return Merge(sortedLeft, sortedRight);
    }

    // Returns the last node of the first half of the linear chain using the slow/fast
    // pointer technique. When fast reaches the end, slow is at the midpoint.
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

    // Merges two sorted linear chains into one sorted linear chain by relinking Next pointers.
    // Prev pointers are left in an intermediate state; MergeSort fixes them after the recursion
    // unwinds, walking the completed chain once to set every Prev correctly.
    private static Node Merge(Node left, Node right)
    {
        // A local sentinel avoids a special case for selecting the very first merged node.
        var localSentinel = new Node(default(T)!);
        var tail = localSentinel;

        while (true)
        {
            if (left.Value.CompareTo(right.Value) <= 0)
            {
                // Advance tail before the exhaustion check; otherwise tail.Next = right (below)
                // would overwrite the pointer we just set with tail.Next = left.
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

        return localSentinel.Next!;
    }
}
