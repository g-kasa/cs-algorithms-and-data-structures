namespace Algorithms.Lists.Tests;

public class SkipListTests
{
    // ── Count ───────────────────────────────────────────────────────────────

    [Fact]
    public void Count_EmptyList_IsZero()
    {
        var list = new SkipList<int>();
        Assert.Empty(list);
    }

    [Fact]
    public void Count_AfterInserts_ReflectsAllInsertionsIncludingDuplicates()
    {
        var list = new SkipList<int>();
        list.Insert(5);
        list.Insert(5);
        list.Insert(10);
        Assert.Equal(3, list.Count);
    }

    // ── Contains ────────────────────────────────────────────────────────────

    [Fact]
    public void Contains_EmptyList_ReturnsFalse()
    {
        Assert.DoesNotContain(42, new SkipList<int>());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void Contains_SingleInsertedElement_ReturnsTrue(int value)
    {
        var list = new SkipList<int>();
        list.Insert(value);
        Assert.Contains(value, list);
    }

    [Fact]
    public void Contains_ValueNotInserted_ReturnsFalse()
    {
        var list = SkipListOf(1, 2, 3);
        Assert.DoesNotContain(99, list);
    }

    [Fact]
    public void Contains_AfterInsertThenRemove_ReturnsFalse()
    {
        var list = new SkipList<int>();
        list.Insert(7);
        list.Remove(7);
        Assert.DoesNotContain(7, list);
    }

    // ── Insert ──────────────────────────────────────────────────────────────

    [Fact]
    public void Insert_SingleElement_CountIsOne()
    {
        var list = new SkipList<int>();
        list.Insert(42);
        Assert.Single(list);
    }

    [Fact]
    public void Insert_TwoDistinctElements_BothPresent()
    {
        var list = new SkipList<int>();
        list.Insert(1);
        list.Insert(2);
        Assert.Contains(1, list);
        Assert.Contains(2, list);
        Assert.Equal(2, list.Count);
    }

    [Theory]
    [MemberData(nameof(InsertOrderCases))]
    public void Insert_VariousOrders_AllElementsContained(int[] values)
    {
        var list = new SkipList<int>();
        foreach (var v in values) list.Insert(v);
        foreach (var v in values) Assert.Contains(v, list);
    }

    public static IEnumerable<object[]> InsertOrderCases =>
    [
        [new[] { 1, 2, 3, 4, 5 }],           // pre-sorted
        [new[] { 5, 4, 3, 2, 1 }],           // reverse-sorted
        [new[] { 3, 1, 4, 1, 5, 9, 2, 6 }],  // random with duplicate
        [new[] { -10, 0, 10, -5, 5 }],        // mixed negative/positive
    ];

    [Fact]
    public void Insert_AllIdenticalElements_CountEqualsNumberOfInserts()
    {
        var list = new SkipList<int>();
        for (int i = 0; i < 5; i++) list.Insert(7);
        Assert.Equal(5, list.Count);
        Assert.Contains(7, list);
    }

    [Fact]
    public void Insert_BoundaryValues_BothContained()
    {
        var list = new SkipList<int>();
        list.Insert(int.MinValue);
        list.Insert(int.MaxValue);
        Assert.Contains(int.MinValue, list);
        Assert.Contains(int.MaxValue, list);
        Assert.Equal(2, list.Count);
    }

    // ── Remove ──────────────────────────────────────────────────────────────

    [Fact]
    public void Remove_EmptyList_ReturnsFalse()
    {
        Assert.False(new SkipList<int>().Remove(1));
    }

    [Fact]
    public void Remove_EmptyList_DoesNotThrow()
    {
        var list = new SkipList<int>();
        var ex = Record.Exception(() => list.Remove(99));
        Assert.Null(ex);
    }

    [Fact]
    public void Remove_ValueNotPresent_ReturnsFalse()
    {
        var list = SkipListOf(1, 2, 3);
        Assert.False(list.Remove(99));
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void Remove_OnlyElement_ReturnsTrueAndCountIsZero()
    {
        var list = new SkipList<int>();
        list.Insert(42);
        Assert.True(list.Remove(42));
        Assert.Empty(list);
        Assert.DoesNotContain(42, list);
    }

    [Fact]
    public void Remove_FirstOfTwoElements_OtherElementStillPresent()
    {
        var list = new SkipList<int>();
        list.Insert(1);
        list.Insert(2);
        Assert.True(list.Remove(1));
        Assert.DoesNotContain(1, list);
        Assert.Contains(2, list);
        Assert.Single(list);
    }

    [Fact]
    public void Remove_SecondOfTwoElements_OtherElementStillPresent()
    {
        var list = new SkipList<int>();
        list.Insert(1);
        list.Insert(2);
        Assert.True(list.Remove(2));
        Assert.DoesNotContain(2, list);
        Assert.Contains(1, list);
        Assert.Single(list);
    }

    [Fact]
    public void Remove_MiddleElement_OtherElementsStillPresent()
    {
        var list = SkipListOf(10, 20, 30);
        Assert.True(list.Remove(20));
        Assert.DoesNotContain(20, list);
        Assert.Contains(10, list);
        Assert.Contains(30, list);
        Assert.Equal(2, list.Count);
    }

    // ── Duplicate insert / remove semantics ─────────────────────────────────

    [Fact]
    public void Remove_DuplicateInserted_OnlyOneOccurrenceRemoved()
    {
        var list = new SkipList<int>();
        list.Insert(5);
        list.Insert(5);

        Assert.True(list.Remove(5));
        Assert.Single(list);
        Assert.Contains(5, list); // one copy remains
    }

    [Fact]
    public void Remove_DuplicateTwice_BothOccurrencesGone()
    {
        var list = new SkipList<int>();
        list.Insert(5);
        list.Insert(5);

        Assert.True(list.Remove(5));
        Assert.True(list.Remove(5));
        Assert.Empty(list);
        Assert.DoesNotContain(5, list);
    }

    [Fact]
    public void Remove_MoreTimesThanInserted_ThirdRemoveReturnsFalse()
    {
        var list = new SkipList<int>();
        list.Insert(5);
        list.Insert(5);

        list.Remove(5);
        list.Remove(5);
        Assert.False(list.Remove(5));
        Assert.Empty(list);
    }

    [Fact]
    public void Insert_ThreeDuplicates_CountAndEnumerationMatchExpected()
    {
        var list = new SkipList<int>();
        list.Insert(3);
        list.Insert(3);
        list.Insert(3);

        Assert.Equal(3, list.Count);
        Assert.Equal([3, 3, 3], list.ToList());
    }

    // ── Remove all elements ─────────────────────────────────────────────────

    [Fact]
    public void Remove_AllElements_CountIsZeroAndContainsFalse()
    {
        var values = new[] { 4, 2, 7, 1, 9 };
        var list = SkipListOf(values);

        foreach (var v in values) list.Remove(v);

        Assert.Empty(list);
        foreach (var v in values) Assert.DoesNotContain(v, list);
    }

    [Fact]
    public void Remove_AllElements_EnumerationIsEmpty()
    {
        var list = SkipListOf(1, 2, 3);
        list.Remove(1);
        list.Remove(2);
        list.Remove(3);
        Assert.Empty(list);
    }

    // ── Enumeration / ascending order ────────────────────────────────────────

    [Fact]
    public void GetEnumerator_EmptyList_YieldsNothing()
    {
        Assert.Empty(new SkipList<int>());
    }

    [Fact]
    public void GetEnumerator_SingleElement_YieldsThatElement()
    {
        var list = new SkipList<int>();
        list.Insert(99);
        Assert.Equal([99], list.ToList());
    }

    [Fact]
    public void GetEnumerator_TwoElements_YieldsAscending()
    {
        var list = new SkipList<int>();
        list.Insert(2);
        list.Insert(1);
        Assert.Equal([1, 2], list.ToList());
    }

    [Theory]
    [MemberData(nameof(EnumerationOrderCases))]
    public void GetEnumerator_AnyInsertionOrder_YieldsAscendingSorted(int[] values)
    {
        var list = new SkipList<int>();
        foreach (var v in values) list.Insert(v);
        var expected = values.OrderBy(x => x).ToList();
        Assert.Equal(expected, list.ToList());
    }

    public static IEnumerable<object[]> EnumerationOrderCases =>
    [
        [new[] { 3, 1, 4, 1, 5, 9, 2, 6 }],           // random with duplicate
        [new[] { 5, 4, 3, 2, 1 }],                     // reverse-sorted
        [new[] { 1, 2, 3, 4, 5 }],                     // pre-sorted
        [new[] { 7, 7, 7 }],                            // all identical
        [new[] { -5, 15, 0, -100, 42 }],                // mixed negatives
        [new[] { int.MinValue, 0, int.MaxValue }],       // boundary values
    ];

    [Fact]
    public void GetEnumerator_AfterRemoval_YieldsRemainingElementsAscending()
    {
        var list = SkipListOf(5, 3, 8, 1, 9, 2);
        list.Remove(3);
        list.Remove(9);
        Assert.Equal([1, 2, 5, 8], list.ToList());
    }

    [Fact]
    public void GetEnumerator_StringElements_YieldsAscendingLexicographic()
    {
        var list = new SkipList<string>();
        list.Insert("banana");
        list.Insert("apple");
        list.Insert("cherry");
        Assert.Equal(["apple", "banana", "cherry"], list.ToList());
    }

    // ── Pre-sorted insertion ─────────────────────────────────────────────────

    [Fact]
    public void Insert_PreSortedInput_ContainsAllAndEnumeratesAscending()
    {
        var values = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var list = SkipListOf(values);

        Assert.Equal(10, list.Count);
        Assert.Equal(values, list.ToList());
    }

    // ── Reverse-sorted insertion ─────────────────────────────────────────────

    [Fact]
    public void Insert_ReverseSortedInput_ContainsAllAndEnumeratesAscending()
    {
        var values = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
        var list = SkipListOf(values);

        Assert.Equal(10, list.Count);
        Assert.Equal(values.OrderBy(x => x).ToList(), list.ToList());
    }

    // ── All identical elements ───────────────────────────────────────────────

    [Fact]
    public void Insert_AllIdenticalElements_EnumeratesAllCopies()
    {
        var list = new SkipList<int>();
        for (int i = 0; i < 4; i++) list.Insert(42);

        Assert.Equal([42, 42, 42, 42], list.ToList());
    }

    // ── Large input ─────────────────────────────────────────────────────────

    [Fact]
    public void Insert_1000RandomIntegers_CountCorrectAndEnumerationSorted()
    {
        var rng = new Random(1337);
        var values = Enumerable.Range(0, 1_000).Select(_ => rng.Next(100_000)).ToArray();

        var list = new SkipList<int>();
        foreach (var v in values) list.Insert(v);

        Assert.Equal(1_000, list.Count);
        var expected = values.OrderBy(x => x).ToList();
        Assert.Equal(expected, list.ToList());
    }

    [Fact]
    public void Contains_1000RandomIntegers_AllInsertedValuesFound()
    {
        var rng = new Random(2024);
        var values = Enumerable.Range(0, 1_000).Select(_ => rng.Next(100_000)).Distinct().ToArray();

        var list = new SkipList<int>();
        foreach (var v in values) list.Insert(v);

        foreach (var v in values) Assert.Contains(v, list);
    }

    [Fact]
    public void Remove_500Of1000Elements_CountAndEnumerationCorrect()
    {
        var rng = new Random(9999);
        var values = Enumerable.Range(0, 1_000).Select(_ => rng.Next(10_000)).ToArray();

        var list = new SkipList<int>();
        foreach (var v in values) list.Insert(v);

        // Remove every element that appears at an even index (may remove duplicates one at a time)
        var toRemove = values.Where((_, i) => i % 2 == 0).ToArray();
        foreach (var v in toRemove) list.Remove(v);

        // After removals, Count must reflect exact number of successful removes.
        // Build expected by replaying the same operations on a sorted multiset.
        var remaining = values.ToList();
        foreach (var v in toRemove)
        {
            var idx = remaining.IndexOf(v);
            if (idx >= 0) remaining.RemoveAt(idx);
        }

        Assert.Equal(remaining.Count, list.Count);
        Assert.Equal(remaining.OrderBy(x => x).ToList(), list.ToList());
    }

    [Fact]
    public void Insert_1000ElementsPresortedDescending_EnumeratesAscending()
    {
        var values = Enumerable.Range(1, 1_000).Reverse().ToArray();

        var list = new SkipList<int>();
        foreach (var v in values) list.Insert(v);

        Assert.Equal(1_000, list.Count);
        Assert.Equal(Enumerable.Range(1, 1_000).ToList(), list.ToList());
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static SkipList<int> SkipListOf(params int[] values)
    {
        var list = new SkipList<int>();
        foreach (var v in values) list.Insert(v);
        return list;
    }
}
