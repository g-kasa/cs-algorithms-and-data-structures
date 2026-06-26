using System.Collections;

namespace Algorithms.Lists;

/// <summary>A resizable array that grows automatically as elements are added.</summary>
/// <typeparam name="T">The element type; must be non-nullable.</typeparam>
/// <remarks>
/// Backed by a contiguous <c>T[]</c> that starts at capacity 4 and doubles whenever it is full.
/// The backing array never shrinks automatically.
/// Implements <see cref="IList{T}"/>; all index-based operations are O(1) via the backing array.
/// All public members are thread-safe: every operation serialises through an internal monitor lock.
/// <see cref="GetEnumerator"/> returns an enumerator over a point-in-time snapshot so callers
/// do not need to hold a lock while iterating.
/// Time:  Add O(1) amortized, Insert/RemoveAt/Remove O(n), Contains/IndexOf O(n),
///        Rotate O(n), indexer O(1).
/// Space: O(n).
/// </remarks>
public class DynamicArray<T> : IList<T> where T : notnull
{
    private const int InitialCapacity = 4;

    protected T[] _items = new T[InitialCapacity];
    protected readonly object _syncRoot = new();

    /// <summary>Gets the number of elements currently stored in the array.</summary>
    public int Count { get; protected set; }

    /// <summary>Gets the current length of the backing array.</summary>
    public int Capacity
    {
        get { lock (_syncRoot) { return _items.Length; } }
    }

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>Gets or sets the element at <paramref name="index"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="index"/> is less than 0 or greater than or equal to <see cref="Count"/>.
    /// </exception>
    /// <remarks>Time: O(1).</remarks>
    public T this[int index]
    {
        get
        {
            lock (_syncRoot)
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range for an array with {Count} element(s).");
                return _items[index];
            }
        }
        set
        {
            lock (_syncRoot)
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range for an array with {Count} element(s).");
                _items[index] = value;
            }
        }
    }

    /// <summary>Appends <paramref name="value"/> to the end of the array.</summary>
    /// <remarks>Time: O(1) amortized — a capacity doubling occurs at most once every n additions.</remarks>
    public void Add(T value)
    {
        lock (_syncRoot)
        {
            EnsureCapacity();
            _items[Count] = value;
            Count++;
        }
    }

    /// <summary>Removes all elements from the array.</summary>
    /// <remarks>Time: O(n) — clears stale references so the GC can collect them.</remarks>
    public void Clear()
    {
        lock (_syncRoot)
        {
            Array.Clear(_items, 0, Count);
            Count = 0;
        }
    }

    /// <summary>Inserts <paramref name="value"/> at <paramref name="index"/>, shifting all subsequent elements one position to the right.</summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="index"/> is less than 0 or greater than <see cref="Count"/>.
    /// Inserting at <see cref="Count"/> is valid and equivalent to <see cref="Add"/>.
    /// </exception>
    /// <remarks>Time: O(n) — shifts up to n elements.</remarks>
    public void Insert(int index, T value)
    {
        lock (_syncRoot)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range for an array with {Count} element(s). Valid insert indices are 0 to {Count}.");

            EnsureCapacity();

            for (int i = Count; i > index; i--)
                _items[i] = _items[i - 1];

            _items[index] = value;
            Count++;
        }
    }

    /// <summary>Removes the element at <paramref name="index"/>, shifting all subsequent elements one position to the left.</summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="index"/> is less than 0 or greater than or equal to <see cref="Count"/>.
    /// </exception>
    /// <remarks>Time: O(n) — shifts up to n elements.</remarks>
    public void RemoveAt(int index)
    {
        lock (_syncRoot)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range for an array with {Count} element(s).");

            for (int i = index; i < Count - 1; i++)
                _items[i] = _items[i + 1];

            _items[Count - 1] = default!;
            Count--;
        }
    }

    /// <summary>Removes the first occurrence of <paramref name="value"/> from the array.</summary>
    /// <returns><c>true</c> if <paramref name="value"/> was found and removed; otherwise <c>false</c>.</returns>
    /// <remarks>Time: O(n).</remarks>
    public bool Remove(T value)
    {
        lock (_syncRoot)
        {
            int index = IndexOfCore(value);
            if (index == -1) return false;
            RemoveAtCore(index);
            return true;
        }
    }

    /// <summary>Determines whether the array contains <paramref name="value"/>.</summary>
    /// <remarks>Time: O(n).</remarks>
    public bool Contains(T value)
    {
        lock (_syncRoot) { return IndexOfCore(value) != -1; }
    }

    /// <summary>Returns the zero-based index of the first occurrence of <paramref name="value"/>, or -1 if not found.</summary>
    /// <remarks>Time: O(n).</remarks>
    public int IndexOf(T value)
    {
        lock (_syncRoot) { return IndexOfCore(value); }
    }

    /// <summary>Copies all elements to <paramref name="array"/> starting at <paramref name="arrayIndex"/>.</summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="arrayIndex"/> is negative.</exception>
    /// <exception cref="ArgumentException">Thrown when the destination array is too small.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);

        lock (_syncRoot)
        {
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("Destination array is too small.", nameof(array));
            Array.Copy(_items, 0, array, arrayIndex, Count);
        }
    }

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
        lock (_syncRoot)
        {
            if (Count == 0) return;
            steps = ((steps % Count) + Count) % Count;
            if (steps == 0) return;
            ReverseSegment(0, steps - 1);
            ReverseSegment(steps, Count - 1);
            ReverseSegment(0, Count - 1);
        }
    }

    /// <summary>Returns an enumerator over a point-in-time snapshot of the array.</summary>
    public IEnumerator<T> GetEnumerator()
    {
        T[] snapshot;
        lock (_syncRoot)
        {
            snapshot = new T[Count];
            Array.Copy(_items, snapshot, Count);
        }
        return ((IEnumerable<T>)snapshot).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // Doubles the backing array when it is full. Must be called within a locked section.
    protected void EnsureCapacity()
    {
        if (Count < _items.Length) return;
        var larger = new T[_items.Length * 2];
        Array.Copy(_items, larger, Count);
        _items = larger;
    }

    // Reverses the slice _items[lo..hi] inclusive. Must be called within a locked section.
    protected void ReverseSegment(int lo, int hi)
    {
        while (lo < hi)
        {
            (_items[lo], _items[hi]) = (_items[hi], _items[lo]);
            lo++; hi--;
        }
    }

    // Searches for value without acquiring the lock. Must be called within a locked section.
    private int IndexOfCore(T value)
    {
        for (int i = 0; i < Count; i++)
            if (EqualityComparer<T>.Default.Equals(_items[i], value)) return i;
        return -1;
    }

    // Removes at index without acquiring the lock. Must be called within a locked section.
    private void RemoveAtCore(int index)
    {
        for (int i = index; i < Count - 1; i++) _items[i] = _items[i + 1];
        _items[Count - 1] = default!;
        Count--;
    }
}
