namespace Algorithms.Lists;

/// <summary>A singly-linked list that adds in-place sorting algorithms.</summary>
/// <typeparam name="T">Element type; must implement <see cref="IComparable{T}"/>.</typeparam>
/// <remarks>
/// Inherits all O(1)/O(n) operations from <see cref="SinglyLinkedList{T}"/>.
/// All sort methods are thread-safe and serialise through the inherited monitor lock.
/// Time:  BubbleSort O(n²), SelectionSort O(n²), InsertionSort O(n²), MergeSort O(n log n).
/// Space: O(1) auxiliary for all sorts except MergeSort which uses O(log n) stack space.
/// </remarks>
public sealed class SortableSinglyLinkedList<T> : SinglyLinkedList<T> where T : notnull, IComparable<T>
{
    /// <summary>
    /// Sorts the list in ascending order using the bubble sort algorithm.
    /// Adjacent values are swapped each pass; sorting stops early when a pass produces no swaps.
    /// </summary>
    /// <remarks>Time: O(n²) average and worst. O(n) best when already sorted. Space: O(1).</remarks>
    public void BubbleSort()
    {
        lock (_syncRoot)
        {
            if (_head is null || _head.Next is null) return;

            bool swapped;
            do
            {
                swapped = false;
                var current = _head;
                while (current.Next is not null)
                {
                    if (current.Value.CompareTo(current.Next.Value) > 0)
                    {
                        (current.Value, current.Next.Value) = (current.Next.Value, current.Value);
                        swapped = true;
                    }
                    current = current.Next;
                }
            }
            while (swapped);
        }
    }

    /// <summary>
    /// Sorts the list in ascending order using the selection sort algorithm.
    /// On each step the minimum value in the unsorted suffix is found and swapped into position.
    /// </summary>
    /// <remarks>Time: O(n²). Space: O(1).</remarks>
    public void SelectionSort()
    {
        lock (_syncRoot)
        {
            var position = _head;
            while (position is not null)
            {
                var minimumNode = position;
                var candidate = position.Next;
                while (candidate is not null)
                {
                    if (candidate.Value.CompareTo(minimumNode.Value) < 0)
                        minimumNode = candidate;
                    candidate = candidate.Next;
                }

                if (!ReferenceEquals(minimumNode, position))
                    (position.Value, minimumNode.Value) = (minimumNode.Value, position.Value);

                position = position.Next;
            }
        }
    }

    /// <summary>
    /// Sorts the list in ascending order using the insertion sort algorithm.
    /// Nodes are relinked (not value-swapped) to build a sorted prefix one element at a time.
    /// </summary>
    /// <remarks>Time: O(n²) average and worst. O(n) best when already sorted. Space: O(1).</remarks>
    public void InsertionSort()
    {
        lock (_syncRoot)
        {
            if (_head is null || _head.Next is null) return;

            var sorted = _head;
            var unsorted = _head.Next;
            sorted.Next = null;

            while (unsorted is not null)
            {
                var next = unsorted.Next;

                if (unsorted.Value.CompareTo(sorted.Value) <= 0)
                {
                    unsorted.Next = sorted;
                    sorted = unsorted;
                }
                else
                {
                    var walker = sorted;
                    while (walker.Next is not null && walker.Next.Value.CompareTo(unsorted.Value) <= 0)
                        walker = walker.Next;

                    unsorted.Next = walker.Next;
                    walker.Next = unsorted;
                }

                unsorted = next;
            }

            _head = sorted;
        }
    }

    /// <summary>
    /// Sorts the list in ascending order using the merge sort algorithm.
    /// The chain is split at its midpoint, each half is sorted recursively, then the halves are merged.
    /// </summary>
    /// <remarks>Time: O(n log n). Space: O(log n) call-stack depth.</remarks>
    public void MergeSort()
    {
        lock (_syncRoot)
        {
            if (_head is null || _head.Next is null) return;
            _head = MergeSortCore(_head);
        }
    }

    /// <summary>Sorts the list in ascending order. Delegates to <see cref="MergeSort"/>.</summary>
    /// <remarks>Time: O(n log n). Space: O(log n).</remarks>
    public void Sort() => MergeSort();

    // Recursively splits, sorts, and merges the sublist rooted at head.
    private static Node MergeSortCore(Node head)
    {
        if (head.Next is null) return head;

        var middle = FindMiddle(head);
        var rightHead = middle.Next!;
        middle.Next = null;

        var sortedLeft = MergeSortCore(head);
        var sortedRight = MergeSortCore(rightHead);

        return Merge(sortedLeft, sortedRight);
    }

    // Returns the last node of the first half using the slow/fast pointer technique.
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

    // Merges two sorted chains into one sorted chain by relinking Next pointers.
    private static Node Merge(Node left, Node right)
    {
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
