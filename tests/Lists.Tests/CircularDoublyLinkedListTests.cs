namespace Algorithms.Lists.Tests;

public class CircularDoublyLinkedListTests
{
    // ── AddFirst ────────────────────────────────────────────────────────────

    [Fact]
    public void AddFirst_OnEmptyList_CountIsOne()
    {
        var list = new CircularDoublyLinkedList<int>();
        list.AddFirst(42);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void AddFirst_OnEmptyList_SingleElementEnumerates()
    {
        var list = new CircularDoublyLinkedList<int>();
        list.AddFirst(42);
        Assert.Equal([42], list);
    }

    [Fact]
    public void AddFirst_MultipleElements_PrependsInReverseOrder()
    {
        var list = new CircularDoublyLinkedList<int>();
        list.AddFirst(3);
        list.AddFirst(2);
        list.AddFirst(1);
        Assert.Equal([1, 2, 3], list);
    }

    [Fact]
    public void AddFirst_TwoElements_CountIsTwo()
    {
        var list = new CircularDoublyLinkedList<int>();
        list.AddFirst(2);
        list.AddFirst(1);
        Assert.Equal(2, list.Count);
    }

    [Theory]
    [InlineData(new[] { 5 },       new[] { 5 })]
    [InlineData(new[] { 2, 1 },    new[] { 1, 2 })]
    [InlineData(new[] { 3, 2, 1 }, new[] { 1, 2, 3 })]
    public void AddFirst_Theory_EnumerationMatchesExpected(int[] addOrder, int[] expected)
    {
        var list = new CircularDoublyLinkedList<int>();
        foreach (var v in addOrder) list.AddFirst(v);
        Assert.Equal(expected, list);
    }

    // ── AddLast ─────────────────────────────────────────────────────────────

    [Fact]
    public void AddLast_OnEmptyList_CountIsOne()
    {
        var list = new CircularDoublyLinkedList<int>();
        list.AddLast(99);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void AddLast_MultipleElements_AppendsInOrder()
    {
        var list = new CircularDoublyLinkedList<int>();
        list.AddLast(1);
        list.AddLast(2);
        list.AddLast(3);
        Assert.Equal([1, 2, 3], list);
    }

    [Fact]
    public void AddLast_TwoElements_EnumeratesInInsertionOrder()
    {
        var list = new CircularDoublyLinkedList<int>();
        list.AddLast(10);
        list.AddLast(20);
        Assert.Equal([10, 20], list);
    }

    [Theory]
    [InlineData(new[] { 1 },       new[] { 1 })]
    [InlineData(new[] { 1, 2 },    new[] { 1, 2 })]
    [InlineData(new[] { 1, 2, 3 }, new[] { 1, 2, 3 })]
    public void AddLast_Theory_EnumerationMatchesExpected(int[] addOrder, int[] expected)
    {
        var list = new CircularDoublyLinkedList<int>();
        foreach (var v in addOrder) list.AddLast(v);
        Assert.Equal(expected, list);
    }

    // ── AddFirst / AddLast interleaved ───────────────────────────────────────

    [Fact]
    public void AddFirstAndLast_Interleaved_CorrectOrder()
    {
        var list = new CircularDoublyLinkedList<int>();
        list.AddLast(2);
        list.AddFirst(1);
        list.AddLast(3);
        list.AddFirst(0);
        Assert.Equal([0, 1, 2, 3], list);
        Assert.Equal(4, list.Count);
    }

    // ── RemoveFirst ─────────────────────────────────────────────────────────

    [Fact]
    public void RemoveFirst_EmptyList_ThrowsInvalidOperationException()
    {
        var list = new CircularDoublyLinkedList<int>();
        Assert.Throws<InvalidOperationException>(() => list.RemoveFirst());
    }

    [Fact]
    public void RemoveFirst_SingleElement_ReturnsValueAndListBecomesEmpty()
    {
        var list = ListOf(7);
        var removed = list.RemoveFirst();
        Assert.Equal(7, removed);
        Assert.Equal(0, list.Count);
        Assert.Empty(list);
    }

    [Fact]
    public void RemoveFirst_TwoElements_ReturnsHeadAndLeavesTail()
    {
        var list = ListOf(1, 2);
        var removed = list.RemoveFirst();
        Assert.Equal(1, removed);
        Assert.Equal([2], list);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void RemoveFirst_MultipleElements_ShiftsHeadForward()
    {
        var list = ListOf(1, 2, 3, 4);
        Assert.Equal(1, list.RemoveFirst());
        Assert.Equal([2, 3, 4], list);
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void RemoveFirst_AllElements_DrainsList()
    {
        var list = ListOf(1, 2, 3);
        list.RemoveFirst();
        list.RemoveFirst();
        list.RemoveFirst();
        Assert.Equal(0, list.Count);
        Assert.Empty(list);
    }

    [Fact]
    public void RemoveFirst_AfterDraining_ThrowsInvalidOperationException()
    {
        var list = ListOf(1);
        list.RemoveFirst();
        Assert.Throws<InvalidOperationException>(() => list.RemoveFirst());
    }

    // ── RemoveLast ──────────────────────────────────────────────────────────

    [Fact]
    public void RemoveLast_EmptyList_ThrowsInvalidOperationException()
    {
        var list = new CircularDoublyLinkedList<int>();
        Assert.Throws<InvalidOperationException>(() => list.RemoveLast());
    }

    [Fact]
    public void RemoveLast_SingleElement_ReturnsValueAndListBecomesEmpty()
    {
        var list = ListOf(7);
        var removed = list.RemoveLast();
        Assert.Equal(7, removed);
        Assert.Equal(0, list.Count);
        Assert.Empty(list);
    }

    [Fact]
    public void RemoveLast_TwoElements_ReturnsTailAndLeavesHead()
    {
        var list = ListOf(1, 2);
        var removed = list.RemoveLast();
        Assert.Equal(2, removed);
        Assert.Equal([1], list);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void RemoveLast_MultipleElements_TruncatesTail()
    {
        var list = ListOf(1, 2, 3, 4);
        Assert.Equal(4, list.RemoveLast());
        Assert.Equal([1, 2, 3], list);
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void RemoveLast_AllElements_DrainsList()
    {
        var list = ListOf(1, 2, 3);
        list.RemoveLast();
        list.RemoveLast();
        list.RemoveLast();
        Assert.Equal(0, list.Count);
        Assert.Empty(list);
    }

    [Fact]
    public void RemoveLast_AfterDraining_ThrowsInvalidOperationException()
    {
        var list = ListOf(1);
        list.RemoveLast();
        Assert.Throws<InvalidOperationException>(() => list.RemoveLast());
    }

    // ── Remove(value) ───────────────────────────────────────────────────────

    [Fact]
    public void Remove_EmptyList_ReturnsFalse()
    {
        Assert.False(new CircularDoublyLinkedList<int>().Remove(1));
    }

    [Fact]
    public void Remove_ValueNotPresent_ReturnsFalse()
    {
        var list = ListOf(1, 2, 3);
        Assert.False(list.Remove(99));
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
    public void Remove_FirstElement_ShiftsHead()
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
    public void Remove_FirstOccurrenceOfDuplicate_LeavesRemaining()
    {
        var list = ListOf(5, 5, 5);
        Assert.True(list.Remove(5));
        Assert.Equal([5, 5], list);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void Remove_TwoElements_RemoveHead_LeavesOnlyTail()
    {
        var list = ListOf(10, 20);
        Assert.True(list.Remove(10));
        Assert.Equal([20], list);
    }

    [Fact]
    public void Remove_TwoElements_RemoveTail_LeavesOnlyHead()
    {
        var list = ListOf(10, 20);
        Assert.True(list.Remove(20));
        Assert.Equal([10], list);
    }

    // ── Contains ────────────────────────────────────────────────────────────

    [Fact]
    public void Contains_EmptyList_ReturnsFalse()
    {
        Assert.False(new CircularDoublyLinkedList<int>().Contains(1));
    }

    [Fact]
    public void Contains_SingleElement_PresentValue_ReturnsTrue()
    {
        Assert.True(ListOf(42).Contains(42));
    }

    [Fact]
    public void Contains_SingleElement_AbsentValue_ReturnsFalse()
    {
        Assert.False(ListOf(42).Contains(0));
    }

    [Theory]
    [InlineData(1,  true)]
    [InlineData(3,  true)]
    [InlineData(5,  true)]
    [InlineData(0,  false)]
    [InlineData(99, false)]
    public void Contains_Theory_VariousValues(int value, bool expected)
    {
        var list = ListOf(1, 2, 3, 4, 5);
        Assert.Equal(expected, list.Contains(value));
    }

    [Fact]
    public void Contains_DuplicateElements_ReturnsTrue()
    {
        var list = ListOf(7, 7, 7);
        Assert.True(list.Contains(7));
    }

    [Fact]
    public void Contains_AfterRemoval_ReturnsFalse()
    {
        var list = ListOf(1, 2, 3);
        list.Remove(2);
        Assert.False(list.Contains(2));
    }

    [Fact]
    public void Contains_BoundaryValues_IntMinAndMax()
    {
        var list = ListOf(int.MinValue, 0, int.MaxValue);
        Assert.True(list.Contains(int.MinValue));
        Assert.True(list.Contains(int.MaxValue));
        Assert.False(list.Contains(1));
    }

    // ── Rotate ──────────────────────────────────────────────────────────────

    [Fact]
    public void Rotate_EmptyList_NoException()
    {
        var list = new CircularDoublyLinkedList<int>();
        list.Rotate(3);  // must not throw
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void Rotate_SingleElement_NoChange()
    {
        var list = ListOf(1);
        list.Rotate(1);
        Assert.Equal([1], list);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void Rotate_ZeroSteps_NoChange()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.Rotate(0);
        Assert.Equal([1, 2, 3, 4, 5], list);
    }

    [Fact]
    public void Rotate_ByCount_FullCycle_NoChange()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.Rotate(5);
        Assert.Equal([1, 2, 3, 4, 5], list);
    }

    [Fact]
    public void Rotate_ByMultipleOfCount_NoChange()
    {
        var list = ListOf(1, 2, 3);
        list.Rotate(9);  // 9 % 3 == 0
        Assert.Equal([1, 2, 3], list);
    }

    [Fact]
    public void Rotate_LeftByOne_MovesHeadToTail()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.Rotate(1);
        Assert.Equal([2, 3, 4, 5, 1], list);
    }

    [Fact]
    public void Rotate_LeftByTwo_ShiftsFirstTwoToEnd()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.Rotate(2);
        Assert.Equal([3, 4, 5, 1, 2], list);
    }

    [Fact]
    public void Rotate_LeftByCountMinusOne_EquivalentToRightByOne()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.Rotate(4);  // left 4 == right 1
        Assert.Equal([5, 1, 2, 3, 4], list);
    }

    [Fact]
    public void Rotate_NegativeOne_RotatesRight_MovesTailToHead()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.Rotate(-1);
        Assert.Equal([5, 1, 2, 3, 4], list);
    }

    [Fact]
    public void Rotate_NegativeTwo_RotatesRightByTwo()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.Rotate(-2);
        Assert.Equal([4, 5, 1, 2, 3], list);
    }

    [Fact]
    public void Rotate_StepsGreaterThanCount_WrapsAround()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.Rotate(7);  // 7 % 5 == 2
        Assert.Equal([3, 4, 5, 1, 2], list);
    }

    [Fact]
    public void Rotate_NegativeStepsGreaterThanCount_WrapsAround()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.Rotate(-7);  // -7 % 5 normalises to 3 left == 2 right
        Assert.Equal([4, 5, 1, 2, 3], list);
    }

    [Fact]
    public void Rotate_TwoElements_LeftByOne_SwapsElements()
    {
        var list = ListOf(1, 2);
        list.Rotate(1);
        Assert.Equal([2, 1], list);
    }

    [Fact]
    public void Rotate_TwiceInSequence_AccumulatesCorrectly()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.Rotate(1);  // [2,3,4,5,1]
        list.Rotate(2);  // [4,5,1,2,3]
        Assert.Equal([4, 5, 1, 2, 3], list);
    }

    [Fact]
    public void Rotate_CountIsUnchangedAfterRotation()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.Rotate(3);
        Assert.Equal(5, list.Count);
    }

    [Theory]
    [InlineData(0, new[] { 1, 2, 3, 4, 5 })]
    [InlineData(1, new[] { 2, 3, 4, 5, 1 })]
    [InlineData(2, new[] { 3, 4, 5, 1, 2 })]
    [InlineData(3, new[] { 4, 5, 1, 2, 3 })]
    [InlineData(4, new[] { 5, 1, 2, 3, 4 })]
    [InlineData(5, new[] { 1, 2, 3, 4, 5 })]
    public void Rotate_Theory_LeftRotationsByStep(int steps, int[] expected)
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.Rotate(steps);
        Assert.Equal(expected, list);
    }

    [Theory]
    [InlineData(-1, new[] { 5, 1, 2, 3, 4 })]
    [InlineData(-2, new[] { 4, 5, 1, 2, 3 })]
    [InlineData(-3, new[] { 3, 4, 5, 1, 2 })]
    [InlineData(-4, new[] { 2, 3, 4, 5, 1 })]
    [InlineData(-5, new[] { 1, 2, 3, 4, 5 })]
    public void Rotate_Theory_RightRotationsByNegativeStep(int steps, int[] expected)
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.Rotate(steps);
        Assert.Equal(expected, list);
    }

    // ── Count ───────────────────────────────────────────────────────────────

    [Fact]
    public void Count_NewList_IsZero()
    {
        Assert.Equal(0, new CircularDoublyLinkedList<int>().Count);
    }

    [Fact]
    public void Count_AfterAddFirst_IncrementsByOne()
    {
        var list = new CircularDoublyLinkedList<int>();
        list.AddFirst(1);
        Assert.Equal(1, list.Count);
        list.AddFirst(2);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void Count_AfterAddLast_IncrementsByOne()
    {
        var list = new CircularDoublyLinkedList<int>();
        list.AddLast(1);
        Assert.Equal(1, list.Count);
        list.AddLast(2);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void Count_AfterRemoveFirst_DecrementsByOne()
    {
        var list = ListOf(1, 2, 3);
        list.RemoveFirst();
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void Count_AfterRemoveLast_DecrementsByOne()
    {
        var list = ListOf(1, 2, 3);
        list.RemoveLast();
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void Count_AfterSuccessfulRemove_DecrementsByOne()
    {
        var list = ListOf(1, 2, 3);
        list.Remove(2);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void Count_AfterFailedRemove_Unchanged()
    {
        var list = ListOf(1, 2, 3);
        list.Remove(99);
        Assert.Equal(3, list.Count);
    }

    // ── GetEnumerator / enumeration order ───────────────────────────────────

    [Fact]
    public void GetEnumerator_EmptyList_YieldsNoElements()
    {
        Assert.Empty(new CircularDoublyLinkedList<int>());
    }

    [Fact]
    public void GetEnumerator_SingleElement_YieldsExactlyThatElement()
    {
        Assert.Equal([99], ListOf(99));
    }

    [Fact]
    public void GetEnumerator_MultipleElements_YieldsInHeadToTailOrder()
    {
        Assert.Equal([10, 20, 30, 40], ListOf(10, 20, 30, 40));
    }

    [Fact]
    public void GetEnumerator_AllDuplicates_YieldsAllCopies()
    {
        Assert.Equal([3, 3, 3, 3], ListOf(3, 3, 3, 3));
    }

    [Fact]
    public void GetEnumerator_PreSortedInput_YieldsInSameOrder()
    {
        Assert.Equal([1, 2, 3, 4, 5], ListOf(1, 2, 3, 4, 5));
    }

    [Fact]
    public void GetEnumerator_ReverseSortedInput_YieldsInSameOrder()
    {
        Assert.Equal([5, 4, 3, 2, 1], ListOf(5, 4, 3, 2, 1));
    }

    [Fact]
    public void GetEnumerator_AfterRotate_YieldsUpdatedOrder()
    {
        var list = ListOf(1, 2, 3, 4, 5);
        list.Rotate(2);
        Assert.Equal([3, 4, 5, 1, 2], list);
    }

    [Fact]
    public void GetEnumerator_BoundaryValues_IntMinAndMax()
    {
        var list = ListOf(int.MinValue, 0, int.MaxValue);
        Assert.Equal([int.MinValue, 0, int.MaxValue], list);
    }

    // ── Duplicates / all-identical elements ─────────────────────────────────

    [Fact]
    public void AllIdentical_AddAndEnumerate_AllCopiesPresent()
    {
        var list = ListOf(9, 9, 9, 9, 9);
        Assert.Equal(5, list.Count);
        Assert.Equal([9, 9, 9, 9, 9], list);
    }

    [Fact]
    public void AllIdentical_RemoveFirst_DecreasesCount()
    {
        var list = ListOf(9, 9, 9);
        list.RemoveFirst();
        Assert.Equal(2, list.Count);
        Assert.Equal([9, 9], list);
    }

    [Fact]
    public void AllIdentical_Remove_RemovesOnlyFirstOccurrence()
    {
        var list = ListOf(9, 9, 9);
        Assert.True(list.Remove(9));
        Assert.Equal(2, list.Count);
    }

    // ── Large input ─────────────────────────────────────────────────────────

    [Fact]
    public void AddLast_LargeInput_CountMatchesInsertions()
    {
        const int n = 1_000;
        var list = new CircularDoublyLinkedList<int>();
        for (int i = 0; i < n; i++) list.AddLast(i);
        Assert.Equal(n, list.Count);
    }

    [Fact]
    public void AddLast_LargeInput_EnumerationMatchesInsertionOrder()
    {
        const int n = 1_000;
        var list = new CircularDoublyLinkedList<int>();
        for (int i = 0; i < n; i++) list.AddLast(i);
        Assert.Equal(Enumerable.Range(0, n), list);
    }

    [Fact]
    public void AddFirst_LargeInput_EnumerationIsReversed()
    {
        const int n = 1_000;
        var list = new CircularDoublyLinkedList<int>();
        for (int i = 0; i < n; i++) list.AddFirst(i);
        Assert.Equal(Enumerable.Range(0, n).Reverse(), list);
    }

    [Fact]
    public void RemoveFirst_LargeInput_DrainsToEmpty()
    {
        const int n = 1_000;
        var list = new CircularDoublyLinkedList<int>();
        for (int i = 0; i < n; i++) list.AddLast(i);
        for (int i = 0; i < n; i++) Assert.Equal(i, list.RemoveFirst());
        Assert.Equal(0, list.Count);
        Assert.Empty(list);
    }

    [Fact]
    public void RemoveLast_LargeInput_DrainsInReverseOrder()
    {
        const int n = 1_000;
        var list = new CircularDoublyLinkedList<int>();
        for (int i = 0; i < n; i++) list.AddLast(i);
        for (int i = n - 1; i >= 0; i--) Assert.Equal(i, list.RemoveLast());
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void Contains_LargeInput_FindsAllElements()
    {
        const int n = 1_000;
        var list = new CircularDoublyLinkedList<int>();
        for (int i = 0; i < n; i++) list.AddLast(i);
        for (int i = 0; i < n; i++) Assert.True(list.Contains(i));
        Assert.False(list.Contains(n));
        Assert.False(list.Contains(-1));
    }

    [Fact]
    public void Rotate_LargeInput_EquivalentToLinqSkipAndConcat()
    {
        const int n = 1_000;
        const int steps = 37;
        var list = new CircularDoublyLinkedList<int>();
        for (int i = 0; i < n; i++) list.AddLast(i);

        list.Rotate(steps);

        var expected = Enumerable.Range(steps, n - steps).Concat(Enumerable.Range(0, steps));
        Assert.Equal(expected, list);
    }

    [Fact]
    public void Rotate_LargeInputNegativeSteps_EquivalentToRightRotation()
    {
        const int n = 1_000;
        const int rightSteps = 37;
        var list = new CircularDoublyLinkedList<int>();
        for (int i = 0; i < n; i++) list.AddLast(i);

        list.Rotate(-rightSteps);

        // rotating right by k == rotating left by (n - k)
        int leftEquiv = n - rightSteps;
        var expected = Enumerable.Range(leftEquiv, n - leftEquiv).Concat(Enumerable.Range(0, leftEquiv));
        Assert.Equal(expected, list);
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static CircularDoublyLinkedList<int> ListOf(params int[] values)
    {
        var list = new CircularDoublyLinkedList<int>();
        foreach (var v in values) list.AddLast(v);
        return list;
    }
}
