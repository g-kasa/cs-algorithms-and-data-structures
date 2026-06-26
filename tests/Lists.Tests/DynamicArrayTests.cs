namespace Algorithms.Lists.Tests;

public class DynamicArrayTests
{
    // ── Count / Capacity (initial state) ────────────────────────────────────

    [Fact]
    public void Count_NewArray_IsZero()
    {
        var arr = new SortableDynamicArray<int>();
        Assert.Equal(0, arr.Count);
    }

    [Fact]
    public void Capacity_NewArray_IsFour()
    {
        var arr = new SortableDynamicArray<int>();
        Assert.Equal(4, arr.Capacity);
    }

    // ── Add ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Add_SingleElement_CountIsOneAndElementAccessible()
    {
        var arr = new SortableDynamicArray<int>();
        arr.Add(42);
        Assert.Equal(1, arr.Count);
        Assert.Equal(42, arr[0]);
    }

    [Fact]
    public void Add_TwoElements_BothAccessibleInOrder()
    {
        var arr = new SortableDynamicArray<int>();
        arr.Add(10);
        arr.Add(20);
        Assert.Equal(2, arr.Count);
        Assert.Equal(10, arr[0]);
        Assert.Equal(20, arr[1]);
    }

    [Fact]
    public void Add_FillsInitialCapacity_CapacityDoubles()
    {
        var arr = new SortableDynamicArray<int>();
        // Initial capacity is 4; adding a 5th element must trigger a resize.
        arr.Add(1);
        arr.Add(2);
        arr.Add(3);
        arr.Add(4);
        Assert.Equal(4, arr.Capacity); // still 4 at this point

        arr.Add(5);
        Assert.Equal(8, arr.Capacity); // doubled to 8
        Assert.Equal(5, arr.Count);
    }

    [Fact]
    public void Add_RepeatedDoublings_CapacityGrowsCorrectly()
    {
        var arr = new SortableDynamicArray<int>();
        // After 8 adds capacity should be 8; after 9th it becomes 16, etc.
        for (int i = 0; i < 8; i++) arr.Add(i);
        Assert.Equal(8, arr.Capacity);

        arr.Add(8);
        Assert.Equal(16, arr.Capacity);
        Assert.Equal(9, arr.Count);
    }

    [Fact]
    public void Add_AllIdenticalValues_CountCorrectAndAllElementsEqual()
    {
        var arr = ArrayOf(7, 7, 7, 7, 7);
        Assert.Equal(5, arr.Count);
        Assert.All(arr, v => Assert.Equal(7, v));
    }

    [Fact]
    public void Add_LargeInput_AllElementsStoredCorrectly()
    {
        const int n = 2_000;
        var arr = new SortableDynamicArray<int>();
        for (int i = 0; i < n; i++) arr.Add(i);

        Assert.Equal(n, arr.Count);
        for (int i = 0; i < n; i++)
            Assert.Equal(i, arr[i]);
    }

    // ── Indexer (get / set) ──────────────────────────────────────────────────

    [Fact]
    public void Indexer_Get_ReturnsCorrectElement()
    {
        var arr = ArrayOf(10, 20, 30);
        Assert.Equal(10, arr[0]);
        Assert.Equal(20, arr[1]);
        Assert.Equal(30, arr[2]);
    }

    [Fact]
    public void Indexer_Set_UpdatesElement()
    {
        var arr = ArrayOf(1, 2, 3);
        arr[1] = 99;
        Assert.Equal(99, arr[1]);
        Assert.Equal(3, arr.Count); // count unchanged
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    [InlineData(100)]
    public void Indexer_Get_OutOfRange_ThrowsArgumentOutOfRangeException(int index)
    {
        var arr = ArrayOf(1, 2, 3);
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = arr[index]);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    [InlineData(100)]
    public void Indexer_Set_OutOfRange_ThrowsArgumentOutOfRangeException(int index)
    {
        var arr = ArrayOf(1, 2, 3);
        Assert.Throws<ArgumentOutOfRangeException>(() => arr[index] = 0);
    }

    [Fact]
    public void Indexer_Get_OnEmptyArray_ThrowsArgumentOutOfRangeException()
    {
        var arr = new SortableDynamicArray<int>();
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = arr[0]);
    }

    [Fact]
    public void Indexer_BoundaryValues_IntMinAndMax()
    {
        var arr = ArrayOf(int.MinValue, int.MaxValue);
        Assert.Equal(int.MinValue, arr[0]);
        Assert.Equal(int.MaxValue, arr[1]);
    }

    // ── Insert ───────────────────────────────────────────────────────────────

    [Fact]
    public void Insert_AtIndexZero_PrependsSingleElement()
    {
        var arr = ArrayOf(2, 3, 4);
        arr.Insert(0, 1);
        Assert.Equal(4, arr.Count);
        Assert.Equal(new[] { 1, 2, 3, 4 }, arr);
    }

    [Fact]
    public void Insert_AtMiddleIndex_ShiftsSubsequentElements()
    {
        var arr = ArrayOf(1, 3, 4);
        arr.Insert(1, 2);
        Assert.Equal(new[] { 1, 2, 3, 4 }, arr);
    }

    [Fact]
    public void Insert_AtCount_EquivalentToAdd()
    {
        var arr = ArrayOf(1, 2, 3);
        arr.Insert(arr.Count, 4);
        Assert.Equal(new[] { 1, 2, 3, 4 }, arr);
    }

    [Fact]
    public void Insert_IntoEmptyArray_AtIndexZero_CountIsOne()
    {
        var arr = new SortableDynamicArray<int>();
        arr.Insert(0, 42);
        Assert.Equal(1, arr.Count);
        Assert.Equal(42, arr[0]);
    }

    [Fact]
    public void Insert_IntoSingleElementArray_AtIndexZero_PrependCorrectly()
    {
        var arr = ArrayOf(99);
        arr.Insert(0, 1);
        Assert.Equal(new[] { 1, 99 }, arr);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    [InlineData(100)]
    public void Insert_OutOfRange_ThrowsArgumentOutOfRangeException(int index)
    {
        var arr = ArrayOf(1, 2, 3);
        Assert.Throws<ArgumentOutOfRangeException>(() => arr.Insert(index, 0));
    }

    [Fact]
    public void Insert_TriggersCapacityGrowth_AllElementsPreserved()
    {
        // Exactly fill the initial capacity then insert at front to force resize.
        var arr = ArrayOf(1, 2, 3, 4);
        Assert.Equal(4, arr.Capacity);
        arr.Insert(0, 0);
        Assert.Equal(8, arr.Capacity);
        Assert.Equal(new[] { 0, 1, 2, 3, 4 }, arr);
    }

    // ── RemoveAt ─────────────────────────────────────────────────────────────

    [Fact]
    public void RemoveAt_FirstElement_ShiftsRemainingLeft()
    {
        var arr = ArrayOf(1, 2, 3);
        arr.RemoveAt(0);
        Assert.Equal(new[] { 2, 3 }, arr);
        Assert.Equal(2, arr.Count);
    }

    [Fact]
    public void RemoveAt_MiddleElement_BridgesNeighbors()
    {
        var arr = ArrayOf(1, 2, 3);
        arr.RemoveAt(1);
        Assert.Equal(new[] { 1, 3 }, arr);
    }

    [Fact]
    public void RemoveAt_LastElement_TruncatesArray()
    {
        var arr = ArrayOf(1, 2, 3);
        arr.RemoveAt(2);
        Assert.Equal(new[] { 1, 2 }, arr);
    }

    [Fact]
    public void RemoveAt_OnlyElement_CountBecomesZero()
    {
        var arr = ArrayOf(42);
        arr.RemoveAt(0);
        Assert.Equal(0, arr.Count);
        Assert.Empty(arr);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    [InlineData(100)]
    public void RemoveAt_OutOfRange_ThrowsArgumentOutOfRangeException(int index)
    {
        var arr = ArrayOf(1, 2, 3);
        Assert.Throws<ArgumentOutOfRangeException>(() => arr.RemoveAt(index));
    }

    [Fact]
    public void RemoveAt_EmptyArray_ThrowsArgumentOutOfRangeException()
    {
        var arr = new SortableDynamicArray<int>();
        Assert.Throws<ArgumentOutOfRangeException>(() => arr.RemoveAt(0));
    }

    // ── Remove ───────────────────────────────────────────────────────────────

    [Fact]
    public void Remove_ExistingValue_ReturnsTrueAndDecreasesCount()
    {
        var arr = ArrayOf(1, 2, 3);
        bool removed = arr.Remove(2);
        Assert.True(removed);
        Assert.Equal(2, arr.Count);
        Assert.Equal(new[] { 1, 3 }, arr);
    }

    [Fact]
    public void Remove_FirstOccurrence_WhenDuplicatesExist_RemovesOnlyFirst()
    {
        var arr = ArrayOf(5, 5, 5);
        bool removed = arr.Remove(5);
        Assert.True(removed);
        Assert.Equal(2, arr.Count);
        Assert.Equal(new[] { 5, 5 }, arr);
    }

    [Fact]
    public void Remove_MissingValue_ReturnsFalseAndCountUnchanged()
    {
        var arr = ArrayOf(1, 2, 3);
        bool removed = arr.Remove(99);
        Assert.False(removed);
        Assert.Equal(3, arr.Count);
    }

    [Fact]
    public void Remove_OnEmptyArray_ReturnsFalse()
    {
        var arr = new SortableDynamicArray<int>();
        Assert.False(arr.Remove(1));
    }

    [Fact]
    public void Remove_OnlyElement_ArrayBecomesEmpty()
    {
        var arr = ArrayOf(7);
        Assert.True(arr.Remove(7));
        Assert.Equal(0, arr.Count);
        Assert.Empty(arr);
    }

    [Fact]
    public void Remove_BoundaryValue_IntMinValue_RemovesCorrectly()
    {
        var arr = ArrayOf(int.MinValue, 0, int.MaxValue);
        Assert.True(arr.Remove(int.MinValue));
        Assert.Equal(new[] { 0, int.MaxValue }, arr);
    }

    [Fact]
    public void Remove_BoundaryValue_IntMaxValue_RemovesCorrectly()
    {
        var arr = ArrayOf(int.MinValue, 0, int.MaxValue);
        Assert.True(arr.Remove(int.MaxValue));
        Assert.Equal(new[] { int.MinValue, 0 }, arr);
    }

    // ── Contains ─────────────────────────────────────────────────────────────

    [Fact]
    public void Contains_EmptyArray_ReturnsFalse()
    {
        Assert.False(new DynamicArray<int>().Contains(1));
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3 }, 1, true)]
    [InlineData(new[] { 1, 2, 3 }, 2, true)]
    [InlineData(new[] { 1, 2, 3 }, 3, true)]
    [InlineData(new[] { 1, 2, 3 }, 99, false)]
    [InlineData(new[] { 7, 7, 7 }, 7, true)]
    [InlineData(new[] { 7, 7, 7 }, 0, false)]
    public void Contains_VariousInputs_ReturnsExpected(int[] values, int target, bool expected)
    {
        Assert.Equal(expected, ArrayOf(values).Contains(target));
    }

    // ── IndexOf ──────────────────────────────────────────────────────────────

    [Fact]
    public void IndexOf_EmptyArray_ReturnsNegativeOne()
    {
        Assert.Equal(-1, new DynamicArray<int>().IndexOf(1));
    }

    [Theory]
    [InlineData(new[] { 10, 20, 30 }, 10, 0)]   // first element
    [InlineData(new[] { 10, 20, 30 }, 20, 1)]   // middle element
    [InlineData(new[] { 10, 20, 30 }, 30, 2)]   // last element
    [InlineData(new[] { 10, 20, 30 }, 99, -1)]  // not present
    [InlineData(new[] { 5, 5, 5 }, 5, 0)]       // returns first occurrence
    public void IndexOf_VariousInputs_ReturnsExpectedIndex(int[] values, int target, int expected)
    {
        Assert.Equal(expected, ArrayOf(values).IndexOf(target));
    }

    // ── BinarySearch ─────────────────────────────────────────────────────────

    [Fact]
    public void BinarySearch_EmptyArray_ReturnsNegativeOne()
    {
        Assert.Equal(-1, new SortableDynamicArray<int>().BinarySearch(5));
    }

    [Fact]
    public void BinarySearch_SingleElement_Found_ReturnsZero()
    {
        var arr = ArrayOf(42);
        Assert.Equal(0, arr.BinarySearch(42));
    }

    [Fact]
    public void BinarySearch_SingleElement_NotFound_ReturnsNegativeOne()
    {
        var arr = ArrayOf(42);
        Assert.Equal(-1, arr.BinarySearch(99));
    }

    [Theory]
    [MemberData(nameof(BinarySearchFoundCases))]
    public void BinarySearch_ValuePresent_ReturnsCorrectIndex(int[] sortedValues, int target, int expectedIndex)
    {
        Assert.Equal(expectedIndex, ArrayOf(sortedValues).BinarySearch(target));
    }

    public static IEnumerable<object[]> BinarySearchFoundCases =>
    [
        [new[] { 1, 3, 5, 7, 9 }, 1, 0],   // first element
        [new[] { 1, 3, 5, 7, 9 }, 9, 4],   // last element
        [new[] { 1, 3, 5, 7, 9 }, 5, 2],   // middle element
        [new[] { 1, 3, 5, 7, 9 }, 3, 1],   // second element
        [new[] { 1, 3, 5, 7, 9 }, 7, 3],   // fourth element
        [new[] { 2, 4 },          2, 0],   // two-element, first
        [new[] { 2, 4 },          4, 1],   // two-element, last
    ];

    [Theory]
    [InlineData(new[] { 1, 3, 5, 7, 9 }, 0)]   // below range
    [InlineData(new[] { 1, 3, 5, 7, 9 }, 2)]   // between elements
    [InlineData(new[] { 1, 3, 5, 7, 9 }, 10)]  // above range
    public void BinarySearch_ValueAbsent_ReturnsNegativeOne(int[] sortedValues, int target)
    {
        Assert.Equal(-1, ArrayOf(sortedValues).BinarySearch(target));
    }

    [Fact]
    public void BinarySearch_AllIdenticalElements_Found_ReturnsAnIndex()
    {
        var arr = ArrayOf(6, 6, 6, 6, 6);
        int idx = arr.BinarySearch(6);
        Assert.InRange(idx, 0, arr.Count - 1);
    }

    [Fact]
    public void BinarySearch_BoundaryValues_IntMinAndMax_Found()
    {
        var arr = ArrayOf(int.MinValue, 0, int.MaxValue);
        Assert.Equal(0, arr.BinarySearch(int.MinValue));
        Assert.Equal(2, arr.BinarySearch(int.MaxValue));
    }

    // ── Sort ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Sort_EmptyArray_NoException()
    {
        var arr = new SortableDynamicArray<int>();
        arr.Sort(); // must not throw
        Assert.Equal(0, arr.Count);
    }

    [Fact]
    public void Sort_SingleElement_ArrayUnchanged()
    {
        var arr = ArrayOf(5);
        arr.Sort();
        Assert.Equal(new[] { 5 }, arr);
    }

    [Fact]
    public void Sort_TwoElements_Unsorted_SortsCorrectly()
    {
        var arr = ArrayOf(2, 1);
        arr.Sort();
        Assert.Equal(new[] { 1, 2 }, arr);
    }

    [Theory]
    [MemberData(nameof(SortCases))]
    public void Sort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var arr = ArrayOf(input);
        arr.Sort();
        Assert.Equal(expected, arr);
    }

    public static IEnumerable<object[]> SortCases =>
    [
        [new[] { 3, 1, 4, 1, 5, 9, 2, 6 }, new[] { 1, 1, 2, 3, 4, 5, 6, 9 }], // random with duplicates
        [new[] { 5, 4, 3, 2, 1 },           new[] { 1, 2, 3, 4, 5 }],           // reverse-sorted
        [new[] { 1, 2, 3, 4, 5 },           new[] { 1, 2, 3, 4, 5 }],           // pre-sorted
        [new[] { 7, 7, 7, 7 },              new[] { 7, 7, 7, 7 }],              // all identical
        [new[] { -3, 0, -1, 5, 2 },         new[] { -3, -1, 0, 2, 5 }],         // negative values
        [new[] { int.MinValue, 0, int.MaxValue, -1, 1 }, new[] { int.MinValue, -1, 0, 1, int.MaxValue }], // boundary values
    ];

    [Fact]
    public void Sort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(42);
        int[] input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(10_000)).ToArray();
        var arr = ArrayOf(input);

        arr.Sort();

        Assert.Equal(input.OrderBy(x => x), arr);
    }

    [Fact]
    public void Sort_ThenBinarySearch_FindsAllElements()
    {
        var arr = ArrayOf(9, 3, 7, 1, 5);
        arr.Sort();
        // After sort: [1, 3, 5, 7, 9]
        Assert.Equal(0, arr.BinarySearch(1));
        Assert.Equal(1, arr.BinarySearch(3));
        Assert.Equal(2, arr.BinarySearch(5));
        Assert.Equal(3, arr.BinarySearch(7));
        Assert.Equal(4, arr.BinarySearch(9));
    }

    [Fact]
    public void Sort_ThenBinarySearch_AbsentValue_ReturnsNegativeOne()
    {
        var arr = ArrayOf(5, 1, 4, 2, 3);
        arr.Sort();
        Assert.Equal(-1, arr.BinarySearch(99));
    }

    // ── Rotate ───────────────────────────────────────────────────────────────

    [Fact]
    public void Rotate_EmptyArray_NoException()
    {
        var arr = new SortableDynamicArray<int>();
        arr.Rotate(3); // must not throw
        Assert.Equal(0, arr.Count);
    }

    [Fact]
    public void Rotate_SingleElement_ArrayUnchanged()
    {
        var arr = ArrayOf(42);
        arr.Rotate(1);
        Assert.Equal(new[] { 42 }, arr);
    }

    [Fact]
    public void Rotate_ZeroSteps_ArrayUnchanged()
    {
        var arr = ArrayOf(1, 2, 3, 4, 5);
        arr.Rotate(0);
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, arr);
    }

    [Fact]
    public void Rotate_ByCount_ArrayUnchanged()
    {
        var arr = ArrayOf(1, 2, 3, 4, 5);
        arr.Rotate(arr.Count);
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, arr);
    }

    [Theory]
    [MemberData(nameof(RotateLeftCases))]
    public void Rotate_PositiveSteps_RotatesLeft(int[] input, int steps, int[] expected)
    {
        var arr = ArrayOf(input);
        arr.Rotate(steps);
        Assert.Equal(expected, arr);
    }

    public static IEnumerable<object[]> RotateLeftCases =>
    [
        [new[] { 1, 2, 3, 4, 5 }, 1, new[] { 2, 3, 4, 5, 1 }],  // left by 1
        [new[] { 1, 2, 3, 4, 5 }, 2, new[] { 3, 4, 5, 1, 2 }],  // left by 2
        [new[] { 1, 2, 3, 4, 5 }, 4, new[] { 5, 1, 2, 3, 4 }],  // left by 4
        [new[] { 10, 20 },        1, new[] { 20, 10 }],          // two elements
    ];

    [Theory]
    [MemberData(nameof(RotateRightCases))]
    public void Rotate_NegativeSteps_RotatesRight(int[] input, int steps, int[] expected)
    {
        var arr = ArrayOf(input);
        arr.Rotate(steps);
        Assert.Equal(expected, arr);
    }

    public static IEnumerable<object[]> RotateRightCases =>
    [
        [new[] { 1, 2, 3, 4, 5 }, -1, new[] { 5, 1, 2, 3, 4 }], // right by 1
        [new[] { 1, 2, 3, 4, 5 }, -2, new[] { 4, 5, 1, 2, 3 }], // right by 2
        [new[] { 1, 2, 3, 4, 5 }, -4, new[] { 2, 3, 4, 5, 1 }], // right by 4
        [new[] { 10, 20 },        -1, new[] { 20, 10 }],         // two elements, right
    ];

    [Fact]
    public void Rotate_StepsGreaterThanCount_WrapsCorrectly()
    {
        var arr = ArrayOf(1, 2, 3, 4, 5);
        arr.Rotate(7); // 7 % 5 = 2  →  same as left rotate by 2
        Assert.Equal(new[] { 3, 4, 5, 1, 2 }, arr);
    }

    [Fact]
    public void Rotate_NegativeStepsGreaterThanCount_WrapsCorrectly()
    {
        var arr = ArrayOf(1, 2, 3, 4, 5);
        arr.Rotate(-7); // -7 % 5 normalised = 3  →  same as left rotate by 3
        Assert.Equal(new[] { 4, 5, 1, 2, 3 }, arr);
    }

    [Fact]
    public void Rotate_TwiceByHalfCount_RestoresOriginalOrder()
    {
        var arr = ArrayOf(1, 2, 3, 4);
        arr.Rotate(2);
        arr.Rotate(2);
        Assert.Equal(new[] { 1, 2, 3, 4 }, arr);
    }

    [Fact]
    public void Rotate_AllIdenticalElements_ArrayUnchanged()
    {
        var arr = ArrayOf(9, 9, 9, 9);
        arr.Rotate(3);
        Assert.Equal(new[] { 9, 9, 9, 9 }, arr);
    }

    // ── GetEnumerator ────────────────────────────────────────────────────────

    [Fact]
    public void GetEnumerator_EmptyArray_YieldsNoElements()
    {
        Assert.Empty(new DynamicArray<int>());
    }

    [Fact]
    public void GetEnumerator_NonEmptyArray_YieldsElementsInOrder()
    {
        var arr = ArrayOf(3, 1, 4, 1, 5);
        Assert.Equal(new[] { 3, 1, 4, 1, 5 }, arr);
    }

    [Fact]
    public void GetEnumerator_OnlyLiveElementsYielded_NotBackingArraySlack()
    {
        // Capacity is 4 initially; we add 2 elements — only those 2 should be enumerated.
        var arr = ArrayOf(10, 20);
        Assert.Equal(4, arr.Capacity);
        Assert.Equal(2, arr.Count);
        Assert.Equal(new[] { 10, 20 }, arr);
    }

    [Fact]
    public void GetEnumerator_AfterRemoveAt_YieldsUpdatedElements()
    {
        var arr = ArrayOf(1, 2, 3, 4);
        arr.RemoveAt(1);
        Assert.Equal(new[] { 1, 3, 4 }, arr);
    }

    // ── Integration: Add → Sort → BinarySearch → Remove ─────────────────────

    [Fact]
    public void Integration_AddSortBinarySearchRemove_WorksEndToEnd()
    {
        var arr = new SortableDynamicArray<int>();
        int[] values = [5, 3, 8, 1, 9, 2, 7, 4, 6, 0];
        foreach (var v in values) arr.Add(v);

        arr.Sort();
        // After sort: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]

        Assert.Equal(0, arr.BinarySearch(0));
        Assert.Equal(5, arr.BinarySearch(5));
        Assert.Equal(9, arr.BinarySearch(9));
        Assert.Equal(-1, arr.BinarySearch(99));

        arr.Remove(5);
        Assert.Equal(9, arr.Count);
        Assert.False(arr.Contains(5));
        // BinarySearch result for 5 is now undefined but it should be -1 since 5 is gone.
        Assert.Equal(-1, arr.BinarySearch(5));
    }

    [Fact]
    public void Integration_LargeInput_SortAndBinarySearchAllElements()
    {
        const int n = 1_000;
        var rng = new Random(7);
        int[] input = Enumerable.Range(0, n).Select(_ => rng.Next(10_000)).Distinct().Take(500).ToArray();

        var arr = ArrayOf(input);
        arr.Sort();

        int[] sorted = input.OrderBy(x => x).ToArray();
        for (int i = 0; i < sorted.Length; i++)
        {
            int idx = arr.BinarySearch(sorted[i]);
            Assert.True(idx >= 0, $"Expected to find {sorted[i]} via BinarySearch but got -1.");
            Assert.Equal(sorted[i], arr[idx]);
        }
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static SortableDynamicArray<int> ArrayOf(params int[] values)
    {
        var arr = new SortableDynamicArray<int>();
        foreach (var v in values) arr.Add(v);
        return arr;
    }
}
