using System.Collections;

namespace Algorithms.Lists;

/// <summary>A resizable array that grows automatically as elements are added.</summary>
/// <typeparam name="T">The element type; must implement <see cref="IComparable{T}"/> to support sorting and binary search.</typeparam>
/// <remarks>
/// Backed by a contiguous <c>T[]</c> that starts at capacity 4 and doubles whenever it is full.
/// The backing array never shrinks automatically.
/// Time:  Add O(1) amortized, Insert/RemoveAt/Remove O(n), Contains/IndexOf O(n),
///        BinarySearch O(log n), Sort O(n log n), Rotate O(n), indexer O(1).
/// Space: O(n).
/// </remarks>
public sealed class DynamicArray<T> : IEnumerable<T> where T : IComparable<T>
{
    private const int InitialCapacity = 4;

    private T[] _items = new T[InitialCapacity];

    /// <summary>Gets the number of elements currently stored in the array.</summary>
    public int Count { get; private set; }

    /// <summary>Gets the current length of the backing array.</summary>
    public int Capacity => _items.Length;

    /// <summary>Gets or sets the element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="index"/> is less than 0 or greater than or equal to <see cref="Count"/>.
    /// </exception>
    /// <remarks>Time: O(1).</remarks>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range for an array with {Count} element(s).");
            return _items[index];
        }
        set
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range for an array with {Count} element(s).");
            _items[index] = value;
        }
    }

    /// <summary>Appends <paramref name="value"/> to the end of the array.</summary>
    /// <remarks>Time: O(1) amortized — a capacity doubling occurs at most once every n additions.</remarks>
    public void Add(T value)
    {
        EnsureCapacity();
        _items[Count] = value;
        Count++;
    }

    /// <summary>Inserts <paramref name="value"/> at <paramref name="index"/>, shifting all subsequent elements one position to the right.</summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="index"/> is less than 0 or greater than <see cref="Count"/>.
    /// Inserting at <see cref="Count"/> is valid and equivalent to <see cref="Add"/>.
    /// </exception>
    /// <remarks>Time: O(n) — shifts up to n elements.</remarks>
    public void Insert(int index, T value)
    {
        if (index < 0 || index > Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range for an array with {Count} element(s). Valid insert indices are 0 to {Count}.");

        EnsureCapacity();

        // Shift elements right to open a slot at index.
        for (int i = Count; i > index; i--)
            _items[i] = _items[i - 1];

        _items[index] = value;
        Count++;
    }

    /// <summary>Removes the element at <paramref name="index"/>, shifting all subsequent elements one position to the left.</summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="index"/> is less than 0 or greater than or equal to <see cref="Count"/>.
    /// </exception>
    /// <remarks>Time: O(n) — shifts up to n elements.</remarks>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range for an array with {Count} element(s).");

        // Shift elements left to close the gap left by the removed slot.
        for (int i = index; i < Count - 1; i++)
            _items[i] = _items[i + 1];

        // Clear the stale reference at the now-unused tail slot so the GC can collect it.
        _items[Count - 1] = default!;
        Count--;
    }

    /// <summary>Removes the first occurrence of <paramref name="value"/> from the array.</summary>
    /// <returns><c>true</c> if <paramref name="value"/> was found and removed; otherwise <c>false</c>.</returns>
    /// <remarks>Time: O(n).</remarks>
    public bool Remove(T value)
    {
        int index = IndexOf(value);
        if (index == -1) return false;

        RemoveAt(index);
        return true;
    }

    /// <summary>Determines whether the array contains <paramref name="value"/>.</summary>
    /// <remarks>Time: O(n).</remarks>
    public bool Contains(T value) => IndexOf(value) != -1;

    /// <summary>Returns the zero-based index of the first occurrence of <paramref name="value"/>, or -1 if not found.</summary>
    /// <remarks>Time: O(n).</remarks>
    public int IndexOf(T value)
    {
        for (int i = 0; i < Count; i++)
        {
            if (_items[i].CompareTo(value) == 0) return i;
        }
        return -1;
    }

    /// <summary>
    /// Searches a <em>sorted</em> array for <paramref name="value"/> using binary search and returns its zero-based index, or -1 if not found.
    /// </summary>
    /// <remarks>
    /// The array must be sorted in ascending order before calling this method; behaviour is undefined otherwise.
    /// Time: O(log n).
    /// </remarks>
    public int BinarySearch(T value)
    {
        int low = 0;
        int high = Count - 1;

        while (low <= high)
        {
            // Compute the midpoint without integer overflow.
            int mid = low + (high - low) / 2;
            int comparison = _items[mid].CompareTo(value);

            if (comparison == 0) return mid;
            if (comparison < 0) low = mid + 1;
            else high = mid - 1;
        }

        return -1;
    }

    /// <summary>Sorts the array in ascending order.</summary>
    /// <remarks>
    /// Delegates to <see cref="Array.Sort{T}(T[], int, int)"/> on the live slice,
    /// which uses an introspective sort (introsort) internally.
    /// Time: O(n log n) average and worst. Space: O(log n) stack space for recursion.
    /// </remarks>
    public void Sort() => Array.Sort(_items, 0, Count);

    /// <summary>
    /// Rotates the array left by <paramref name="steps"/> positions in place.
    /// A negative value rotates right.
    /// </summary>
    /// <remarks>
    /// Uses the three-reverse trick: reverse [0, steps-1], reverse [steps, n-1], reverse [0, n-1].
    /// The steps value is normalised so any integer is accepted safely.
    /// Time: O(n). Space: O(1).
    /// </remarks>
    public void Rotate(int steps)
    {
        if (Count == 0) return;

        // Normalise to a value in [0, Count) so that negative steps and values
        // larger than Count both produce the canonical left-rotation distance.
        steps = ((steps % Count) + Count) % Count;
        if (steps == 0) return;

        Reverse(0, steps - 1);
        Reverse(steps, Count - 1);
        Reverse(0, Count - 1);
    }

    /// <summary>Returns an enumerator that iterates over the live elements from index 0 to <see cref="Count"/> - 1.</summary>
    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
            yield return _items[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // Doubles the backing array when it is full.
    private void EnsureCapacity()
    {
        if (Count < _items.Length) return;

        var larger = new T[_items.Length * 2];
        Array.Copy(_items, larger, Count);
        _items = larger;
    }

    // Reverses the slice _items[lo..hi] inclusive.
    private void Reverse(int lo, int hi)
    {
        while (lo < hi)
        {
            (_items[lo], _items[hi]) = (_items[hi], _items[lo]);
            lo++;
            hi--;
        }
    }
}
