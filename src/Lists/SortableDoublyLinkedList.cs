namespace Algorithms.Lists;

/// <summary>
/// A doubly-linked list that extends <see cref="DoublyLinkedList{T}"/> with four classic
/// in-place sorting algorithms: Bubble Sort, Selection Sort, Insertion Sort, and Merge Sort.
/// </summary>
/// <typeparam name="T">
/// The element type; must implement <see cref="IComparable{T}"/> because all four algorithms
/// rely on a total order to determine the sort sequence.
/// </typeparam>
/// <remarks>
/// Inherits all base operations (AddFirst, AddLast, RemoveFirst, RemoveLast, Remove, Contains,
/// Reverse) at their original complexities. All sorts are ascending and operate in place.
///
/// Time:  BubbleSort O(n²) average/worst, O(n) best (already sorted).
///        SelectionSort O(n²) always.
///        InsertionSort O(n²) average/worst, O(n) best (already sorted).
///        MergeSort O(n log n) always.
/// Space: BubbleSort O(1) auxiliary. SelectionSort O(1) auxiliary.
///        InsertionSort O(1) auxiliary. MergeSort O(log n) auxiliary (recursion stack).
/// </remarks>
public sealed class SortableDoublyLinkedList<T> : DoublyLinkedList<T> where T : IComparable<T>
{
    /// <summary>
    /// Sorts the list in ascending order using Bubble Sort. Adjacent nodes whose values
    /// are out of order have their values swapped. The algorithm stops as soon as a full
    /// pass completes without a swap, so an already-sorted list costs O(n).
    /// </summary>
    /// <remarks>
    /// Time: O(n²) average and worst case; O(n) best case (already sorted).
    /// Space: O(1) auxiliary.
    /// </remarks>
    public void BubbleSort()
    {
        if (_head is null || _head.Next is null) return;

        // unsortedTail marks the boundary between the unsorted prefix and the sorted
        // suffix that has already bubbled into place. It retreats one node per pass.
        Node? unsortedTail = null;

        while (true)
        {
            bool swapped = false;
            var current = _head;

            while (current.Next is not null && current.Next != unsortedTail)
            {
                if (current.Value.CompareTo(current.Next.Value) > 0)
                {
                    (current.Value, current.Next.Value) = (current.Next.Value, current.Value);
                    swapped = true;
                }
                current = current.Next;
            }

            // After this pass, `current` is the last node we visited — it now holds the
            // largest value in the unsorted region and belongs in the sorted suffix.
            unsortedTail = current;

            if (!swapped) break;
        }
    }

    /// <summary>
    /// Sorts the list in ascending order using Selection Sort. For each position the
    /// algorithm scans the remainder of the list for the minimum value and swaps it
    /// into the current position.
    /// </summary>
    /// <remarks>
    /// Time: O(n²) always — the inner scan always runs to the end of the unsorted region.
    /// Space: O(1) auxiliary.
    /// </remarks>
    public void SelectionSort()
    {
        var cursor = _head;
        while (cursor is not null)
        {
            var minimumNode = cursor;
            var scanner = cursor.Next;

            while (scanner is not null)
            {
                if (scanner.Value.CompareTo(minimumNode.Value) < 0)
                    minimumNode = scanner;
                scanner = scanner.Next;
            }

            if (!ReferenceEquals(minimumNode, cursor))
                (cursor.Value, minimumNode.Value) = (minimumNode.Value, cursor.Value);

            cursor = cursor.Next;
        }
    }

    /// <summary>
    /// Sorts the list in ascending order using Insertion Sort. Each unsorted node is
    /// compared against the already-sorted prefix using <c>Previous</c> links. Values
    /// are shifted one slot to the right until the correct insertion position is found;
    /// no nodes are relinked.
    /// </summary>
    /// <remarks>
    /// Time: O(n²) average and worst case; O(n) best case (already sorted).
    /// Space: O(1) auxiliary.
    /// </remarks>
    public void InsertionSort()
    {
        if (_head is null || _head.Next is null) return;

        var cursor = _head.Next;
        while (cursor is not null)
        {
            // Capture the next unsorted node before we overwrite cursor.Value.
            var nextCursor = cursor.Next;
            var key = cursor.Value;

            // Walk backwards through the sorted prefix, shifting each value one node
            // to the right to open a slot for `key`.
            var scan = cursor.Previous;
            while (scan is not null && scan.Value.CompareTo(key) > 0)
            {
                scan.Next!.Value = scan.Value;
                scan = scan.Previous;
            }

            // scan is null  →  key is smaller than every sorted element; write into _head.
            // scan is not null  →  scan.Next is the correct slot (scan.Value ≤ key).
            (scan is null ? _head! : scan.Next!).Value = key;

            cursor = nextCursor;
        }
    }

    /// <summary>
    /// Sorts the list in ascending order using Merge Sort. The list is recursively split
    /// at the midpoint, each half is sorted, and the halves are merged. All <c>Previous</c>
    /// links and the <c>_tail</c> pointer are rebuilt in a single forward pass after the
    /// recursive sort completes.
    /// </summary>
    /// <remarks>
    /// Time: O(n log n) always — halving produces O(log n) levels; each level merges O(n) nodes.
    /// Space: O(log n) auxiliary for the recursion stack.
    /// </remarks>
    public void MergeSort()
    {
        if (_head is null || _head.Next is null) return;

        _head = MergeSortCore(_head);

        // Rebuild Previous links and locate the new tail in one O(n) pass.
        _head.Previous = null;
        var current = _head;
        while (current.Next is not null)
        {
            current.Next.Previous = current;
            current = current.Next;
        }
        _tail = current;
    }

    /// <summary>Sorts the list in ascending order. Delegates to <see cref="MergeSort"/>.</summary>
    /// <remarks>Time: O(n log n). Space: O(log n).</remarks>
    public void Sort() => MergeSort();

    // Recursively splits the chain at its midpoint, sorts each half, then merges.
    // Operates only on Next links; Previous links are rebuilt by the public caller.
    private static Node MergeSortCore(Node head)
    {
        // Base case: a single node is already sorted.
        if (head.Next is null) return head;

        Node middle = FindMiddle(head);
        Node rightHead = middle.Next!;

        // Sever the chain so the two halves are independent singly-linked lists.
        middle.Next = null;

        Node sortedLeft = MergeSortCore(head);
        Node sortedRight = MergeSortCore(rightHead);

        return Merge(sortedLeft, sortedRight);
    }

    // Returns the last node of the first half using the slow/fast pointer technique.
    // For a two-node chain this returns the first node, keeping the split balanced.
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

    // Merges two sorted, singly-linked chains (only Next links are meaningful here)
    // into one sorted chain and returns its head. Uses a sentinel to avoid a null
    // check on every iteration.
    private static Node Merge(Node left, Node right)
    {
        // The sentinel's Value is never read; it exists only to give `tail` a
        // non-null starting point so the loop body is uniform from the first iteration.
        var sentinel = new Node(default!);
        var tail = sentinel;

        while (true)
        {
            if (left.Value.CompareTo(right.Value) <= 0)
            {
                tail.Next = left;
                tail = left;          // advance tail before checking exhaustion
                left = left.Next!;
                if (left is null) { tail.Next = right; break; }
            }
            else
            {
                tail.Next = right;
                tail = right;         // advance tail before checking exhaustion
                right = right.Next!;
                if (right is null) { tail.Next = left; break; }
            }
        }

        return sentinel.Next!;
    }
}
