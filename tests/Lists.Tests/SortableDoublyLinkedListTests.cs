namespace Algorithms.Lists.Tests;

/// <summary>
/// Comprehensive tests for <see cref="SortableDoublyLinkedList{T}"/> covering all four sort
/// algorithms: BubbleSort, SelectionSort, InsertionSort, and MergeSort, plus Sort() which
/// delegates to MergeSort. Each algorithm is exercised against the full shared SortCases
/// matrix and a large-input regression fact. MergeSort additionally verifies DLL structural
/// invariants (Previous links and _tail pointer) that other sorts leave untouched.
/// </summary>
public class SortableDoublyLinkedListTests
{
    // ════════════════════════════════════════════════════════════════════════
    // Shared sort-case data
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// All parameterized sort scenarios shared across every algorithm under test.
    /// Rows: (input, expectedAscending).
    /// </summary>
    public static IEnumerable<object[]> SortCases =>
    [
        // empty
        [Array.Empty<int>(),                                  Array.Empty<int>()],
        // single element
        [new[] { 42 },                                        new[] { 42 }],
        // two elements — already ordered
        [new[] { 1, 2 },                                      new[] { 1, 2 }],
        // two elements — reverse ordered
        [new[] { 2, 1 },                                      new[] { 1, 2 }],
        // all identical (duplicates)
        [new[] { 7, 7, 7, 7 },                                new[] { 7, 7, 7, 7 }],
        // pre-sorted / best-case
        [new[] { 1, 2, 3, 4, 5 },                             new[] { 1, 2, 3, 4, 5 }],
        // reverse-sorted / worst-case
        [new[] { 5, 4, 3, 2, 1 },                             new[] { 1, 2, 3, 4, 5 }],
        // general random — odd length with duplicates
        [new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5 },                new[] { 1, 1, 2, 3, 4, 5, 5, 6, 9 }],
        // general random — even length, no duplicates
        [new[] { 8, 3, 7, 1, 6, 2, 5, 4 },                   new[] { 1, 2, 3, 4, 5, 6, 7, 8 }],
        // general random — larger mixed set
        [new[] { 10, -3, 0, 7, -7, 3, 10, -3, 1 },           new[] { -7, -3, -3, 0, 1, 3, 7, 10, 10 }],
        // type boundary values
        [new[] { int.MaxValue, 0, int.MinValue },              new[] { int.MinValue, 0, int.MaxValue }],
        // boundary values with duplicates
        [new[] { int.MaxValue, int.MinValue, int.MaxValue, int.MinValue },
         new[] { int.MinValue, int.MinValue, int.MaxValue, int.MaxValue }],
    ];

    // ════════════════════════════════════════════════════════════════════════
    // BubbleSort
    // ════════════════════════════════════════════════════════════════════════

    [Theory]
    [MemberData(nameof(SortCases))]
    public void BubbleSort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var list = ListOf(input);
        list.BubbleSort();
        Assert.Equal(expected, list);
    }

    [Fact]
    public void BubbleSort_PreservesCount()
    {
        var list = ListOf(5, 3, 1, 4, 2);
        list.BubbleSort();
        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void BubbleSort_EmptyList_NoExceptionAndStaysEmpty()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.BubbleSort();
        Assert.Empty(list);
    }

    [Fact]
    public void BubbleSort_SingleElement_ListUnchanged()
    {
        var list = ListOf(99);
        list.BubbleSort();
        Assert.Equal([99], list);
    }

    [Fact]
    public void BubbleSort_AllIdentical_OrderUnchanged()
    {
        var list = ListOf(5, 5, 5, 5, 5);
        list.BubbleSort();
        Assert.Equal([5, 5, 5, 5, 5], list);
    }

    [Fact]
    public void BubbleSort_PreSorted_ListUnchanged()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.BubbleSort();
        Assert.Equal([1, 2, 3, 4, 5], list);
    }

    [Fact]
    public void BubbleSort_ReverseSorted_ProducesAscendingOrder()
    {
        var list = ListOf(5, 4, 3, 2, 1);
        list.BubbleSort();
        Assert.Equal([1, 2, 3, 4, 5], list);
    }

    [Fact]
    public void BubbleSort_BoundaryValues_ProducesAscendingOrder()
    {
        var list = ListOf(int.MaxValue, 0, int.MinValue);
        list.BubbleSort();
        Assert.Equal([int.MinValue, 0, int.MaxValue], list);
    }

    [Fact]
    public void BubbleSort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(1001);
        var input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(10_000)).ToArray();
        var list = ListOf(input);

        list.BubbleSort();

        Assert.Equal(input.OrderBy(x => x), list);
    }

    [Fact]
    public void BubbleSort_AfterSort_EnumerationStartsAtSmallestElement()
    {
        var list = ListOf(9, 3, 7, 1, 5);
        list.BubbleSort();
        Assert.Equal(1, list.First());
    }

    [Fact]
    public void BubbleSort_AfterSort_EnumerationEndsAtLargestElement()
    {
        var list = ListOf(9, 3, 7, 1, 5);
        list.BubbleSort();
        Assert.Equal(9, list.Last());
    }

    // ════════════════════════════════════════════════════════════════════════
    // SelectionSort
    // ════════════════════════════════════════════════════════════════════════

    [Theory]
    [MemberData(nameof(SortCases))]
    public void SelectionSort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var list = ListOf(input);
        list.SelectionSort();
        Assert.Equal(expected, list);
    }

    [Fact]
    public void SelectionSort_PreservesCount()
    {
        var list = ListOf(5, 3, 1, 4, 2);
        list.SelectionSort();
        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void SelectionSort_EmptyList_NoExceptionAndStaysEmpty()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.SelectionSort();
        Assert.Empty(list);
        Assert.Empty(list);
    }

    [Fact]
    public void SelectionSort_SingleElement_ListUnchanged()
    {
        var list = ListOf(99);
        list.SelectionSort();
        Assert.Equal([99], list);
    }

    [Fact]
    public void SelectionSort_AllIdentical_OrderUnchanged()
    {
        var list = ListOf(3, 3, 3, 3);
        list.SelectionSort();
        Assert.Equal([3, 3, 3, 3], list);
    }

    [Fact]
    public void SelectionSort_PreSorted_ListUnchanged()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.SelectionSort();
        Assert.Equal([1, 2, 3, 4, 5], list);
    }

    [Fact]
    public void SelectionSort_ReverseSorted_ProducesAscendingOrder()
    {
        var list = ListOf(5, 4, 3, 2, 1);
        list.SelectionSort();
        Assert.Equal([1, 2, 3, 4, 5], list);
    }

    [Fact]
    public void SelectionSort_BoundaryValues_ProducesAscendingOrder()
    {
        var list = ListOf(int.MaxValue, 0, int.MinValue);
        list.SelectionSort();
        Assert.Equal([int.MinValue, 0, int.MaxValue], list);
    }

    [Fact]
    public void SelectionSort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(2002);
        var input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(10_000)).ToArray();
        var list = ListOf(input);

        list.SelectionSort();

        Assert.Equal(input.OrderBy(x => x), list);
    }

    [Fact]
    public void SelectionSort_AfterSort_EnumerationStartsAtSmallestElement()
    {
        var list = ListOf(9, 3, 7, 1, 5);
        list.SelectionSort();
        Assert.Equal(1, list.First());
    }

    [Fact]
    public void SelectionSort_AfterSort_EnumerationEndsAtLargestElement()
    {
        var list = ListOf(9, 3, 7, 1, 5);
        list.SelectionSort();
        Assert.Equal(9, list.Last());
    }

    // ════════════════════════════════════════════════════════════════════════
    // InsertionSort
    // ════════════════════════════════════════════════════════════════════════

    [Theory]
    [MemberData(nameof(SortCases))]
    public void InsertionSort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var list = ListOf(input);
        list.InsertionSort();
        Assert.Equal(expected, list);
    }

    [Fact]
    public void InsertionSort_PreservesCount()
    {
        var list = ListOf(5, 3, 1, 4, 2);
        list.InsertionSort();
        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void InsertionSort_EmptyList_NoExceptionAndStaysEmpty()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.InsertionSort();
        Assert.Empty(list);
        Assert.Empty(list);
    }

    [Fact]
    public void InsertionSort_SingleElement_ListUnchanged()
    {
        var list = ListOf(99);
        list.InsertionSort();
        Assert.Equal([99], list);
    }

    [Fact]
    public void InsertionSort_AllIdentical_OrderUnchanged()
    {
        var list = ListOf(6, 6, 6, 6);
        list.InsertionSort();
        Assert.Equal([6, 6, 6, 6], list);
    }

    [Fact]
    public void InsertionSort_PreSorted_ListUnchanged()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.InsertionSort();
        Assert.Equal([1, 2, 3, 4, 5], list);
    }

    [Fact]
    public void InsertionSort_ReverseSorted_ProducesAscendingOrder()
    {
        var list = ListOf(5, 4, 3, 2, 1);
        list.InsertionSort();
        Assert.Equal([1, 2, 3, 4, 5], list);
    }

    [Fact]
    public void InsertionSort_BoundaryValues_ProducesAscendingOrder()
    {
        var list = ListOf(int.MaxValue, 0, int.MinValue);
        list.InsertionSort();
        Assert.Equal([int.MinValue, 0, int.MaxValue], list);
    }

    [Fact]
    public void InsertionSort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(3003);
        var input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(10_000)).ToArray();
        var list = ListOf(input);

        list.InsertionSort();

        Assert.Equal(input.OrderBy(x => x), list);
    }

    [Fact]
    public void InsertionSort_AfterSort_EnumerationStartsAtSmallestElement()
    {
        var list = ListOf(9, 3, 7, 1, 5);
        list.InsertionSort();
        Assert.Equal(1, list.First());
    }

    [Fact]
    public void InsertionSort_AfterSort_EnumerationEndsAtLargestElement()
    {
        var list = ListOf(9, 3, 7, 1, 5);
        list.InsertionSort();
        Assert.Equal(9, list.Last());
    }

    // ════════════════════════════════════════════════════════════════════════
    // MergeSort
    // ════════════════════════════════════════════════════════════════════════

    [Theory]
    [MemberData(nameof(SortCases))]
    public void MergeSort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var list = ListOf(input);
        list.MergeSort();
        Assert.Equal(expected, list);
    }

    [Fact]
    public void MergeSort_PreservesCount()
    {
        var list = ListOf(5, 3, 1, 4, 2);
        list.MergeSort();
        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void MergeSort_EmptyList_NoExceptionAndStaysEmpty()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.MergeSort();
        Assert.Empty(list);
        Assert.Empty(list);
    }

    [Fact]
    public void MergeSort_SingleElement_ListUnchanged()
    {
        var list = ListOf(99);
        list.MergeSort();
        Assert.Equal([99], list);
    }

    [Fact]
    public void MergeSort_AllIdentical_OrderUnchanged()
    {
        var list = ListOf(4, 4, 4, 4);
        list.MergeSort();
        Assert.Equal([4, 4, 4, 4], list);
    }

    [Fact]
    public void MergeSort_PreSorted_ListUnchanged()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.MergeSort();
        Assert.Equal([1, 2, 3, 4, 5], list);
    }

    [Fact]
    public void MergeSort_ReverseSorted_ProducesAscendingOrder()
    {
        var list = ListOf(5, 4, 3, 2, 1);
        list.MergeSort();
        Assert.Equal([1, 2, 3, 4, 5], list);
    }

    [Fact]
    public void MergeSort_BoundaryValues_ProducesAscendingOrder()
    {
        var list = ListOf(int.MaxValue, 0, int.MinValue);
        list.MergeSort();
        Assert.Equal([int.MinValue, 0, int.MaxValue], list);
    }

    [Fact]
    public void MergeSort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(4004);
        var input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(10_000)).ToArray();
        var list = ListOf(input);

        list.MergeSort();

        Assert.Equal(input.OrderBy(x => x), list);
    }

    // ── MergeSort DLL structural invariants ─────────────────────────────────
    // After MergeSort the Previous links and _tail pointer must be correctly
    // rebuilt so every inherited DoublyLinkedList operation still works.

    [Fact]
    public void MergeSort_AfterSort_RemoveFirstReturnsSmallestElement()
    {
        var list = ListOf(5, 3, 9, 1, 7);
        list.MergeSort();                  // [1, 3, 5, 7, 9]
        Assert.Equal(1, list.RemoveFirst());
    }

    [Fact]
    public void MergeSort_AfterSort_RemoveLastReturnsLargestElement()
    {
        var list = ListOf(5, 3, 9, 1, 7);
        list.MergeSort();                  // [1, 3, 5, 7, 9]
        Assert.Equal(9, list.RemoveLast());
    }

    [Fact]
    public void MergeSort_AfterSort_RemoveFirstThenRemoveLast_LeavesMiddleElements()
    {
        var list = ListOf(5, 3, 9, 1, 7);
        list.MergeSort();                  // [1, 3, 5, 7, 9]
        list.RemoveFirst();
        list.RemoveLast();
        Assert.Equal([3, 5, 7], list);
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void MergeSort_AfterSort_AddFirstProducesCorrectOrder()
    {
        var list = ListOf(3, 1, 2);
        list.MergeSort();                  // [1, 2, 3]
        list.AddFirst(0);
        Assert.Equal([0, 1, 2, 3], list);
    }

    [Fact]
    public void MergeSort_AfterSort_AddLastProducesCorrectOrder()
    {
        var list = ListOf(3, 1, 2);
        list.MergeSort();                  // [1, 2, 3]
        list.AddLast(4);
        Assert.Equal([1, 2, 3, 4], list);
    }

    [Fact]
    public void MergeSort_AfterSort_AddFirstAndAddLastBothProduceCorrectOrder()
    {
        var list = ListOf(3, 1, 2);
        list.MergeSort();                  // [1, 2, 3]
        list.AddFirst(0);
        list.AddLast(4);
        Assert.Equal([0, 1, 2, 3, 4], list);
    }

    [Fact]
    public void MergeSort_AfterSort_ReverseThenEnumerateYieldsDescendingOrder()
    {
        // Reverse relies on Previous links being valid, so this also validates
        // that MergeSort correctly rebuilt all Previous pointers.
        var list = ListOf(3, 1, 4, 1, 5, 9, 2, 6);
        list.MergeSort();                  // [1, 1, 2, 3, 4, 5, 6, 9]
        list.Reverse();
        Assert.Equal([9, 6, 5, 4, 3, 2, 1, 1], list);
    }

    [Fact]
    public void MergeSort_AfterSort_RemoveByValueWorksCorrectly()
    {
        // Remove() walks Next links and stitches via Previous links —
        // both must be valid after MergeSort.
        var list = ListOf(4, 2, 5, 1, 3);
        list.MergeSort();                  // [1, 2, 3, 4, 5]
        Assert.True(list.Remove(3));
        Assert.Equal([1, 2, 4, 5], list);
        Assert.Equal(4, list.Count);
    }

    [Fact]
    public void MergeSort_AfterSort_ConsecutiveRemoveLastDrainsListCorrectly()
    {
        // Exercises _tail pointer correctness: each RemoveLast must find a
        // valid Previous pointer on the current tail.
        var list = ListOf(3, 1, 4, 1, 5);
        list.MergeSort();                  // [1, 1, 3, 4, 5]

        var removed = new List<int>();
        while (list.Count > 0) removed.Add(list.RemoveLast());

        Assert.Equal([5, 4, 3, 1, 1], removed);
    }

    // ── MergeSort stability ──────────────────────────────────────────────────
    // MergeSort uses <= in the merge step, so equal elements from the left
    // half are always emitted before equal elements from the right half,
    // preserving insertion order for equal keys.

    [Fact]
    public void MergeSort_IsStable_EqualKeysPreserveInsertionOrder()
    {
        var list = new SortableDoublyLinkedList<StableItem>();
        list.AddLast(new StableItem(2, 'A'));
        list.AddLast(new StableItem(1, 'B'));
        list.AddLast(new StableItem(2, 'C'));
        list.AddLast(new StableItem(1, 'D'));
        list.MergeSort();
        // Elements with equal keys must retain their original insertion order.
        Assert.Equal(
            [new StableItem(1, 'B'), new StableItem(1, 'D'), new StableItem(2, 'A'), new StableItem(2, 'C')],
            list);
    }

    [Fact]
    public void MergeSort_IsStable_AllEqualElements_InsertionOrderPreserved()
    {
        // When every element is equal the entire list must be reproduced
        // unchanged, which is the trivial stability requirement.
        var list = ListOf(2, 2, 2, 2, 2);
        list.MergeSort();
        Assert.Equal([2, 2, 2, 2, 2], list);
    }

    // ════════════════════════════════════════════════════════════════════════
    // Sort() — delegates to MergeSort
    // ════════════════════════════════════════════════════════════════════════

    [Theory]
    [MemberData(nameof(SortCases))]
    public void Sort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var list = ListOf(input);
        list.Sort();
        Assert.Equal(expected, list);
    }

    [Fact]
    public void Sort_PreservesCount()
    {
        var list = ListOf(5, 3, 1, 4, 2);
        list.Sort();
        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void Sort_EmptyList_NoExceptionAndStaysEmpty()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.Sort();
        Assert.Empty(list);
        Assert.Empty(list);
    }

    [Fact]
    public void Sort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(5005);
        var input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(10_000)).ToArray();
        var list = ListOf(input);

        list.Sort();

        Assert.Equal(input.OrderBy(x => x), list);
    }

    // ════════════════════════════════════════════════════════════════════════
    // Cross-algorithm consistency
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Verifies that all four algorithms produce bit-for-bit identical output
    /// for the same input, acting as a mutual-oracle check.
    /// </summary>
    [Fact]
    public void AllSortAlgorithms_SameRandomInput_ProduceIdenticalOutput()
    {
        var rng = new Random(6006);
        var input = Enumerable.Range(0, 200).Select(_ => rng.Next(1_000)).ToArray();

        var bubble    = ListOf(input); bubble.BubbleSort();
        var selection = ListOf(input); selection.SelectionSort();
        var insertion = ListOf(input); insertion.InsertionSort();
        var merge     = ListOf(input); merge.MergeSort();

        Assert.Equal(bubble, selection);
        Assert.Equal(bubble, insertion);
        Assert.Equal(bubble, merge);
    }

    [Fact]
    public void AllSortAlgorithms_BoundaryValues_ProduceIdenticalOutput()
    {
        int[] input = [int.MaxValue, int.MinValue, 0, int.MaxValue, int.MinValue];

        var bubble    = ListOf(input); bubble.BubbleSort();
        var selection = ListOf(input); selection.SelectionSort();
        var insertion = ListOf(input); insertion.InsertionSort();
        var merge     = ListOf(input); merge.MergeSort();

        Assert.Equal(bubble, selection);
        Assert.Equal(bubble, insertion);
        Assert.Equal(bubble, merge);
    }

    [Fact]
    public void AllSortAlgorithms_TwoElements_SameOutput()
    {
        int[] input = [2, 1];

        var bubble    = ListOf(input); bubble.BubbleSort();
        var selection = ListOf(input); selection.SelectionSort();
        var insertion = ListOf(input); insertion.InsertionSort();
        var merge     = ListOf(input); merge.MergeSort();

        int[] expected = [1, 2];
        Assert.Equal(expected, bubble);
        Assert.Equal(expected, selection);
        Assert.Equal(expected, insertion);
        Assert.Equal(expected, merge);
    }

    // ════════════════════════════════════════════════════════════════════════
    // Helper
    // ════════════════════════════════════════════════════════════════════════

    private static SortableDoublyLinkedList<int> ListOf(params int[] values)
    {
        var list = new SortableDoublyLinkedList<int>();
        foreach (var v in values) list.AddLast(v);
        return list;
    }

    private sealed record StableItem(int Key, char Label) : IComparable<StableItem>
    {
        public int CompareTo(StableItem? other) => Key.CompareTo(other?.Key ?? 0);
    }
}
