namespace Algorithms.Lists.Tests;

public class SortableCircularDoublyLinkedListTests
{
    // ── Shared sort cases ────────────────────────────────────────────────────

    /// <summary>
    /// All parameterized sort tests draw from this single data source so that
    /// adding a new scenario automatically covers SelectionSort, InsertionSort,
    /// and MergeSort.
    /// </summary>
    public static IEnumerable<object[]> SortCases =>
    [
        // Empty — sentinel ring only, Count == 0
        [Array.Empty<int>(), Array.Empty<int>()],

        // Single element
        [new[] { 42 }, new[] { 42 }],

        // Two elements — already sorted
        [new[] { 1, 2 }, new[] { 1, 2 }],

        // Two elements — reverse-sorted
        [new[] { 2, 1 }, new[] { 1, 2 }],

        // All identical (duplicates)
        [new[] { 7, 7, 7, 7, 7 }, new[] { 7, 7, 7, 7, 7 }],

        // Pre-sorted / best-case
        [new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2, 3, 4, 5 }],

        // Reverse-sorted / worst-case
        [new[] { 5, 4, 3, 2, 1 }, new[] { 1, 2, 3, 4, 5 }],

        // Random with duplicates (mid-size)
        [new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5 }, new[] { 1, 1, 2, 3, 3, 4, 5, 5, 5, 6, 9 }],

        // Another random case — different size
        [new[] { 10, -3, 0, 7, -1, 4 }, new[] { -3, -1, 0, 4, 7, 10 }],

        // Negative-heavy random case
        [new[] { -8, -2, -5, -1, -9, -4, -7, -3, -6 }, new[] { -9, -8, -7, -6, -5, -4, -3, -2, -1 }],

        // Boundary values — int.MinValue and int.MaxValue
        [new[] { int.MaxValue, 0, int.MinValue }, new[] { int.MinValue, 0, int.MaxValue }],

        // Boundary values — both extremes plus other values
        [new[] { int.MinValue, 1, int.MaxValue, -1, int.MinValue }, new[] { int.MinValue, int.MinValue, -1, 1, int.MaxValue }],
    ];

    // ── SelectionSort ────────────────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(SortCases))]
    public void SelectionSort_SortCases_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var list = ListOf(input);
        list.SelectionSort();
        Assert.Equal(expected, list);
    }

    [Theory]
    [MemberData(nameof(SortCases))]
    public void SelectionSort_SortCases_CountIsUnchanged(int[] input, int[] expected)
    {
        var list = ListOf(input);
        list.SelectionSort();
        Assert.Equal(expected.Length, list.Count);
    }

    [Fact]
    public void SelectionSort_LargeRandomInput_ProducesAscendingOrder()
    {
        const int n = 1_000;
        var rng = new Random(42);
        var list = new SortableCircularDoublyLinkedList<int>();
        for (int i = 0; i < n; i++) list.AddLast(rng.Next(-500, 500));

        list.SelectionSort();

        var result = list.ToArray();
        Assert.Equal(n, result.Length);
        for (int i = 1; i < result.Length; i++)
            Assert.True(result[i - 1] <= result[i],
                $"SelectionSort: element at [{i - 1}]={result[i - 1]} > element at [{i}]={result[i]}");
    }

    [Fact]
    public void SelectionSort_LargeRandomInput_CountIsUnchanged()
    {
        const int n = 1_000;
        var rng = new Random(99);
        var list = new SortableCircularDoublyLinkedList<int>();
        for (int i = 0; i < n; i++) list.AddLast(rng.Next());

        list.SelectionSort();

        Assert.Equal(n, list.Count);
    }

    [Fact]
    public void SelectionSort_AllIdentical_LargeInput_ElementsUnchanged()
    {
        const int n = 1_000;
        var list = new SortableCircularDoublyLinkedList<int>();
        for (int i = 0; i < n; i++) list.AddLast(3);

        list.SelectionSort();

        Assert.Equal(Enumerable.Repeat(3, n), list);
    }

    // ── InsertionSort ────────────────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(SortCases))]
    public void InsertionSort_SortCases_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var list = ListOf(input);
        list.InsertionSort();
        Assert.Equal(expected, list);
    }

    [Theory]
    [MemberData(nameof(SortCases))]
    public void InsertionSort_SortCases_CountIsUnchanged(int[] input, int[] expected)
    {
        var list = ListOf(input);
        list.InsertionSort();
        Assert.Equal(expected.Length, list.Count);
    }

    [Fact]
    public void InsertionSort_LargeRandomInput_ProducesAscendingOrder()
    {
        const int n = 1_000;
        var rng = new Random(17);
        var list = new SortableCircularDoublyLinkedList<int>();
        for (int i = 0; i < n; i++) list.AddLast(rng.Next(-500, 500));

        list.InsertionSort();

        var result = list.ToArray();
        Assert.Equal(n, result.Length);
        for (int i = 1; i < result.Length; i++)
            Assert.True(result[i - 1] <= result[i],
                $"InsertionSort: element at [{i - 1}]={result[i - 1]} > element at [{i}]={result[i]}");
    }

    [Fact]
    public void InsertionSort_LargeRandomInput_CountIsUnchanged()
    {
        const int n = 1_000;
        var rng = new Random(71);
        var list = new SortableCircularDoublyLinkedList<int>();
        for (int i = 0; i < n; i++) list.AddLast(rng.Next());

        list.InsertionSort();

        Assert.Equal(n, list.Count);
    }

    [Fact]
    public void InsertionSort_PreSortedLargeInput_LeavesOrderUnchanged()
    {
        // InsertionSort should be O(n) on already-sorted data — this verifies
        // correctness and acts as a regression guard for the best-case path.
        const int n = 1_000;
        var list = new SortableCircularDoublyLinkedList<int>();
        for (int i = 0; i < n; i++) list.AddLast(i);

        list.InsertionSort();

        Assert.Equal(Enumerable.Range(0, n), list);
    }

    [Fact]
    public void InsertionSort_AllIdentical_LargeInput_ElementsUnchanged()
    {
        const int n = 1_000;
        var list = new SortableCircularDoublyLinkedList<int>();
        for (int i = 0; i < n; i++) list.AddLast(5);

        list.InsertionSort();

        Assert.Equal(Enumerable.Repeat(5, n), list);
    }

    // ── MergeSort ────────────────────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(SortCases))]
    public void MergeSort_SortCases_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var list = ListOf(input);
        list.MergeSort();
        Assert.Equal(expected, list);
    }

    [Theory]
    [MemberData(nameof(SortCases))]
    public void MergeSort_SortCases_CountIsUnchanged(int[] input, int[] expected)
    {
        var list = ListOf(input);
        list.MergeSort();
        Assert.Equal(expected.Length, list.Count);
    }

    [Fact]
    public void MergeSort_LargeRandomInput_ProducesAscendingOrder()
    {
        const int n = 1_000;
        var rng = new Random(7);
        var list = new SortableCircularDoublyLinkedList<int>();
        for (int i = 0; i < n; i++) list.AddLast(rng.Next(-500, 500));

        list.MergeSort();

        var result = list.ToArray();
        Assert.Equal(n, result.Length);
        for (int i = 1; i < result.Length; i++)
            Assert.True(result[i - 1] <= result[i],
                $"MergeSort: element at [{i - 1}]={result[i - 1]} > element at [{i}]={result[i]}");
    }

    [Fact]
    public void MergeSort_LargeRandomInput_CountIsUnchanged()
    {
        const int n = 1_000;
        var rng = new Random(31);
        var list = new SortableCircularDoublyLinkedList<int>();
        for (int i = 0; i < n; i++) list.AddLast(rng.Next());

        list.MergeSort();

        Assert.Equal(n, list.Count);
    }

    [Fact]
    public void MergeSort_AllIdentical_LargeInput_ElementsUnchanged()
    {
        const int n = 1_000;
        var list = new SortableCircularDoublyLinkedList<int>();
        for (int i = 0; i < n; i++) list.AddLast(2);

        list.MergeSort();

        Assert.Equal(Enumerable.Repeat(2, n), list);
    }

    // ── MergeSort — ring invariant after sort ────────────────────────────────

    [Fact]
    public void MergeSort_RemoveFirst_ReturnsMinimum()
    {
        var list = ListOf(9, 3, 6, 1, 8, 2, 7, 4, 5);
        list.MergeSort();
        Assert.Equal(1, list.RemoveFirst());
    }

    [Fact]
    public void MergeSort_RemoveLast_ReturnsMaximum()
    {
        var list = ListOf(9, 3, 6, 1, 8, 2, 7, 4, 5);
        list.MergeSort();
        Assert.Equal(9, list.RemoveLast());
    }

    [Fact]
    public void MergeSort_RemoveFirstAndLast_LeavesInteriorIntact()
    {
        // After removing min and max the remaining elements must still be sorted.
        var list = ListOf(9, 3, 6, 1, 8, 2, 7, 4, 5);
        list.MergeSort();
        list.RemoveFirst(); // removes 1
        list.RemoveLast();  // removes 9
        Assert.Equal([2, 3, 4, 5, 6, 7, 8], list);
    }

    [Fact]
    public void MergeSort_AddFirst_PrependsBelowMinimum()
    {
        var list = ListOf(5, 3, 7, 1, 9);
        list.MergeSort();          // [1, 3, 5, 7, 9]
        list.AddFirst(0);
        Assert.Equal([0, 1, 3, 5, 7, 9], list);
        Assert.Equal(6, list.Count);
    }

    [Fact]
    public void MergeSort_AddLast_AppendsAboveMaximum()
    {
        var list = ListOf(5, 3, 7, 1, 9);
        list.MergeSort();          // [1, 3, 5, 7, 9]
        list.AddLast(100);
        Assert.Equal([1, 3, 5, 7, 9, 100], list);
        Assert.Equal(6, list.Count);
    }

    [Fact]
    public void MergeSort_AddFirstAndLast_BothEndsAreReachable()
    {
        var list = ListOf(4, 2, 6);
        list.MergeSort();           // [2, 4, 6]
        list.AddFirst(-1);
        list.AddLast(99);
        Assert.Equal([-1, 2, 4, 6, 99], list);
        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void MergeSort_RotateByOne_ProducesExpectedOrder()
    {
        // After MergeSort the ring must be properly circular so that Rotate works.
        var list = ListOf(5, 1, 4, 2, 3);
        list.MergeSort();   // [1, 2, 3, 4, 5]
        list.Rotate(1);     // [2, 3, 4, 5, 1]
        Assert.Equal([2, 3, 4, 5, 1], list);
    }

    [Fact]
    public void MergeSort_RotateByNegativeOne_ProducesExpectedOrder()
    {
        // Rotate right by 1: tail moves to head.
        var list = ListOf(5, 1, 4, 2, 3);
        list.MergeSort();   // [1, 2, 3, 4, 5]
        list.Rotate(-1);    // [5, 1, 2, 3, 4]
        Assert.Equal([5, 1, 2, 3, 4], list);
    }

    [Fact]
    public void MergeSort_RotateByCount_FullCycle_OrderIsUnchanged()
    {
        var list = ListOf(3, 1, 4, 1, 5);
        list.MergeSort();       // [1, 1, 3, 4, 5]
        list.Rotate(list.Count); // full cycle — same order
        Assert.Equal([1, 1, 3, 4, 5], list);
    }

    [Fact]
    public void MergeSort_CountIsUnchangedAfterSort_WithSubsequentMutation()
    {
        var list = ListOf(8, 3, 5, 1, 7);
        list.MergeSort();
        Assert.Equal(5, list.Count);

        list.RemoveFirst();
        Assert.Equal(4, list.Count);

        list.AddLast(99);
        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void MergeSort_EmptyList_CountRemainsZero()
    {
        var list = new SortableCircularDoublyLinkedList<int>();
        list.MergeSort();
        Assert.Empty(list);
        Assert.Empty(list);
    }

    [Fact]
    public void MergeSort_SingleElement_CountRemainsOne()
    {
        var list = ListOf(42);
        list.MergeSort();
        Assert.Single(list);
        Assert.Equal([42], list);
    }

    [Fact]
    public void MergeSort_TwoElements_RingStillNavigableAfterSort()
    {
        // Validates that the sentinel is correctly re-wired for a minimal non-trivial ring.
        var list = ListOf(2, 1);
        list.MergeSort();
        Assert.Equal([1, 2], list);
        Assert.Equal(2, list.Count);

        // Structural probe: the ring must survive a RemoveFirst followed by RemoveLast.
        Assert.Equal(1, list.RemoveFirst());
        Assert.Equal(2, list.RemoveLast());
        Assert.Empty(list);
    }

    [Fact]
    public void MergeSort_PowerOfTwoSize_ProducesAscendingOrder()
    {
        // Power-of-two sizes stress the even-split path of MergeSortCore.
        var data = new[] { 16, 7, 4, 14, 2, 11, 6, 13, 1, 10, 5, 12, 3, 9, 8, 15 };
        var list = ListOf(data);
        list.MergeSort();
        Assert.Equal(Enumerable.Range(1, 16), list);
    }

    [Fact]
    public void MergeSort_OddSize_ProducesAscendingOrder()
    {
        // Odd count exercises the uneven split (slow/fast pointer lands off-centre).
        var list = ListOf(7, 2, 9, 1, 5, 3, 8, 4, 6);
        list.MergeSort();
        Assert.Equal([1, 2, 3, 4, 5, 6, 7, 8, 9], list);
    }

    // ── Cross-algorithm consistency ──────────────────────────────────────────

    [Fact]
    public void AllThreeSorts_ProduceSameResult_OnIdenticalInput()
    {
        int[] input = [15, 3, 9, 1, 12, 5, 7, 2, 11, 6, 14, 4, 13, 8, 10];

        var sel = ListOf(input);
        var ins = ListOf(input);
        var mrg = ListOf(input);

        sel.SelectionSort();
        ins.InsertionSort();
        mrg.MergeSort();

        Assert.Equal(sel, ins);
        Assert.Equal(sel, mrg);
    }

    [Fact]
    public void AllThreeSorts_BoundaryValues_ProduceSameResult()
    {
        int[] input = [int.MaxValue, 0, int.MinValue, 1, -1];

        var sel = ListOf(input);
        var ins = ListOf(input);
        var mrg = ListOf(input);

        sel.SelectionSort();
        ins.InsertionSort();
        mrg.MergeSort();

        int[] expected = [int.MinValue, -1, 0, 1, int.MaxValue];
        Assert.Equal(expected, sel);
        Assert.Equal(expected, ins);
        Assert.Equal(expected, mrg);
    }

    [Fact]
    public void AllThreeSorts_AllIdentical_ProduceSameResult()
    {
        int[] input = [5, 5, 5, 5, 5, 5];

        var sel = ListOf(input);
        var ins = ListOf(input);
        var mrg = ListOf(input);

        sel.SelectionSort();
        ins.InsertionSort();
        mrg.MergeSort();

        Assert.Equal(sel, ins);
        Assert.Equal(sel, mrg);
        Assert.Equal(input, sel.ToArray());
    }

    // ── Idempotency — sorting an already-sorted list ─────────────────────────

    [Fact]
    public void SelectionSort_CalledTwice_IsIdempotent()
    {
        var list = ListOf(3, 1, 4, 1, 5, 9, 2, 6);
        list.SelectionSort();
        var afterFirst = list.ToArray();
        list.SelectionSort();
        Assert.Equal(afterFirst, list.ToArray());
    }

    [Fact]
    public void InsertionSort_CalledTwice_IsIdempotent()
    {
        var list = ListOf(3, 1, 4, 1, 5, 9, 2, 6);
        list.InsertionSort();
        var afterFirst = list.ToArray();
        list.InsertionSort();
        Assert.Equal(afterFirst, list.ToArray());
    }

    [Fact]
    public void MergeSort_CalledTwice_IsIdempotent()
    {
        var list = ListOf(3, 1, 4, 1, 5, 9, 2, 6);
        list.MergeSort();
        var afterFirst = list.ToArray();
        list.MergeSort();
        Assert.Equal(afterFirst, list.ToArray());
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static SortableCircularDoublyLinkedList<int> ListOf(params int[] values)
    {
        var list = new SortableCircularDoublyLinkedList<int>();
        foreach (var v in values) list.AddLast(v);
        return list;
    }
}
