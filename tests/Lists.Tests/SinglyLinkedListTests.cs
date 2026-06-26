namespace Algorithms.Lists.Tests;

public class SinglyLinkedListTests
{
    // ── AddFirst ────────────────────────────────────────────────────────────

    [Fact]
    public void AddFirst_OnEmptyList_CountIsOne()
    {
        var list = new SortableSinglyLinkedList<int>();
        list.AddFirst(1);
        Assert.Single(list);
    }

    [Fact]
    public void AddFirst_MultipleElements_PrependsInOrder()
    {
        var list = new SortableSinglyLinkedList<int>();
        list.AddFirst(3);
        list.AddFirst(2);
        list.AddFirst(1);
        Assert.Equal([1, 2, 3], list);
    }

    // ── AddLast ─────────────────────────────────────────────────────────────

    [Fact]
    public void AddLast_MultipleElements_AppendsInOrder()
    {
        var list = new SortableSinglyLinkedList<int>();
        list.AddLast(1);
        list.AddLast(2);
        list.AddLast(3);
        Assert.Equal([1, 2, 3], list);
    }

    // ── Remove ──────────────────────────────────────────────────────────────

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
    }

    [Fact]
    public void Remove_LastElement_TruncatesTail()
    {
        var list = ListOf(1, 2, 3);
        Assert.True(list.Remove(3));
        Assert.Equal([1, 2], list);
    }

    [Fact]
    public void Remove_MissingElement_ReturnsFalse()
    {
        var list = ListOf(1, 2, 3);
        Assert.False(list.Remove(99));
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void Remove_EmptyList_ReturnsFalse()
    {
        Assert.False(new SinglyLinkedList<int>().Remove(1));
    }

    [Fact]
    public void Remove_OnlyElement_ListBecomesEmpty()
    {
        var list = ListOf(42);
        Assert.True(list.Remove(42));
        Assert.Empty(list);
        Assert.Empty(list);
    }

    // ── Contains ────────────────────────────────────────────────────────────

    [Fact]
    public void Contains_ExistingElement_ReturnsTrue()
    {
        Assert.Contains(2, ListOf(1, 2, 3));
    }

    [Fact]
    public void Contains_MissingElement_ReturnsFalse()
    {
        Assert.DoesNotContain(99, ListOf(1, 2, 3));
    }

    [Fact]
    public void Contains_EmptyList_ReturnsFalse()
    {
        Assert.DoesNotContain(1, new SinglyLinkedList<int>());
    }

    // ── Reverse ─────────────────────────────────────────────────────────────

    [Fact]
    public void Reverse_MultipleElements_ReversesOrder()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.Reverse();
        Assert.Equal([5, 4, 3, 2, 1], list);
    }

    [Fact]
    public void Reverse_SingleElement_ListUnchanged()
    {
        var list = ListOf(42);
        list.Reverse();
        Assert.Equal([42], list);
    }

    [Fact]
    public void Reverse_EmptyList_NoException()
    {
        var list = new SortableSinglyLinkedList<int>();
        list.Reverse();
        Assert.Empty(list);
    }

    [Fact]
    public void Reverse_TwiceRestoresOriginalOrder()
    {
        var list = ListOf(1, 2, 3);
        list.Reverse();
        list.Reverse();
        Assert.Equal([1, 2, 3], list);
    }

    // ── Sort ────────────────────────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(SortCases))]
    public void Sort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var list = new SortableSinglyLinkedList<int>();
        foreach (var x in input) list.AddLast(x);
        list.Sort();
        Assert.Equal(expected, list);
    }

    public static IEnumerable<object[]> SortCases =>
    [
        [new[] { 3, 1, 4, 1, 5, 9, 2, 6 }, new[] { 1, 1, 2, 3, 4, 5, 6, 9 }],  // random
        [new[] { 5, 4, 3, 2, 1 },           new[] { 1, 2, 3, 4, 5 }],            // reverse sorted
        [new[] { 1, 2, 3, 4, 5 },           new[] { 1, 2, 3, 4, 5 }],            // already sorted
        [new[] { 7, 7, 7, 7 },              new[] { 7, 7, 7, 7 }],               // all identical
        [new[] { 42 },                       new[] { 42 }],                        // single element
        [new[] { 2, 1 },                     new[] { 1, 2 }],                      // two elements
    ];

    [Fact]
    public void Sort_EmptyList_NoException()
    {
        var list = new SortableSinglyLinkedList<int>();
        list.Sort();
        Assert.Empty(list);
    }

    [Fact]
    public void Sort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(42);
        var input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(10_000)).ToArray();
        var list = new SortableSinglyLinkedList<int>();
        foreach (var x in input) list.AddLast(x);

        list.Sort();

        Assert.Equal(input.OrderBy(x => x), list);
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static SortableSinglyLinkedList<int> ListOf(params int[] values)
    {
        var list = new SortableSinglyLinkedList<int>();
        foreach (var v in values) list.AddLast(v);
        return list;
    }
}
