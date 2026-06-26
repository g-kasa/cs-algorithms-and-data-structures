using System.Collections;

namespace Algorithms.Lists;

/// <summary>
/// A probabilistic sorted data structure that maintains multiple levels of linked lists,
/// allowing binary-search-like skipping over large portions of the data on each operation.
/// </summary>
/// <typeparam name="T">The element type; must be non-nullable and implement <see cref="IComparable{T}"/>.</typeparam>
/// <remarks>
/// Time:  Insert O(log n) average, O(n) worst. Contains O(log n) average, O(n) worst.
///        Remove O(log n) average, O(n) worst. Count O(1).
/// Space: O(n) expected for n elements (each node occupies O(1) levels on average given p = 0.5).
/// </remarks>
public sealed class SkipList<T> : IEnumerable<T> where T : notnull, IComparable<T>
{
    private const int MaxLevels = 16;

    private sealed class Node(T value, int levels)
    {
        internal readonly T Value = value;
        internal readonly Node?[] Forward = new Node?[levels];
    }

    // The header sentinel is never yielded to callers; its Value is unused.
    private readonly Node _header = new(default!, MaxLevels);

    // Count of currently active levels; callers iterate from `_currentLevel - 1` down to 0.
    private int _currentLevel;

    /// <summary>Gets the number of elements currently in the skip list.</summary>
    public int Count { get; private set; }

    /// <summary>Inserts <paramref name="value"/> into the skip list. Duplicates are allowed.</summary>
    /// <remarks>Time: O(log n) average, O(n) worst.</remarks>
    public void Insert(T value)
    {
        var update = BuildUpdateArray(value);

        int newLevel = RandomLevel();

        // If the new node reaches above the current top, the extra levels have no
        // predecessor other than the header itself.
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

    /// <summary>Determines whether the skip list contains at least one occurrence of <paramref name="value"/>.</summary>
    /// <remarks>Time: O(log n) average, O(n) worst.</remarks>
    public bool Contains(T value)
    {
        var current = _header;

        for (int level = _currentLevel - 1; level >= 0; level--)
        {
            while (current.Forward[level] is not null
                   && current.Forward[level]!.Value.CompareTo(value) < 0)
            {
                current = current.Forward[level]!;
            }
        }

        // After descending to level 0, the node immediately ahead is the closest
        // candidate that could equal value.
        var candidate = current.Forward[0];
        return candidate is not null && candidate.Value.CompareTo(value) == 0;
    }

    /// <summary>Removes one occurrence of <paramref name="value"/> from the skip list.</summary>
    /// <returns><c>true</c> if a matching element was found and removed; otherwise <c>false</c>.</returns>
    /// <remarks>Time: O(log n) average, O(n) worst.</remarks>
    public bool Remove(T value)
    {
        var update = BuildUpdateArray(value);

        var target = update[0].Forward[0];

        if (target is null || target.Value.CompareTo(value) != 0)
            return false;

        for (int level = 0; level < _currentLevel; level++)
        {
            // Levels above the target node's own height will not reference it.
            if (update[level].Forward[level] != target) break;
            update[level].Forward[level] = target.Forward[level];
        }

        // Shrink _currentLevel if the removal left any top-level header pointers empty.
        while (_currentLevel > 0 && _header.Forward[_currentLevel - 1] is null)
            _currentLevel--;

        Count--;
        return true;
    }

    /// <summary>Returns an enumerator that yields all values in ascending order via the level-0 chain.</summary>
    /// <remarks>Time: O(n).</remarks>
    public IEnumerator<T> GetEnumerator()
    {
        var current = _header.Forward[0];
        while (current is not null)
        {
            yield return current.Value;
            current = current.Forward[0];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // Walks from _currentLevel down to level 0, recording for each level the last node
    // whose Forward pointer must be updated to splice in (or out) a node at value.
    private Node[] BuildUpdateArray(T value)
    {
        var update = new Node[MaxLevels];
        var current = _header;

        for (int level = _currentLevel - 1; level >= 0; level--)
        {
            while (current.Forward[level] is not null
                   && current.Forward[level]!.Value.CompareTo(value) < 0)
            {
                current = current.Forward[level]!;
            }
            update[level] = current;
        }

        // Levels above _currentLevel are always "preceded" by the header.
        for (int level = _currentLevel; level < MaxLevels; level++)
            update[level] = _header;

        return update;
    }

    // Flips fair coins until tails or MaxLevels is reached.
    // Returns a level in [1, MaxLevels].
    private static int RandomLevel()
    {
        int level = 1;
        while (level < MaxLevels && Random.Shared.NextDouble() < 0.5)
            level++;
        return level;
    }
}
