namespace Algorithms.Lists.Tests;

public class SortableSinglyLinkedListTests
{
    // ── Shared data source ──────────────────────────────────────────────────
    // Every sort method is driven through the same set of cases so that a
    // regression in any algorithm is caught by the corresponding [Theory].

    public static IEnumerable<object[]> SortCases =>
    [
        // Empty — all sorts must be no-ops
        [Array.Empty<int>(), Array.Empty<int>()],

        // Single element — nothing to sort
        [new[] { 42 }, new[] { 42 }],

        // Two elements — already sorted
        [new[] { 1, 2 }, new[] { 1, 2 }],

        // Two elements — reverse sorted
        [new[] { 2, 1 }, new[] { 1, 2 }],

        // All identical — duplicates must be preserved
        [new[] { 7, 7, 7, 7 }, new[] { 7, 7, 7, 7 }],

        // Pre-sorted (best-case for BubbleSort / InsertionSort early-exit)
        [new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2, 3, 4, 5 }],

        // Reverse-sorted (worst-case for BubbleSort / InsertionSort)
        [new[] { 5, 4, 3, 2, 1 }, new[] { 1, 2, 3, 4, 5 }],

        // Random input with duplicates — general correctness
        [new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3 }, new[] { 1, 1, 2, 3, 3, 4, 5, 5, 6, 9 }],

        // Random input, odd length
        [new[] { 10, -3, 7, 0, -3, 4 }, new[] { -3, -3, 0, 4, 7, 10 }],

        // Random input — larger spread
        [new[] { 100, 1, 50, 25, 75, 0, -50 }, new[] { -50, 0, 1, 25, 50, 75, 100 }],

        // Boundary values: int.MinValue and int.MaxValue
        [new[] { int.MaxValue, 0, int.MinValue }, new[] { int.MinValue, 0, int.MaxValue }],

        // Boundary values mixed with negatives and duplicates
        [new[] { int.MinValue, int.MaxValue, int.MinValue, int.MaxValue },
         new[] { int.MinValue, int.MinValue, int.MaxValue, int.MaxValue }],
    ];

    // ── BubbleSort ──────────────────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(SortCases))]
    public void BubbleSort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var list = ListOf(input);
        list.BubbleSort();
        Assert.Equal(expected, list);
    }

    [Fact]
    public void BubbleSort_PreservesSortedListCount()
    {
        var list = ListOf(5, 4, 3, 2, 1);
        list.BubbleSort();
        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void BubbleSort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(1234);
        var input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(10_000)).ToArray();
        var list = ListOf(input);

        list.BubbleSort();

        Assert.Equal(input.OrderBy(x => x), list);
        Assert.Equal(1_000, list.Count);
    }

    [Fact]
    public void BubbleSort_EarlyExitOnPresortedInput_Succeeds()
    {
        // Verifies the early-exit (swapped flag) path executes without error
        // and leaves an already-sorted list unchanged.
        var list = ListOf(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        list.BubbleSort();
        Assert.Equal([1, 2, 3, 4, 5, 6, 7, 8, 9, 10], list);
    }

    // ── SelectionSort ───────────────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(SortCases))]
    public void SelectionSort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var list = ListOf(input);
        list.SelectionSort();
        Assert.Equal(expected, list);
    }

    [Fact]
    public void SelectionSort_PreservesSortedListCount()
    {
        var list = ListOf(5, 4, 3, 2, 1);
        list.SelectionSort();
        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void SelectionSort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(5678);
        var input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(10_000)).ToArray();
        var list = ListOf(input);

        list.SelectionSort();

        Assert.Equal(input.OrderBy(x => x), list);
        Assert.Equal(1_000, list.Count);
    }

    [Fact]
    public void SelectionSort_AllSameValue_ListUnchanged()
    {
        // When the minimum node is always the position node, no swap occurs;
        // this exercises the ReferenceEquals guard in SelectionSort.
        var list = ListOf(3, 3, 3, 3, 3);
        list.SelectionSort();
        Assert.Equal([3, 3, 3, 3, 3], list);
    }

    // ── InsertionSort ───────────────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(SortCases))]
    public void InsertionSort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var list = ListOf(input);
        list.InsertionSort();
        Assert.Equal(expected, list);
    }

    [Fact]
    public void InsertionSort_PreservesSortedListCount()
    {
        var list = ListOf(5, 4, 3, 2, 1);
        list.InsertionSort();
        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void InsertionSort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(9012);
        var input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(10_000)).ToArray();
        var list = ListOf(input);

        list.InsertionSort();

        Assert.Equal(input.OrderBy(x => x), list);
        Assert.Equal(1_000, list.Count);
    }

    [Fact]
    public void InsertionSort_HeadIsUpdated_WhenSmallestComesLast()
    {
        // Each new element is smaller than everything before it, so the sorted
        // prefix head must be relinked on every step — exercises the _head reassignment.
        var list = ListOf(5, 4, 3, 2, 1);
        list.InsertionSort();
        Assert.Equal([1, 2, 3, 4, 5], list);
        // Also confirm the new head is correct after relinking.
        Assert.Equal(1, list.First());
    }

    // ── MergeSort ───────────────────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(SortCases))]
    public void MergeSort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var list = ListOf(input);
        list.MergeSort();
        Assert.Equal(expected, list);
    }

    [Fact]
    public void MergeSort_PreservesSortedListCount()
    {
        var list = ListOf(5, 4, 3, 2, 1);
        list.MergeSort();
        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void MergeSort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(3456);
        var input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(10_000)).ToArray();
        var list = ListOf(input);

        list.MergeSort();

        Assert.Equal(input.OrderBy(x => x), list);
        Assert.Equal(1_000, list.Count);
    }

    [Fact]
    public void MergeSort_VeryLargeInput_DoesNotStackOverflow()
    {
        // 10 000 elements — confirms O(log n) recursion depth does not overflow.
        var rng = new Random(7890);
        var input = Enumerable.Range(0, 10_000).Select(_ => rng.Next()).ToArray();
        var list = ListOf(input);

        list.MergeSort();

        Assert.Equal(input.OrderBy(x => x), list);
    }

    // ── Sort (delegates to MergeSort) ───────────────────────────────────────

    [Theory]
    [MemberData(nameof(SortCases))]
    public void Sort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var list = ListOf(input);
        list.Sort();
        Assert.Equal(expected, list);
    }

    [Fact]
    public void Sort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(2468);
        var input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(10_000)).ToArray();
        var list = ListOf(input);

        list.Sort();

        Assert.Equal(input.OrderBy(x => x), list);
        Assert.Equal(1_000, list.Count);
    }

    // ── Cross-algorithm consistency ─────────────────────────────────────────
    // Feed every algorithm the same random input and assert they all produce
    // the same result — catches algorithms that agree on easy cases but
    // diverge on harder ones.

    [Fact]
    public void AllSortAlgorithms_SameRandomInput_ProduceSameResult()
    {
        var rng = new Random(13579);
        var input = Enumerable.Range(0, 200).Select(_ => rng.Next(-500, 500)).ToArray();

        var bubble = ListOf(input);
        var selection = ListOf(input);
        var insertion = ListOf(input);
        var merge = ListOf(input);

        bubble.BubbleSort();
        selection.SelectionSort();
        insertion.InsertionSort();
        merge.MergeSort();

        var expected = input.OrderBy(x => x).ToArray();
        Assert.Equal(expected, bubble);
        Assert.Equal(expected, selection);
        Assert.Equal(expected, insertion);
        Assert.Equal(expected, merge);
    }

    [Fact]
    public void AllSortAlgorithms_NegativeValuesOnly_ProduceAscendingOrder()
    {
        int[] input = [-9, -1, -5, -3, -7, -2, -8, -4, -6];
        int[] expected = [-9, -8, -7, -6, -5, -4, -3, -2, -1];

        AssertAllSortsEqual(input, expected);
    }

    [Fact]
    public void AllSortAlgorithms_MixedNegativeAndPositive_ProduceAscendingOrder()
    {
        int[] input = [3, -1, 0, -5, 4, -2, 1];
        int[] expected = [-5, -2, -1, 0, 1, 3, 4];

        AssertAllSortsEqual(input, expected);
    }

    [Fact]
    public void AllSortAlgorithms_BoundaryValues_ProduceAscendingOrder()
    {
        int[] input = [int.MaxValue, int.MinValue, 0, -1, 1, int.MaxValue, int.MinValue];
        int[] expected = [int.MinValue, int.MinValue, -1, 0, 1, int.MaxValue, int.MaxValue];

        AssertAllSortsEqual(input, expected);
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static SortableSinglyLinkedList<int> ListOf(params int[] values)
    {
        var list = new SortableSinglyLinkedList<int>();
        foreach (var v in values) list.AddLast(v);
        return list;
    }

    private static void AssertAllSortsEqual(int[] input, int[] expected)
    {
        var bubble    = ListOf(input);
        var selection = ListOf(input);
        var insertion = ListOf(input);
        var merge     = ListOf(input);

        bubble.BubbleSort();
        selection.SelectionSort();
        insertion.InsertionSort();
        merge.MergeSort();

        Assert.Equal(expected, bubble);
        Assert.Equal(expected, selection);
        Assert.Equal(expected, insertion);
        Assert.Equal(expected, merge);
    }
}
