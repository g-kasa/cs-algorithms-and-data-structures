namespace Algorithms.Trees.Tests;

public class BinarySearchTreeTests
{
    // ── Insert ──────────────────────────────────────────────────────────────

    [Fact]
    public void Insert_SingleElement_CountIsOne()
    {
        var tree = new BinarySearchTree<int>();
        tree.Insert(5);
        Assert.Equal(1, tree.Count);
    }

    [Fact]
    public void Insert_Duplicate_IgnoredAndCountUnchanged()
    {
        var tree = new BinarySearchTree<int>();
        tree.Insert(5);
        tree.Insert(5);
        Assert.Equal(1, tree.Count);
    }

    [Fact]
    public void Insert_MultipleDistinctValues_CountMatchesInserted()
    {
        var tree = TreeOf(5, 3, 7, 1, 4, 6, 8);
        Assert.Equal(7, tree.Count);
    }

    // ── Contains ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(8)]
    public void Contains_InsertedElement_ReturnsTrue(int value)
    {
        var tree = TreeOf(5, 3, 7, 1, 4, 6, 8);
        Assert.True(tree.Contains(value));
    }

    [Fact]
    public void Contains_EmptyTree_ReturnsFalse()
    {
        Assert.False(new BinarySearchTree<int>().Contains(5));
    }

    [Fact]
    public void Contains_MissingElement_ReturnsFalse()
    {
        Assert.False(TreeOf(5, 3, 7).Contains(99));
    }

    // ── InOrder traversal ───────────────────────────────────────────────────

    [Fact]
    public void InOrder_MultipleElements_ReturnsSortedAscending()
    {
        Assert.Equal([1, 3, 4, 5, 6, 7, 8], TreeOf(5, 3, 7, 1, 4, 6, 8).InOrder());
    }

    [Fact]
    public void InOrder_InsertedInDescendingOrder_ReturnsSortedAscending()
    {
        Assert.Equal([1, 2, 3, 4, 5], TreeOf(5, 4, 3, 2, 1).InOrder());
    }

    [Fact]
    public void InOrder_EmptyTree_ReturnsEmpty()
    {
        Assert.Empty(new BinarySearchTree<int>().InOrder());
    }

    // ── PreOrder traversal ──────────────────────────────────────────────────

    [Fact]
    public void PreOrder_BalancedTree_ReturnsRootFirst()
    {
        // Root 5 must come before any child.
        var result = TreeOf(5, 3, 7).PreOrder().ToList();
        Assert.Equal(5, result[0]);
        Assert.Equal([5, 3, 7], result);
    }

    [Fact]
    public void PreOrder_EmptyTree_ReturnsEmpty()
    {
        Assert.Empty(new BinarySearchTree<int>().PreOrder());
    }

    // ── PostOrder traversal ─────────────────────────────────────────────────

    [Fact]
    public void PostOrder_BalancedTree_ReturnsRootLast()
    {
        // Root 5 must come after both children.
        var result = TreeOf(5, 3, 7).PostOrder().ToList();
        Assert.Equal(5, result[^1]);
        Assert.Equal([3, 7, 5], result);
    }

    [Fact]
    public void PostOrder_EmptyTree_ReturnsEmpty()
    {
        Assert.Empty(new BinarySearchTree<int>().PostOrder());
    }

    // ── LevelOrder traversal ────────────────────────────────────────────────

    [Fact]
    public void LevelOrder_BalancedTree_VisitsLevelByLevel()
    {
        //       5
        //      / \
        //     3   7
        //    / \ / \
        //   1  4 6  8
        Assert.Equal([5, 3, 7, 1, 4, 6, 8], TreeOf(5, 3, 7, 1, 4, 6, 8).LevelOrder());
    }

    [Fact]
    public void LevelOrder_EmptyTree_ReturnsEmpty()
    {
        Assert.Empty(new BinarySearchTree<int>().LevelOrder());
    }

    // ── Remove ──────────────────────────────────────────────────────────────

    [Fact]
    public void Remove_Leaf_DeletesNode()
    {
        var tree = TreeOf(5, 3, 7);
        Assert.True(tree.Remove(3));
        Assert.False(tree.Contains(3));
        Assert.Equal(2, tree.Count);
    }

    [Fact]
    public void Remove_NodeWithOneChild_ReplacesWithChild()
    {
        var tree = TreeOf(5, 3, 7, 2); // 3 has a left child (2), no right child
        Assert.True(tree.Remove(3));
        Assert.False(tree.Contains(3));
        Assert.True(tree.Contains(2));
        Assert.Equal([2, 5, 7], tree.InOrder());
    }

    [Fact]
    public void Remove_NodeWithTwoChildren_ReplacesWithSuccessor()
    {
        var tree = TreeOf(5, 3, 7, 1, 4); // 3 has children 1 and 4
        Assert.True(tree.Remove(3));
        Assert.False(tree.Contains(3));
        Assert.True(tree.Contains(1));
        Assert.True(tree.Contains(4));
        Assert.Equal([1, 4, 5, 7], tree.InOrder());
    }

    [Fact]
    public void Remove_Root_TreeRemainsValid()
    {
        var tree = TreeOf(5, 3, 7);
        Assert.True(tree.Remove(5));
        Assert.False(tree.Contains(5));
        Assert.Equal(2, tree.Count);
        Assert.Equal([3, 7], tree.InOrder());
    }

    [Fact]
    public void Remove_MissingElement_ReturnsFalse()
    {
        var tree = TreeOf(5, 3, 7);
        Assert.False(tree.Remove(99));
        Assert.Equal(3, tree.Count);
    }

    [Fact]
    public void Remove_EmptyTree_ReturnsFalse()
    {
        Assert.False(new BinarySearchTree<int>().Remove(1));
    }

    [Fact]
    public void Remove_AllElements_TreeIsEmpty()
    {
        var tree = TreeOf(5, 3, 7);
        tree.Remove(5);
        tree.Remove(3);
        tree.Remove(7);
        Assert.Equal(0, tree.Count);
        Assert.Empty(tree.InOrder());
    }

    // ── Large-input regression ──────────────────────────────────────────────

    [Fact]
    public void Insert_LargeRandomInput_InOrderYieldsSortedSequence()
    {
        var rng = new Random(42);
        var values = Enumerable.Range(0, 1_000).Select(_ => rng.Next(10_000)).Distinct().ToArray();
        var tree = new BinarySearchTree<int>();
        foreach (var v in values) tree.Insert(v);

        Assert.Equal(values.OrderBy(x => x), tree.InOrder());
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static BinarySearchTree<int> TreeOf(params int[] values)
    {
        var tree = new BinarySearchTree<int>();
        foreach (var v in values) tree.Insert(v);
        return tree;
    }
}
