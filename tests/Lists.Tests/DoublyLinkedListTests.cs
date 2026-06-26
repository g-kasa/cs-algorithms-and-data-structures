namespace Algorithms.Lists.Tests;

public class DoublyLinkedListTests
{
    // ── Count ───────────────────────────────────────────────────────────────

    [Fact]
    public void Count_NewList_IsZero()
    {
        var list = new SortableDoublyLinkedList<int>();
        Assert.Equal(0, list.Count);
    }

    // ── AddFirst ────────────────────────────────────────────────────────────

    [Fact]
    public void AddFirst_OnEmptyList_CountIsOne()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.AddFirst(1);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void AddFirst_OnEmptyList_ElementIsEnumerated()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.AddFirst(42);
        Assert.Equal([42], list);
    }

    [Fact]
    public void AddFirst_MultipleElements_PrependsInOrder()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.AddFirst(3);
        list.AddFirst(2);
        list.AddFirst(1);
        Assert.Equal([1, 2, 3], list);
    }

    [Fact]
    public void AddFirst_MultipleElements_CountMatchesNumberOfCalls()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.AddFirst(1);
        list.AddFirst(2);
        list.AddFirst(3);
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void AddFirst_ThenAddLast_HeadAndTailAreCorrect()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.AddFirst(2);
        list.AddLast(3);
        list.AddFirst(1);
        Assert.Equal([1, 2, 3], list);
    }

    // ── AddLast ─────────────────────────────────────────────────────────────

    [Fact]
    public void AddLast_OnEmptyList_CountIsOne()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.AddLast(1);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void AddLast_OnEmptyList_ElementIsEnumerated()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.AddLast(99);
        Assert.Equal([99], list);
    }

    [Fact]
    public void AddLast_MultipleElements_AppendsInOrder()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.AddLast(1);
        list.AddLast(2);
        list.AddLast(3);
        Assert.Equal([1, 2, 3], list);
    }

    [Fact]
    public void AddLast_MultipleElements_CountMatchesNumberOfCalls()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.AddLast(10);
        list.AddLast(20);
        list.AddLast(30);
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void AddLast_DuplicateValues_AllStoredInOrder()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.AddLast(5);
        list.AddLast(5);
        list.AddLast(5);
        Assert.Equal([5, 5, 5], list);
        Assert.Equal(3, list.Count);
    }

    // ── RemoveFirst ─────────────────────────────────────────────────────────

    [Fact]
    public void RemoveFirst_EmptyList_ThrowsInvalidOperationException()
    {
        var list = new SortableDoublyLinkedList<int>();
        Assert.Throws<InvalidOperationException>(() => list.RemoveFirst());
    }

    [Fact]
    public void RemoveFirst_SingleElement_ReturnsValueAndListBecomesEmpty()
    {
        var list = ListOf(42);
        var removed = list.RemoveFirst();
        Assert.Equal(42, removed);
        Assert.Equal(0, list.Count);
        Assert.Empty(list);
    }

    [Fact]
    public void RemoveFirst_MultipleElements_ReturnsHeadValue()
    {
        var list = ListOf(10, 20, 30);
        var removed = list.RemoveFirst();
        Assert.Equal(10, removed);
    }

    [Fact]
    public void RemoveFirst_MultipleElements_DecrementsCount()
    {
        var list = ListOf(10, 20, 30);
        list.RemoveFirst();
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void RemoveFirst_MultipleElements_RemainingOrderIsCorrect()
    {
        var list = ListOf(10, 20, 30);
        list.RemoveFirst();
        Assert.Equal([20, 30], list);
    }

    [Fact]
    public void RemoveFirst_TwoElements_SecondElementBecomesOnlyElement()
    {
        var list = ListOf(1, 2);
        list.RemoveFirst();
        Assert.Equal([2], list);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void RemoveFirst_AllElements_CountDropsToZero()
    {
        var list = ListOf(1, 2, 3);
        list.RemoveFirst();
        list.RemoveFirst();
        list.RemoveFirst();
        Assert.Equal(0, list.Count);
        Assert.Empty(list);
    }

    [Fact]
    public void RemoveFirst_AfterBecomingEmpty_ThrowsInvalidOperationException()
    {
        var list = ListOf(1);
        list.RemoveFirst();
        Assert.Throws<InvalidOperationException>(() => list.RemoveFirst());
    }

    // ── RemoveLast ──────────────────────────────────────────────────────────

    [Fact]
    public void RemoveLast_EmptyList_ThrowsInvalidOperationException()
    {
        var list = new SortableDoublyLinkedList<int>();
        Assert.Throws<InvalidOperationException>(() => list.RemoveLast());
    }

    [Fact]
    public void RemoveLast_SingleElement_ReturnsValueAndListBecomesEmpty()
    {
        var list = ListOf(42);
        var removed = list.RemoveLast();
        Assert.Equal(42, removed);
        Assert.Equal(0, list.Count);
        Assert.Empty(list);
    }

    [Fact]
    public void RemoveLast_MultipleElements_ReturnsTailValue()
    {
        var list = ListOf(10, 20, 30);
        var removed = list.RemoveLast();
        Assert.Equal(30, removed);
    }

    [Fact]
    public void RemoveLast_MultipleElements_DecrementsCount()
    {
        var list = ListOf(10, 20, 30);
        list.RemoveLast();
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void RemoveLast_MultipleElements_RemainingOrderIsCorrect()
    {
        var list = ListOf(10, 20, 30);
        list.RemoveLast();
        Assert.Equal([10, 20], list);
    }

    [Fact]
    public void RemoveLast_TwoElements_FirstElementBecomesOnlyElement()
    {
        var list = ListOf(1, 2);
        list.RemoveLast();
        Assert.Equal([1], list);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void RemoveLast_AllElements_CountDropsToZero()
    {
        var list = ListOf(1, 2, 3);
        list.RemoveLast();
        list.RemoveLast();
        list.RemoveLast();
        Assert.Equal(0, list.Count);
        Assert.Empty(list);
    }

    [Fact]
    public void RemoveLast_AfterBecomingEmpty_ThrowsInvalidOperationException()
    {
        var list = ListOf(1);
        list.RemoveLast();
        Assert.Throws<InvalidOperationException>(() => list.RemoveLast());
    }

    [Fact]
    public void RemoveFirst_ThenRemoveLast_LeavesListEmpty()
    {
        var list = ListOf(1, 2);
        list.RemoveFirst();
        list.RemoveLast();
        Assert.Equal(0, list.Count);
        Assert.Empty(list);
    }

    // ── Remove ──────────────────────────────────────────────────────────────

    [Fact]
    public void Remove_EmptyList_ReturnsFalse()
    {
        Assert.False(new DoublyLinkedList<int>().Remove(1));
    }

    [Fact]
    public void Remove_MissingElement_ReturnsFalse()
    {
        var list = ListOf(1, 2, 3);
        Assert.False(list.Remove(99));
    }

    [Fact]
    public void Remove_MissingElement_CountUnchanged()
    {
        var list = ListOf(1, 2, 3);
        list.Remove(99);
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void Remove_OnlyElement_ListBecomesEmpty()
    {
        var list = ListOf(42);
        Assert.True(list.Remove(42));
        Assert.Equal(0, list.Count);
        Assert.Empty(list);
    }

    [Fact]
    public void Remove_FirstElement_ShiftsHeadForward()
    {
        var list = ListOf(1, 2, 3);
        Assert.True(list.Remove(1));
        Assert.Equal([2, 3], list);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void Remove_MiddleElement_BridgesNeighbors()
    {
        var list = ListOf(1, 2, 3);
        Assert.True(list.Remove(2));
        Assert.Equal([1, 3], list);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void Remove_LastElement_TruncatesTail()
    {
        var list = ListOf(1, 2, 3);
        Assert.True(list.Remove(3));
        Assert.Equal([1, 2], list);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void Remove_DuplicateValues_RemovesOnlyFirstOccurrence()
    {
        var list = ListOf(5, 5, 5);
        Assert.True(list.Remove(5));
        Assert.Equal([5, 5], list);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void Remove_TwoElements_RemoveFirst_OnlySecondRemains()
    {
        var list = ListOf(1, 2);
        Assert.True(list.Remove(1));
        Assert.Equal([2], list);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void Remove_TwoElements_RemoveLast_OnlyFirstRemains()
    {
        var list = ListOf(1, 2);
        Assert.True(list.Remove(2));
        Assert.Equal([1], list);
        Assert.Equal(1, list.Count);
    }

    // ── Contains ────────────────────────────────────────────────────────────

    [Fact]
    public void Contains_EmptyList_ReturnsFalse()
    {
        Assert.False(new DoublyLinkedList<int>().Contains(1));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Contains_ExistingElement_ReturnsTrue(int value)
    {
        Assert.True(ListOf(1, 2, 3).Contains(value));
    }

    [Fact]
    public void Contains_MissingElement_ReturnsFalse()
    {
        Assert.False(ListOf(1, 2, 3).Contains(99));
    }

    [Fact]
    public void Contains_SingleElement_Present_ReturnsTrue()
    {
        Assert.True(ListOf(7).Contains(7));
    }

    [Fact]
    public void Contains_SingleElement_Absent_ReturnsFalse()
    {
        Assert.False(ListOf(7).Contains(8));
    }

    [Fact]
    public void Contains_DuplicateValues_ReturnsTrue()
    {
        Assert.True(ListOf(4, 4, 4).Contains(4));
    }

    [Fact]
    public void Contains_AfterRemove_ReturnsFalse()
    {
        var list = ListOf(1, 2, 3);
        list.Remove(2);
        Assert.False(list.Contains(2));
    }

    [Fact]
    public void Contains_BoundaryValues_ReturnsTrue()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.AddLast(int.MinValue);
        list.AddLast(int.MaxValue);
        Assert.True(list.Contains(int.MinValue));
        Assert.True(list.Contains(int.MaxValue));
    }

    // ── Reverse ─────────────────────────────────────────────────────────────

    [Fact]
    public void Reverse_EmptyList_NoExceptionAndStaysEmpty()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.Reverse();
        Assert.Empty(list);
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void Reverse_SingleElement_ListUnchanged()
    {
        var list = ListOf(42);
        list.Reverse();
        Assert.Equal([42], list);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void Reverse_TwoElements_SwapsOrder()
    {
        var list = ListOf(1, 2);
        list.Reverse();
        Assert.Equal([2, 1], list);
    }

    [Fact]
    public void Reverse_OddLength_ReversesCorrectly()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.Reverse();
        Assert.Equal([5, 4, 3, 2, 1], list);
    }

    [Fact]
    public void Reverse_EvenLength_ReversesCorrectly()
    {
        var list = ListOf(1, 2, 3, 4);
        list.Reverse();
        Assert.Equal([4, 3, 2, 1], list);
    }

    [Fact]
    public void Reverse_TwiceRestoresOriginalOrder()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.Reverse();
        list.Reverse();
        Assert.Equal([1, 2, 3, 4, 5], list);
    }

    [Fact]
    public void Reverse_CountUnchangedAfterReverse()
    {
        var list = ListOf(1, 2, 3);
        list.Reverse();
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void Reverse_ThenAddFirst_AppendsAtNewHead()
    {
        // After reverse, what was the tail is now the head.
        // AddFirst should prepend in front of the new head.
        var list = ListOf(1, 2, 3);
        list.Reverse();               // [3, 2, 1]
        list.AddFirst(4);             // [4, 3, 2, 1]
        Assert.Equal([4, 3, 2, 1], list);
    }

    [Fact]
    public void Reverse_ThenAddLast_AppendsAtNewTail()
    {
        var list = ListOf(1, 2, 3);
        list.Reverse();               // [3, 2, 1]
        list.AddLast(0);              // [3, 2, 1, 0]
        Assert.Equal([3, 2, 1, 0], list);
    }

    [Fact]
    public void Reverse_ThenRemoveFirst_RemovesFromNewHead()
    {
        var list = ListOf(1, 2, 3);
        list.Reverse();               // [3, 2, 1]
        var removed = list.RemoveFirst();
        Assert.Equal(3, removed);
        Assert.Equal([2, 1], list);
    }

    [Fact]
    public void Reverse_ThenRemoveLast_RemovesFromNewTail()
    {
        var list = ListOf(1, 2, 3);
        list.Reverse();               // [3, 2, 1]
        var removed = list.RemoveLast();
        Assert.Equal(1, removed);
        Assert.Equal([3, 2], list);
    }

    // ── Sort ────────────────────────────────────────────────────────────────

    [Fact]
    public void Sort_EmptyList_NoExceptionAndStaysEmpty()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.Sort();
        Assert.Equal(0, list.Count);
        Assert.Empty(list);
    }

    [Theory]
    [MemberData(nameof(SortCases))]
    public void Sort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var list = new SortableDoublyLinkedList<int>();
        foreach (var x in input) list.AddLast(x);
        list.Sort();
        Assert.Equal(expected, list);
    }

    public static IEnumerable<object[]> SortCases =>
    [
        // single element
        [new[] { 42 },                                  new[] { 42 }],
        // two elements — already ordered
        [new[] { 1, 2 },                                new[] { 1, 2 }],
        // two elements — reverse ordered
        [new[] { 2, 1 },                                new[] { 1, 2 }],
        // all identical
        [new[] { 7, 7, 7, 7 },                          new[] { 7, 7, 7, 7 }],
        // already sorted (best case)
        [new[] { 1, 2, 3, 4, 5 },                       new[] { 1, 2, 3, 4, 5 }],
        // reverse sorted (worst case)
        [new[] { 5, 4, 3, 2, 1 },                       new[] { 1, 2, 3, 4, 5 }],
        // general random — odd length
        [new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5 },           new[] { 1, 1, 2, 3, 4, 5, 5, 6, 9 }],
        // general random — even length
        [new[] { 8, 3, 7, 1, 6, 2, 5, 4 },              new[] { 1, 2, 3, 4, 5, 6, 7, 8 }],
        // boundary values
        [new[] { int.MaxValue, 0, int.MinValue },        new[] { int.MinValue, 0, int.MaxValue }],
    ];

    [Fact]
    public void Sort_PreservesCount()
    {
        var list = ListOf(5, 3, 1, 4, 2);
        list.Sort();
        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void Sort_AfterSort_RemoveFirstAndRemoveLastWorkCorrectly()
    {
        // Ensures head and tail pointers are valid after sort.
        var list = ListOf(5, 3, 1, 4, 2);
        list.Sort();                          // [1, 2, 3, 4, 5]
        Assert.Equal(1, list.RemoveFirst());
        Assert.Equal(5, list.RemoveLast());
        Assert.Equal([2, 3, 4], list);
    }

    [Fact]
    public void Sort_AfterSort_AddFirstAndAddLastWorkCorrectly()
    {
        var list = ListOf(3, 1, 2);
        list.Sort();                // [1, 2, 3]
        list.AddFirst(0);
        list.AddLast(4);
        Assert.Equal([0, 1, 2, 3, 4], list);
    }

    [Fact]
    public void Sort_IsStable_EqualElementsPreserveRelativeOrder()
    {
        // Use a list of tuples encoded as ints: sort by first digit, second digit is tie-breaker
        // to observe stability. We use strings to embed stable-sort evidence.
        var list = new SortableDoublyLinkedList<string>();
        list.AddLast("b1");
        list.AddLast("a2");
        list.AddLast("a1");
        list.AddLast("b2");
        list.Sort();
        // Alphabetical: a1, a2, b1, b2 — stable sort preserves insertion order for equal keys.
        Assert.Equal(["a1", "a2", "b1", "b2"], list);
    }

    [Fact]
    public void Sort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(42);
        var input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(10_000)).ToArray();
        var list = new SortableDoublyLinkedList<int>();
        foreach (var x in input) list.AddLast(x);

        list.Sort();

        Assert.Equal(input.OrderBy(x => x), list);
    }

    // ── Enumeration ─────────────────────────────────────────────────────────

    [Fact]
    public void GetEnumerator_EmptyList_YieldsNoElements()
    {
        Assert.Empty(new DoublyLinkedList<int>());
    }

    [Fact]
    public void GetEnumerator_MatchesAddLastInsertionOrder()
    {
        var list = ListOf(10, 20, 30, 40, 50);
        Assert.Equal(new[] { 10, 20, 30, 40, 50 }, list.ToList());
    }

    [Fact]
    public void GetEnumerator_SingleElement_YieldsOneValue()
    {
        var list = ListOf(7);
        Assert.Equal([7], list.ToList());
    }

    [Fact]
    public void GetEnumerator_AfterAddFirstAndAddLast_OrderIsHeadToTail()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.AddLast(2);
        list.AddFirst(1);
        list.AddLast(3);
        Assert.Equal([1, 2, 3], list.ToList());
    }

    [Fact]
    public void GetEnumerator_AfterRemoveOperations_SkipsRemovedElements()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.Remove(2);
        list.Remove(4);
        Assert.Equal([1, 3, 5], list.ToList());
    }

    // ── Mixed operations / integration ──────────────────────────────────────

    [Fact]
    public void AddFirst_AddLast_Remove_Contains_IntegrationScenario()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.AddLast(2);
        list.AddFirst(1);
        list.AddLast(3);
        Assert.Equal(3, list.Count);
        Assert.True(list.Contains(1));
        Assert.True(list.Contains(2));
        Assert.True(list.Contains(3));

        list.Remove(2);
        Assert.Equal(2, list.Count);
        Assert.False(list.Contains(2));
        Assert.Equal([1, 3], list);
    }

    [Fact]
    public void RemoveFirst_RemoveLast_Interleaved_MaintainsCorrectState()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        Assert.Equal(1, list.RemoveFirst());
        Assert.Equal(5, list.RemoveLast());
        Assert.Equal(2, list.RemoveFirst());
        Assert.Equal(4, list.RemoveLast());
        Assert.Equal([3], list);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void SortThenReverse_ProducesDescendingOrder()
    {
        var list = ListOf(3, 1, 4, 1, 5, 9, 2, 6);
        list.Sort();
        list.Reverse();
        Assert.Equal([9, 6, 5, 4, 3, 2, 1, 1], list);
    }

    [Fact]
    public void LargeInput_AddLast_CountAndEnumeration_AreCorrect()
    {
        const int n = 1_000;
        var list = new SortableDoublyLinkedList<int>();
        for (var i = 0; i < n; i++) list.AddLast(i);

        Assert.Equal(n, list.Count);
        Assert.Equal(Enumerable.Range(0, n), list);
    }

    [Fact]
    public void LargeInput_AddFirst_CountAndEnumeration_AreCorrect()
    {
        const int n = 1_000;
        var list = new SortableDoublyLinkedList<int>();
        for (var i = 0; i < n; i++) list.AddFirst(i);

        Assert.Equal(n, list.Count);
        Assert.Equal(Enumerable.Range(0, n).Reverse(), list);
    }

    [Fact]
    public void LargeInput_RemoveFirst_AllElements_CountDropsToZero()
    {
        const int n = 1_000;
        var list = new SortableDoublyLinkedList<int>();
        for (var i = 0; i < n; i++) list.AddLast(i);

        for (var i = 0; i < n; i++) list.RemoveFirst();

        Assert.Equal(0, list.Count);
        Assert.Empty(list);
    }

    [Fact]
    public void LargeInput_RemoveLast_AllElements_CountDropsToZero()
    {
        const int n = 1_000;
        var list = new SortableDoublyLinkedList<int>();
        for (var i = 0; i < n; i++) list.AddLast(i);

        for (var i = 0; i < n; i++) list.RemoveLast();

        Assert.Equal(0, list.Count);
        Assert.Empty(list);
    }

    [Fact]
    public void LargeInput_Reverse_ProducesCorrectOrder()
    {
        const int n = 1_000;
        var list = new SortableDoublyLinkedList<int>();
        for (var i = 0; i < n; i++) list.AddLast(i);

        list.Reverse();

        Assert.Equal(Enumerable.Range(0, n).Reverse(), list);
    }

    [Fact]
    public void LargeInput_Contains_BoundaryValues_AreFound()
    {
        var list = new SortableDoublyLinkedList<int>();
        list.AddFirst(int.MinValue);
        list.AddLast(int.MaxValue);
        for (var i = 1; i < 999; i++) list.AddLast(i);

        Assert.True(list.Contains(int.MinValue));
        Assert.True(list.Contains(int.MaxValue));
        Assert.False(list.Contains(int.MaxValue - 1));  // not inserted
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static SortableDoublyLinkedList<int> ListOf(params int[] values)
    {
        var list = new SortableDoublyLinkedList<int>();
        foreach (var v in values) list.AddLast(v);
        return list;
    }
}
