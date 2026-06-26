namespace Algorithms.Lists;

/// <summary>
/// A circular doubly-linked list that adds three classic in-place sort algorithms.
/// </summary>
/// <typeparam name="T">Element type; must implement <see cref="IComparable{T}"/>.</typeparam>
/// <remarks>
/// Inherits all O(1)/O(n) operations from <see cref="CircularDoublyLinkedList{T}"/>.
/// All sort methods are thread-safe and serialise through the inherited monitor lock.
/// Time:  SelectionSort O(n²), InsertionSort O(n²), MergeSort O(n log n).
/// Space: O(1) auxiliary for SelectionSort and InsertionSort.
///        O(log n) call-stack depth for MergeSort.
/// </remarks>
public sealed class SortableCircularDoublyLinkedList<T> : CircularDoublyLinkedList<T> where T : notnull, IComparable<T>
{
    /// <summary>
    /// Sorts the list in ascending order using the selection sort algorithm.
    /// On each step the minimum value in the unsorted suffix of the ring is found and
    /// swapped into the current position by exchanging node values.
    /// </summary>
    /// <remarks>Time: O(n²). Space: O(1).</remarks>
    public void SelectionSort()
    {
        lock (_syncRoot)
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
        lock (_syncRoot)
        {
            var cursor = _sentinel.Next.Next;
            while (cursor != _sentinel)
            {
                var next = cursor.Next;
                var key = cursor.Value;

                var scan = cursor.Prev;
                while (scan != _sentinel && scan.Value.CompareTo(key) > 0)
                {
                    scan.Next.Value = scan.Value;
                    scan = scan.Prev;
                }

                scan.Next.Value = key;
                cursor = next;
            }
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
        lock (_syncRoot)
        {
            if (Count < 2) return;

            var first = _sentinel.Next;
            var last = _sentinel.Prev;
            first.Prev = null!;
            last.Next = null!;

            first = MergeSortCore(first);

            _sentinel.Next = first;
            first.Prev = _sentinel;

            var current = first;
            while (current.Next is not null)
            {
                current.Next.Prev = current;
                current = current.Next;
            }

            _sentinel.Prev = current;
            current.Next = _sentinel;
        }
    }

    private static Node MergeSortCore(Node head)
    {
        if (head.Next is null) return head;

        var middle = FindMiddle(head);
        var rightHead = middle.Next!;
        middle.Next = null!;

        var sortedLeft = MergeSortCore(head);
        var sortedRight = MergeSortCore(rightHead);

        return Merge(sortedLeft, sortedRight);
    }

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

    private static Node Merge(Node left, Node right)
    {
        var localSentinel = new Node(default(T)!);
        var tail = localSentinel;

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

        return localSentinel.Next!;
    }
}
