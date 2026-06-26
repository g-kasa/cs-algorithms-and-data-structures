namespace Algorithms.Lists;

/// <summary>
/// A dynamic array that extends <see cref="DynamicArray{T}"/> with binary search and seven
/// classic sorting algorithms: Bubble Sort, Selection Sort, Insertion Sort, Shell Sort,
/// Merge Sort, Quick Sort, and Heap Sort.
/// </summary>
/// <typeparam name="T">
/// The element type; must implement <see cref="IComparable{T}"/> because every algorithm
/// relies on a total order.
/// </typeparam>
/// <remarks>
/// Inherits all base operations (Add, Insert, RemoveAt, Remove, Contains, IndexOf, Rotate,
/// indexer) at their original complexities.
///
/// Time:  BinarySearch O(log n) — requires a sorted array.
///        BubbleSort O(n²) average/worst, O(n) best (already sorted).
///        SelectionSort O(n²) always.
///        InsertionSort O(n²) average/worst, O(n) best (already sorted).
///        ShellSort O(n log² n) with Knuth gap sequence.
///        MergeSort O(n log n) always, stable.
///        QuickSort O(n log n) average, O(n²) worst; median-of-three pivot reduces worst-case likelihood.
///        HeapSort O(n log n) always, in-place.
/// Space: O(n) for the array itself. BinarySearch/BubbleSort/SelectionSort/InsertionSort/ShellSort/HeapSort O(1) auxiliary.
///        MergeSort O(n) auxiliary (one extra array). QuickSort O(log n) auxiliary (recursion stack).
/// </remarks>
public sealed class SortableDynamicArray<T> : DynamicArray<T> where T : notnull, IComparable<T>
{
    /// <summary>
    /// Searches a sorted array for <paramref name="value"/> using binary search.
    /// </summary>
    /// <param name="value">The value to search for.</param>
    /// <returns>The zero-based index of <paramref name="value"/>, or -1 if not found.</returns>
    /// <remarks>
    /// The array must be sorted in ascending order before calling this method; the result is
    /// undefined on an unsorted array.
    /// Time: O(log n). Space: O(1).
    /// </remarks>
    public int BinarySearch(T value)
    {
        int low = 0;
        int high = Count - 1;

        while (low <= high)
        {
            int mid = low + (high - low) / 2;
            int comparison = _items[mid].CompareTo(value);

            if (comparison < 0)
                low = mid + 1;       // mid is too small; search the right half
            else if (comparison > 0)
                high = mid - 1;      // mid is too large; search the left half
            else
                return mid;
        }

        return -1;
    }

    /// <summary>
    /// Sorts the array in ascending order using Bubble Sort. Adjacent elements that are out
    /// of order are swapped each pass. The unsorted boundary shrinks by one after every pass
    /// because the largest unsorted element has bubbled to its final position. Sorting stops
    /// early when a full pass produces no swaps.
    /// </summary>
    /// <remarks>
    /// Time: O(n²) average and worst case; O(n) best case (already sorted). Space: O(1).
    /// </remarks>
    public void BubbleSort()
    {
        int unsortedBoundary = Count - 1;

        while (unsortedBoundary > 0)
        {
            bool swapped = false;

            for (int i = 0; i < unsortedBoundary; i++)
            {
                if (_items[i].CompareTo(_items[i + 1]) > 0)
                {
                    (_items[i], _items[i + 1]) = (_items[i + 1], _items[i]);
                    swapped = true;
                }
            }

            if (!swapped) break;

            unsortedBoundary--;
        }
    }

    /// <summary>
    /// Sorts the array in ascending order using Selection Sort. For each position <c>i</c>
    /// the minimum element in the unsorted suffix <c>[i+1, Count-1]</c> is found and swapped
    /// into position <c>i</c>.
    /// </summary>
    /// <remarks>
    /// Time: O(n²) always — the inner scan always runs to the end of the unsorted region.
    /// Space: O(1).
    /// </remarks>
    public void SelectionSort()
    {
        for (int i = 0; i < Count - 1; i++)
        {
            int minimumIndex = i;

            for (int j = i + 1; j < Count; j++)
            {
                if (_items[j].CompareTo(_items[minimumIndex]) < 0)
                    minimumIndex = j;
            }

            if (minimumIndex != i)
                (_items[i], _items[minimumIndex]) = (_items[minimumIndex], _items[i]);
        }
    }

    /// <summary>
    /// Sorts the array in ascending order using Insertion Sort. For each index <c>i</c>,
    /// the element at <c>_items[i]</c> is stored as a key and elements in the sorted prefix
    /// <c>[0, i-1]</c> that are greater than the key are shifted one position to the right,
    /// opening a slot for the key at its correct position.
    /// </summary>
    /// <remarks>
    /// Time: O(n²) average and worst case; O(n) best case (already sorted). Space: O(1).
    /// </remarks>
    public void InsertionSort()
    {
        for (int i = 1; i < Count; i++)
        {
            T key = _items[i];
            int j = i - 1;

            while (j >= 0 && _items[j].CompareTo(key) > 0)
            {
                _items[j + 1] = _items[j];
                j--;
            }

            _items[j + 1] = key;
        }
    }

    /// <summary>
    /// Sorts the array in ascending order using Shell Sort with Knuth's gap sequence
    /// (1, 4, 13, 40, …). An insertion-sort pass is performed at each gap width; shrinking
    /// the gap to 1 on the final pass guarantees a fully sorted result.
    /// </summary>
    /// <remarks>
    /// The Knuth sequence (gap = 3×gap + 1) gives O(n log² n) empirical performance, which
    /// is significantly faster than plain insertion sort in practice while remaining O(1) auxiliary.
    /// Time: O(n log² n) with Knuth gaps. Space: O(1).
    /// </remarks>
    public void ShellSort()
    {
        int gap = 1;
        while (gap < Count)
            gap = 3 * gap + 1;
        gap /= 3;

        while (gap >= 1)
        {
            for (int i = gap; i < Count; i++)
            {
                T key = _items[i];
                int j = i;

                while (j >= gap && _items[j - gap].CompareTo(key) > 0)
                {
                    _items[j] = _items[j - gap];
                    j -= gap;
                }

                _items[j] = key;
            }

            gap /= 3;
        }
    }

    /// <summary>
    /// Sorts the array in ascending order using Merge Sort. A single auxiliary array is
    /// allocated once and reused throughout the recursion to avoid repeated allocations.
    /// Equal elements retain their original relative order (stable sort).
    /// </summary>
    /// <remarks>
    /// Time: O(n log n) always. Space: O(n) auxiliary for the scratch array.
    /// </remarks>
    public void MergeSort()
    {
        if (Count < 2) return;

        T[] auxiliary = new T[Count];
        MergeSortCore(_items, auxiliary, 0, Count - 1);
    }

    /// <summary>
    /// Sorts the array in ascending order using Quick Sort with a median-of-three pivot.
    /// The three candidates — <c>items[lo]</c>, <c>items[mid]</c>, <c>items[hi]</c> — are
    /// partially sorted so that <c>items[hi]</c> holds the median, which then serves as the
    /// Lomuto partition pivot. This strategy avoids worst-case O(n²) behaviour on already-sorted
    /// or reverse-sorted input.
    /// </summary>
    /// <remarks>
    /// Time: O(n log n) average; O(n²) worst (unlikely with median-of-three). Space: O(log n) recursion stack.
    /// </remarks>
    public void QuickSort()
    {
        if (Count < 2) return;

        QuickSortCore(_items, 0, Count - 1);
    }

    /// <summary>
    /// Sorts the array in ascending order using Heap Sort. A max-heap is built in place from
    /// the backing array, then the maximum element is repeatedly extracted to the sorted suffix.
    /// </summary>
    /// <remarks>
    /// Time: O(n log n) always — O(n) to build the heap, O(n log n) to extract elements.
    /// Space: O(1) auxiliary.
    /// </remarks>
    public void HeapSort()
    {
        if (Count < 2) return;

        // Build max-heap: heapify from the last non-leaf down to the root.
        for (int i = Count / 2 - 1; i >= 0; i--)
            SiftDown(_items, i, Count);

        // Extract the maximum (root) into the sorted suffix one element at a time.
        for (int end = Count - 1; end >= 1; end--)
        {
            (_items[0], _items[end]) = (_items[end], _items[0]);
            SiftDown(_items, 0, end);
        }
    }

    /// <summary>Sorts the array in ascending order. Delegates to <see cref="MergeSort"/>.</summary>
    /// <remarks>Time: O(n log n). Space: O(n) auxiliary array.</remarks>
    public void Sort() => MergeSort();

    // Recursively splits items[lo..hi], sorts each half, then merges them.
    private static void MergeSortCore(T[] items, T[] auxiliary, int lo, int hi)
    {
        if (hi <= lo) return;

        int mid = lo + (hi - lo) / 2;
        MergeSortCore(items, auxiliary, lo, mid);
        MergeSortCore(items, auxiliary, mid + 1, hi);
        Merge(items, auxiliary, lo, mid, hi);
    }

    // Merges items[lo..mid] and items[mid+1..hi] into a single sorted run using auxiliary
    // as scratch space. Copying to auxiliary first lets us read from it while writing back
    // to items without overwriting values we still need.
    private static void Merge(T[] items, T[] auxiliary, int lo, int mid, int hi)
    {
        for (int k = lo; k <= hi; k++)
            auxiliary[k] = items[k];

        int left = lo;
        int right = mid + 1;

        for (int k = lo; k <= hi; k++)
        {
            if (left > mid)
                items[k] = auxiliary[right++];
            else if (right > hi)
                items[k] = auxiliary[left++];
            else if (auxiliary[right].CompareTo(auxiliary[left]) < 0)
                items[k] = auxiliary[right++];   // right element is strictly smaller; take it first to preserve stability
            else
                items[k] = auxiliary[left++];
        }
    }

    // Recursively partitions items[lo..hi] and sorts each partition.
    private static void QuickSortCore(T[] items, int lo, int hi)
    {
        if (lo >= hi) return;

        int pivotIndex = Partition(items, lo, hi);
        QuickSortCore(items, lo, pivotIndex - 1);
        QuickSortCore(items, pivotIndex + 1, hi);
    }

    // Partitions items[lo..hi] using Lomuto scheme with median-of-three pivot placed at items[hi].
    // Returns the final index of the pivot element.
    private static int Partition(T[] items, int lo, int hi)
    {
        int mid = lo + (hi - lo) / 2;

        // Sort items[lo], items[mid], items[hi] so that items[hi] holds the median.
        if (items[lo].CompareTo(items[mid]) > 0)
            (items[lo], items[mid]) = (items[mid], items[lo]);
        if (items[lo].CompareTo(items[hi]) > 0)
            (items[lo], items[hi]) = (items[hi], items[lo]);
        if (items[mid].CompareTo(items[hi]) > 0)
            (items[mid], items[hi]) = (items[hi], items[mid]);

        T pivot = items[hi];
        int i = lo - 1;

        for (int j = lo; j < hi; j++)
        {
            if (items[j].CompareTo(pivot) <= 0)
            {
                i++;
                (items[i], items[j]) = (items[j], items[i]);
            }
        }

        (items[i + 1], items[hi]) = (items[hi], items[i + 1]);
        return i + 1;
    }

    // Restores the max-heap property for the subtree rooted at `root` within items[0..count-1].
    // Repeatedly swaps the root down to the largest child until the heap property holds.
    private static void SiftDown(T[] items, int root, int count)
    {
        while (true)
        {
            int largest = root;
            int left = 2 * root + 1;
            int right = 2 * root + 2;

            if (left < count && items[left].CompareTo(items[largest]) > 0)
                largest = left;
            if (right < count && items[right].CompareTo(items[largest]) > 0)
                largest = right;

            if (largest == root) break;

            (items[root], items[largest]) = (items[largest], items[root]);
            root = largest;
        }
    }
}
