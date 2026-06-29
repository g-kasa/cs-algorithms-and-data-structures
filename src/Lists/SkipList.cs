using System.Collections;

namespace Algorithms.Lists;

/// <summary>
/// A probabilistic sorted data structure that maintains multiple levels of linked lists,
/// allowing binary-search-like skipping over large portions of the data on each operation.
/// </summary>
/// <typeparam name="T">The element type; must be non-nullable and implement <see cref="IComparable{T}"/>.</typeparam>
/// <remarks>
/// Implements <see cref="IList{T}"/>. Because the list is kept in sorted order, the following
/// <see cref="IList{T}"/> members throw <see cref="NotSupportedException"/> to preserve the invariant:
/// <list type="bullet">
/// <item><description><see cref="this[int]"/> setter — assigning by position could violate sort order.</description></item>
/// <item><description><see cref="Insert(int,T)"/> — inserting at an arbitrary position could violate sort order; use <see cref="Insert(T)"/> instead.</description></item>
/// </list>
/// All public members are thread-safe: every operation serialises through an internal monitor lock.
/// <see cref="GetEnumerator"/> returns an enumerator over a point-in-time snapshot so callers
/// do not need to hold a lock while iterating.
/// Time:  Insert O(log n) average, O(n) worst. Contains O(log n) average, O(n) worst.
///        Remove O(log n) average, O(n) worst. Count O(1).
///        IndexOf O(n). RemoveAt O(n). indexer getter O(n).
/// Space: O(n) expected for n elements (each node occupies O(1) levels on average given p = 0.5).
/// </remarks>
public sealed class SkipList<T> : IList<T> where T : notnull, IComparable<T>
{
    private const int MaxLevels = 16;

    private sealed class Node(T value, int levels)
    {
        internal readonly T Value = value;
        internal readonly Node?[] Forward = new Node?[levels];
    }

    // The header sentinel is never yielded to callers; its Value is unused.
    private readonly Node _header = new(default!, MaxLevels);
    private readonly object _syncRoot = new();

    // Count of currently active levels; callers iterate from `_currentLevel - 1` down to 0.
    private int _currentLevel;

    private int _count;

    /// <summary>Gets the number of elements currently in the skip list.</summary>
    public int Count
    {
        get => Volatile.Read(ref _count);
        private set => Volatile.Write(ref _count, value);
    }

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>Gets the element at <paramref name="index"/> by traversing the level-0 chain.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is out of range.</exception>
    /// <exception cref="NotSupportedException">Always thrown for the setter — assigning by position would break sorted order.</exception>
    /// <remarks>Time: O(n) — must count along the level-0 chain to reach position.</remarks>
    public T this[int index]
    {
        get { lock (_syncRoot) { return NodeAt(index).Value; } }
        set => throw new NotSupportedException("SkipList maintains sorted order; element values cannot be changed by index.");
    }

    /// <summary>Inserts <paramref name="value"/> into the skip list in sorted position. Duplicates are allowed.</summary>
    /// <remarks>Time: O(log n) average, O(n) worst.</remarks>
    public void Insert(T value)
    {
        lock (_syncRoot)
        {
            var update = BuildUpdateArray(value);
            int newLevel = RandomLevel();

            if (newLevel > _currentLevel)
            {
                for (int level = _currentLevel; level < newLevel; level++)
                    update[level] = _header;
                _currentLevel = newLevel;
            }

            var node = new Node(value, newLevel);

            for (int level = 0; level < newLevel; level++)
            {
                node.Forward[level] = update[level].Forward[level];
                update[level].Forward[level] = node;
            }

            Count++;
        }
    }

    /// <summary>Appends <paramref name="item"/> in sorted position. Duplicates are allowed.</summary>
    /// <remarks>Time: O(log n) average — delegates to <see cref="Insert(T)"/>.</remarks>
    public void Add(T item) => Insert(item);

    /// <summary>Removes all elements from the skip list.</summary>
    /// <remarks>Time: O(1).</remarks>
    public void Clear()
    {
        lock (_syncRoot)
        {
            Array.Clear(_header.Forward, 0, _header.Forward.Length);
            _currentLevel = 0;
            Count = 0;
        }
    }

    /// <summary>Determines whether the skip list contains at least one occurrence of <paramref name="value"/>.</summary>
    /// <remarks>Time: O(log n) average, O(n) worst.</remarks>
    public bool Contains(T value)
    {
        lock (_syncRoot)
        {
            var current = _header;
            for (int level = _currentLevel - 1; level >= 0; level--)
            {
                while (current.Forward[level] is not null
                       && current.Forward[level]!.Value.CompareTo(value) < 0)
                    current = current.Forward[level]!;
            }
            var candidate = current.Forward[0];
            while (candidate is not null && candidate.Value.CompareTo(value) == 0)
            {
                if (EqualityComparer<T>.Default.Equals(candidate.Value, value)) return true;
                candidate = candidate.Forward[0];
            }
            return false;
        }
    }

    /// <summary>Returns the zero-based index of the first occurrence of <paramref name="item"/> in level-0 order, or -1 if not found.</summary>
    /// <remarks>Time: O(n) — must count along the level-0 chain; exits early once values exceed <paramref name="item"/>.</remarks>
    public int IndexOf(T item)
    {
        lock (_syncRoot)
        {
            int index = 0;
            var current = _header.Forward[0];
            while (current is not null)
            {
                int cmp = current.Value.CompareTo(item);
                if (cmp > 0) break;
                if (cmp == 0 && EqualityComparer<T>.Default.Equals(current.Value, item)) return index;
                current = current.Forward[0];
                index++;
            }
            return -1;
        }
    }

    /// <summary>Not supported — inserting at an arbitrary index would break sorted order.</summary>
    /// <exception cref="NotSupportedException">Always thrown. Use <see cref="Insert(T)"/> to add in sorted position.</exception>
    public void Insert(int index, T item) =>
        throw new NotSupportedException("SkipList maintains sorted order; use Insert(T) to add in sorted position.");

    /// <summary>Removes the element at position <paramref name="index"/> by traversing the level-0 chain.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is less than 0 or greater than or equal to <see cref="Count"/>.</exception>
    /// <remarks>
    /// Time: O(n) — O(n) to walk level-0 to the position, then O(MaxLevels) reference-equality scans (constant factor).
    /// </remarks>
    public void RemoveAt(int index)
    {
        lock (_syncRoot)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            // Walk level-0 to find the target node by position.
            var current = _header;
            for (int i = 0; i < index; i++) current = current.Forward[0]!;
            var target = current.Forward[0]!;

            // For each level, find the predecessor that directly points to target.
            var update = new Node[MaxLevels];
            for (int level = 0; level < MaxLevels; level++)
            {
                var walker = _header;
                while (walker.Forward[level] is not null && !ReferenceEquals(walker.Forward[level], target))
                    walker = walker.Forward[level]!;
                update[level] = walker;
            }

            for (int level = 0; level < _currentLevel; level++)
            {
                if (!ReferenceEquals(update[level].Forward[level], target)) break;
                update[level].Forward[level] = target.Forward[level];
            }

            while (_currentLevel > 0 && _header.Forward[_currentLevel - 1] is null)
                _currentLevel--;

            Count--;
        }
    }

    /// <summary>Removes one occurrence of <paramref name="value"/> from the skip list.</summary>
    /// <returns><c>true</c> if a matching element was found and removed; otherwise <c>false</c>.</returns>
    /// <remarks>Time: O(log n) average, O(n) worst.</remarks>
    public bool Remove(T value)
    {
        lock (_syncRoot)
        {
            // Navigate to the first CompareTo==0 position, then walk forward to find the Equals match.
            var update = BuildUpdateArray(value);
            var target = update[0].Forward[0];
            while (target is not null && target.Value.CompareTo(value) == 0)
            {
                if (EqualityComparer<T>.Default.Equals(target.Value, value)) break;
                target = target.Forward[0];
            }

            if (target is null || target.Value.CompareTo(value) != 0)
                return false;

            // Rebuild predecessor links for the exact target node at each level.
            for (int level = 0; level < MaxLevels; level++)
            {
                var walker = _header;
                while (walker.Forward[level] is not null && !ReferenceEquals(walker.Forward[level], target))
                    walker = walker.Forward[level]!;
                update[level] = walker;
            }

            for (int level = 0; level < _currentLevel; level++)
            {
                if (!ReferenceEquals(update[level].Forward[level], target)) break;
                update[level].Forward[level] = target.Forward[level];
            }

            while (_currentLevel > 0 && _header.Forward[_currentLevel - 1] is null)
                _currentLevel--;

            Count--;
            return true;
        }
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
            var current = _header.Forward[0];
            while (current is not null) { array[arrayIndex++] = current.Value; current = current.Forward[0]; }
        }
    }

    /// <summary>Returns an enumerator over a point-in-time snapshot yielding all values in ascending order.</summary>
    public IEnumerator<T> GetEnumerator()
    {
        T[] snapshot;
        lock (_syncRoot)
        {
            snapshot = new T[Count];
            int i = 0;
            var current = _header.Forward[0];
            while (current is not null) { snapshot[i++] = current.Value; current = current.Forward[0]; }
        }
        return ((IEnumerable<T>)snapshot).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // Traverses the level-0 chain to the node at `index`. Must be called within a locked section.
    private Node NodeAt(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index));
        var current = _header.Forward[0];
        for (int i = 0; i < index; i++) current = current!.Forward[0];
        return current!;
    }

    // Walks from _currentLevel down to level 0, recording for each level the last node
    // whose Forward pointer must be updated to splice in (or out) a node at value.
    // Must be called within a locked section.
    private Node[] BuildUpdateArray(T value)
    {
        var update = new Node[MaxLevels];
        var current = _header;

        for (int level = _currentLevel - 1; level >= 0; level--)
        {
            while (current.Forward[level] is not null
                   && current.Forward[level]!.Value.CompareTo(value) < 0)
                current = current.Forward[level]!;
            update[level] = current;
        }

        for (int level = _currentLevel; level < MaxLevels; level++)
            update[level] = _header;

        return update;
    }

    // Flips fair coins until tails or MaxLevels is reached. Returns a level in [1, MaxLevels].
    private static int RandomLevel()
    {
        int level = 1;
        while (level < MaxLevels && Random.Shared.NextDouble() < 0.5)
            level++;
        return level;
    }
}
