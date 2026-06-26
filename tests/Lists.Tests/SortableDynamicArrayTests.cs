namespace Algorithms.Lists.Tests;

/// <summary>
/// Comprehensive tests for <see cref="SortableDynamicArray{T}"/>:
/// BubbleSort, SelectionSort, InsertionSort, ShellSort, MergeSort, QuickSort, HeapSort,
/// the <see cref="SortableDynamicArray{T}.Sort"/> facade, and
/// <see cref="SortableDynamicArray{T}.BinarySearch"/>.
/// </summary>
public class SortableDynamicArrayTests
{
    // ── Shared sort input / expected-output pairs ────────────────────────────
    //
    // Every sort method's [Theory] consumes this same data source so that
    // coverage is consistent across all seven algorithms.

    public static IEnumerable<object[]> SortCases =>
    [
        // empty
        [Array.Empty<int>(), Array.Empty<int>()],
        // single element
        [new[] { 42 }, new[] { 42 }],
        // two elements — already sorted
        [new[] { 1, 2 }, new[] { 1, 2 }],
        // two elements — reverse-sorted
        [new[] { 9, 3 }, new[] { 3, 9 }],
        // all identical
        [new[] { 5, 5, 5, 5, 5 }, new[] { 5, 5, 5, 5, 5 }],
        // pre-sorted (best-case for adaptive algorithms)
        [new[] { 1, 2, 3, 4, 5, 6, 7 }, new[] { 1, 2, 3, 4, 5, 6, 7 }],
        // reverse-sorted (worst-case for naive algorithms)
        [new[] { 7, 6, 5, 4, 3, 2, 1 }, new[] { 1, 2, 3, 4, 5, 6, 7 }],
        // general random with duplicates
        [new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3 }, new[] { 1, 1, 2, 3, 3, 4, 5, 5, 6, 9 }],
        // general random, larger
        [new[] { 64, 25, 12, 22, 11, 90, 7, 33, 45, 2, 78, 56 }, new[] { 2, 7, 11, 12, 22, 25, 33, 45, 56, 64, 78, 90 }],
        // another random set — odd length, no duplicates
        [new[] { 17, 3, 41, 8, 55, 29, 6 }, new[] { 3, 6, 8, 17, 29, 41, 55 }],
        // negative values
        [new[] { -5, 0, -3, 4, -1, 2 }, new[] { -5, -3, -1, 0, 2, 4 }],
        // boundary values: int.MinValue and int.MaxValue mixed with normal values
        [new[] { int.MaxValue, 0, int.MinValue, -1, 1 }, new[] { int.MinValue, -1, 0, 1, int.MaxValue }],
    ];

    // ── BubbleSort ───────────────────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(SortCases))]
    public void BubbleSort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var arr = ArrayOf(input);
        arr.BubbleSort();
        Assert.Equal(expected, arr);
    }

    [Fact]
    public void BubbleSort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(101);
        int[] input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(5_000)).ToArray();
        var arr = ArrayOf(input);

        arr.BubbleSort();

        Assert.Equal(input.OrderBy(x => x), arr);
    }

    // ── SelectionSort ────────────────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(SortCases))]
    public void SelectionSort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var arr = ArrayOf(input);
        arr.SelectionSort();
        Assert.Equal(expected, arr);
    }

    [Fact]
    public void SelectionSort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(202);
        int[] input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(5_000)).ToArray();
        var arr = ArrayOf(input);

        arr.SelectionSort();

        Assert.Equal(input.OrderBy(x => x), arr);
    }

    // ── InsertionSort ────────────────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(SortCases))]
    public void InsertionSort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var arr = ArrayOf(input);
        arr.InsertionSort();
        Assert.Equal(expected, arr);
    }

    [Fact]
    public void InsertionSort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(303);
        int[] input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(5_000)).ToArray();
        var arr = ArrayOf(input);

        arr.InsertionSort();

        Assert.Equal(input.OrderBy(x => x), arr);
    }

    // ── ShellSort ────────────────────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(SortCases))]
    public void ShellSort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var arr = ArrayOf(input);
        arr.ShellSort();
        Assert.Equal(expected, arr);
    }

    [Fact]
    public void ShellSort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(404);
        int[] input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(5_000)).ToArray();
        var arr = ArrayOf(input);

        arr.ShellSort();

        Assert.Equal(input.OrderBy(x => x), arr);
    }

    // ── MergeSort ────────────────────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(SortCases))]
    public void MergeSort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var arr = ArrayOf(input);
        arr.MergeSort();
        Assert.Equal(expected, arr);
    }

    [Fact]
    public void MergeSort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(505);
        int[] input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(5_000)).ToArray();
        var arr = ArrayOf(input);

        arr.MergeSort();

        Assert.Equal(input.OrderBy(x => x), arr);
    }

    [Fact]
    public void MergeSort_IsStable_EqualElementsRetainRelativeOrder()
    {
        // Use a two-element record so we can distinguish "equal by key but different by id".
        // We sort only by Key; original insertion order for equal keys must be preserved.
        var arr = new SortableDynamicArray<StableItem>();
        arr.Add(new StableItem(2, 'A'));
        arr.Add(new StableItem(1, 'B'));
        arr.Add(new StableItem(2, 'C'));
        arr.Add(new StableItem(1, 'D'));
        arr.Add(new StableItem(2, 'E'));

        arr.MergeSort();

        // Expected key order: 1, 1, 2, 2, 2
        // Within each key group the insertion-order label must be preserved.
        Assert.Equal([1, 1, 2, 2, 2], arr.Select(x => x.Key));
        Assert.Equal(['B', 'D', 'A', 'C', 'E'], arr.Select(x => x.Label));
    }

    // ── QuickSort ────────────────────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(SortCases))]
    public void QuickSort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var arr = ArrayOf(input);
        arr.QuickSort();
        Assert.Equal(expected, arr);
    }

    [Fact]
    public void QuickSort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(606);
        int[] input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(5_000)).ToArray();
        var arr = ArrayOf(input);

        arr.QuickSort();

        Assert.Equal(input.OrderBy(x => x), arr);
    }

    // ── HeapSort ─────────────────────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(SortCases))]
    public void HeapSort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var arr = ArrayOf(input);
        arr.HeapSort();
        Assert.Equal(expected, arr);
    }

    [Fact]
    public void HeapSort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(707);
        int[] input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(5_000)).ToArray();
        var arr = ArrayOf(input);

        arr.HeapSort();

        Assert.Equal(input.OrderBy(x => x), arr);
    }

    // ── Sort (facade → MergeSort) ────────────────────────────────────────────

    [Theory]
    [MemberData(nameof(SortCases))]
    public void Sort_GivenInput_ProducesAscendingOrder(int[] input, int[] expected)
    {
        var arr = ArrayOf(input);
        arr.Sort();
        Assert.Equal(expected, arr);
    }

    [Fact]
    public void Sort_LargeRandomInput_MatchesLinqOrderBy()
    {
        var rng = new Random(808);
        int[] input = Enumerable.Range(0, 1_000).Select(_ => rng.Next(5_000)).ToArray();
        var arr = ArrayOf(input);

        arr.Sort();

        Assert.Equal(input.OrderBy(x => x), arr);
    }

    // ── In-place mutation verification ───────────────────────────────────────
    //
    // All sorts must mutate the array in-place; Count must not change.

    [Theory]
    [InlineData("Bubble")]
    [InlineData("Selection")]
    [InlineData("Insertion")]
    [InlineData("Shell")]
    [InlineData("Merge")]
    [InlineData("Quick")]
    [InlineData("Heap")]
    public void AllSorts_CountUnchangedAfterSort(string algorithm)
    {
        int[] input = [5, 3, 8, 1, 9, 2, 7, 4, 6, 0];
        var arr = ArrayOf(input);
        int countBefore = arr.Count;

        InvokeSort(arr, algorithm);

        Assert.Equal(countBefore, arr.Count);
    }

    [Theory]
    [InlineData("Bubble")]
    [InlineData("Selection")]
    [InlineData("Insertion")]
    [InlineData("Shell")]
    [InlineData("Merge")]
    [InlineData("Quick")]
    [InlineData("Heap")]
    public void AllSorts_ContainsSameElementsAfterSort(string algorithm)
    {
        int[] input = [5, 3, 8, 1, 9, 2, 7, 4, 6, 0];
        var arr = ArrayOf(input);

        InvokeSort(arr, algorithm);

        // Every original value must still be present.
        Assert.All(input, v => Assert.Contains(v, arr));
    }

    // ── BinarySearch ─────────────────────────────────────────────────────────

    [Fact]
    public void BinarySearch_EmptyArray_ReturnsNegativeOne()
    {
        var arr = new SortableDynamicArray<int>();
        Assert.Equal(-1, arr.BinarySearch(42));
    }

    [Fact]
    public void BinarySearch_SingleElement_Found_ReturnsZero()
    {
        var arr = ArrayOf(7);
        Assert.Equal(0, arr.BinarySearch(7));
    }

    [Fact]
    public void BinarySearch_SingleElement_NotFound_ReturnsNegativeOne()
    {
        var arr = ArrayOf(7);
        Assert.Equal(-1, arr.BinarySearch(99));
    }

    [Theory]
    [MemberData(nameof(BinarySearchFoundCases))]
    public void BinarySearch_ValuePresent_ReturnsCorrectIndex(int[] sortedValues, int target, int expectedIndex)
    {
        var arr = ArrayOf(sortedValues);
        Assert.Equal(expectedIndex, arr.BinarySearch(target));
    }

    public static IEnumerable<object[]> BinarySearchFoundCases =>
    [
        // first element
        [new[] { 1, 3, 5, 7, 9 }, 1, 0],
        // last element
        [new[] { 1, 3, 5, 7, 9 }, 9, 4],
        // middle element (odd-length array)
        [new[] { 1, 3, 5, 7, 9 }, 5, 2],
        // second element
        [new[] { 1, 3, 5, 7, 9 }, 3, 1],
        // fourth element
        [new[] { 1, 3, 5, 7, 9 }, 7, 3],
        // two-element array — first
        [new[] { 4, 8 }, 4, 0],
        // two-element array — second
        [new[] { 4, 8 }, 8, 1],
        // int.MinValue boundary
        [new[] { int.MinValue, 0, int.MaxValue }, int.MinValue, 0],
        // int.MaxValue boundary
        [new[] { int.MinValue, 0, int.MaxValue }, int.MaxValue, 2],
        // zero in a mixed-sign array
        [new[] { int.MinValue, 0, int.MaxValue }, 0, 1],
    ];

    [Theory]
    [InlineData(new[] { 1, 3, 5, 7, 9 }, 0)]    // below range
    [InlineData(new[] { 1, 3, 5, 7, 9 }, 10)]   // above range
    [InlineData(new[] { 1, 3, 5, 7, 9 }, 2)]    // between elements
    [InlineData(new[] { 1, 3, 5, 7, 9 }, 4)]    // between elements
    [InlineData(new[] { 1, 3, 5, 7, 9 }, 6)]    // between elements
    [InlineData(new[] { 1, 3, 5, 7, 9 }, 8)]    // between elements
    public void BinarySearch_ValueAbsent_ReturnsNegativeOne(int[] sortedValues, int target)
    {
        var arr = ArrayOf(sortedValues);
        Assert.Equal(-1, arr.BinarySearch(target));
    }

    [Fact]
    public void BinarySearch_TwoElementArray_BelowRange_ReturnsNegativeOne()
    {
        var arr = ArrayOf(5, 10);
        Assert.Equal(-1, arr.BinarySearch(3));
    }

    [Fact]
    public void BinarySearch_TwoElementArray_AboveRange_ReturnsNegativeOne()
    {
        var arr = ArrayOf(5, 10);
        Assert.Equal(-1, arr.BinarySearch(12));
    }

    [Fact]
    public void BinarySearch_AllIdenticalElements_Found_ReturnsAnIndex()
    {
        var arr = ArrayOf(6, 6, 6, 6, 6);
        int idx = arr.BinarySearch(6);
        Assert.InRange(idx, 0, arr.Count - 1);
        Assert.Equal(6, arr[idx]);
    }

    [Fact]
    public void BinarySearch_AllIdenticalElements_AbsentValue_ReturnsNegativeOne()
    {
        var arr = ArrayOf(6, 6, 6, 6, 6);
        Assert.Equal(-1, arr.BinarySearch(7));
    }

    [Fact]
    public void BinarySearch_AfterSort_FindsEveryElement()
    {
        int[] unsorted = [9, 3, 7, 1, 5, 0, 8, 4, 6, 2];
        var arr = ArrayOf(unsorted);
        arr.Sort();

        int[] sorted = unsorted.OrderBy(x => x).ToArray();
        for (int i = 0; i < sorted.Length; i++)
        {
            int idx = arr.BinarySearch(sorted[i]);
            Assert.Equal(i, idx);
        }
    }

    [Fact]
    public void BinarySearch_AfterSort_AbsentValue_ReturnsNegativeOne()
    {
        var arr = ArrayOf(5, 1, 4, 2, 3);
        arr.Sort();
        Assert.Equal(-1, arr.BinarySearch(99));
        Assert.Equal(-1, arr.BinarySearch(-1));
    }

    [Fact]
    public void BinarySearch_LargeSortedInput_FindsAllElements()
    {
        const int n = 2_000;
        // Use distinct even numbers so every lookup has a definite index.
        int[] values = Enumerable.Range(0, n).Select(i => i * 2).ToArray();
        var arr = ArrayOf(values);
        // Array is already sorted (ascending evens).

        for (int i = 0; i < n; i++)
            Assert.Equal(i, arr.BinarySearch(values[i]));
    }

    [Fact]
    public void BinarySearch_LargeSortedInput_AbsentOddValues_ReturnNegativeOne()
    {
        const int n = 500;
        int[] values = Enumerable.Range(0, n).Select(i => i * 2).ToArray(); // 0,2,4,...
        var arr = ArrayOf(values);

        // Odd numbers between 1 and 2n-1 are never present.
        for (int odd = 1; odd < 2 * n; odd += 2)
            Assert.Equal(-1, arr.BinarySearch(odd));
    }

    // ── Cross-algorithm consistency ───────────────────────────────────────────
    //
    // All seven algorithms must produce the same output for the same input.

    [Fact]
    public void AllSevenSorts_ProduceSameOutput_OnRandomInput()
    {
        var rng = new Random(999);
        int[] input = Enumerable.Range(0, 50).Select(_ => rng.Next(200)).ToArray();
        int[] expected = input.OrderBy(x => x).ToArray();

        string[] algorithms = ["Bubble", "Selection", "Insertion", "Shell", "Merge", "Quick", "Heap"];

        foreach (string algo in algorithms)
        {
            var arr = ArrayOf(input);
            InvokeSort(arr, algo);
            Assert.Equal(expected, arr);
        }
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static SortableDynamicArray<int> ArrayOf(params int[] values)
    {
        var arr = new SortableDynamicArray<int>();
        foreach (var v in values) arr.Add(v);
        return arr;
    }

    private static void InvokeSort(SortableDynamicArray<int> arr, string algorithm)
    {
        switch (algorithm)
        {
            case "Bubble":    arr.BubbleSort();    break;
            case "Selection": arr.SelectionSort(); break;
            case "Insertion": arr.InsertionSort(); break;
            case "Shell":     arr.ShellSort();     break;
            case "Merge":     arr.MergeSort();     break;
            case "Quick":     arr.QuickSort();     break;
            case "Heap":      arr.HeapSort();      break;
            default: throw new ArgumentException($"Unknown algorithm: {algorithm}");
        }
    }

    // Used by the MergeSort stability test.
    private sealed record StableItem(int Key, char Label) : IComparable<StableItem>
    {
        public int CompareTo(StableItem? other)
        {
            if (other is null) return 1;
            return Key.CompareTo(other.Key);
        }
    }
}
