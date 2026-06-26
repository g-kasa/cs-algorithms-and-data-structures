namespace Algorithms.Graphs.Tests;

public class GraphTests
{
    // ── Vertices ─────────────────────────────────────────────────────────────

    [Fact]
    public void AddVertex_NewVertex_IsInGraph()
    {
        var graph = new Graph<int>();
        graph.AddVertex(1);
        Assert.True(graph.HasVertex(1));
        Assert.Equal(1, graph.VertexCount);
    }

    [Fact]
    public void AddVertex_Duplicate_VertexCountUnchanged()
    {
        var graph = new Graph<int>();
        graph.AddVertex(1);
        graph.AddVertex(1);
        Assert.Equal(1, graph.VertexCount);
    }

    [Fact]
    public void HasVertex_MissingVertex_ReturnsFalse()
    {
        Assert.False(new Graph<int>().HasVertex(99));
    }

    // ── Edges — undirected ───────────────────────────────────────────────────

    [Fact]
    public void AddEdge_UndirectedGraph_BothDirectionsExist()
    {
        var graph = new Graph<int>(directed: false);
        graph.AddEdge(1, 2);
        Assert.True(graph.HasEdge(1, 2));
        Assert.True(graph.HasEdge(2, 1));
    }

    [Fact]
    public void AddEdge_UndirectedGraph_CreatesVerticesAutomatically()
    {
        var graph = new Graph<int>();
        graph.AddEdge(1, 2);
        Assert.True(graph.HasVertex(1));
        Assert.True(graph.HasVertex(2));
        Assert.Equal(2, graph.VertexCount);
    }

    [Fact]
    public void AddEdge_DuplicateUndirectedEdge_EdgeCountUnchanged()
    {
        var graph = new Graph<int>();
        graph.AddEdge(1, 2);
        graph.AddEdge(1, 2);
        Assert.Equal(1, graph.EdgeCount);
    }

    // ── Edges — directed ─────────────────────────────────────────────────────

    [Fact]
    public void AddEdge_DirectedGraph_OnlyForwardEdgeExists()
    {
        var graph = new Graph<int>(directed: true);
        graph.AddEdge(1, 2);
        Assert.True(graph.HasEdge(1, 2));
        Assert.False(graph.HasEdge(2, 1));
    }

    [Fact]
    public void HasEdge_MissingEdge_ReturnsFalse()
    {
        var graph = new Graph<int>();
        graph.AddVertex(1);
        graph.AddVertex(2);
        Assert.False(graph.HasEdge(1, 2));
    }

    // ── BreadthFirstSearch ───────────────────────────────────────────────────

    [Fact]
    public void BreadthFirstSearch_ConnectedGraph_VisitsAllReachableVertices()
    {
        var graph = ConnectedSquare();
        var visited = graph.BreadthFirstSearch(1).ToHashSet();
        Assert.Equal(new HashSet<int> { 1, 2, 3, 4 }, visited);
    }

    [Fact]
    public void BreadthFirstSearch_DisconnectedGraph_VisitsOnlyReachableComponent()
    {
        var graph = new Graph<int>();
        graph.AddEdge(1, 2);
        graph.AddVertex(3); // isolated
        var visited = graph.BreadthFirstSearch(1).ToHashSet();
        Assert.Equal(new HashSet<int> { 1, 2 }, visited);
    }

    [Fact]
    public void BreadthFirstSearch_SingleVertex_ReturnsOnlyThatVertex()
    {
        var graph = new Graph<int>();
        graph.AddVertex(42);
        Assert.Equal([42], graph.BreadthFirstSearch(42));
    }

    [Fact]
    public void BreadthFirstSearch_UnknownStart_ReturnsEmpty()
    {
        Assert.Empty(new Graph<int>().BreadthFirstSearch(99));
    }

    [Fact]
    public void BreadthFirstSearch_DirectedLinearChain_VisitsLevelByLevel()
    {
        // 1 → 2 → 3 → 4
        var graph = new Graph<int>(directed: true);
        graph.AddEdge(1, 2);
        graph.AddEdge(2, 3);
        graph.AddEdge(3, 4);
        Assert.Equal([1, 2, 3, 4], graph.BreadthFirstSearch(1));
    }

    // ── DepthFirstSearch ─────────────────────────────────────────────────────

    [Fact]
    public void DepthFirstSearch_ConnectedGraph_VisitsAllReachableVertices()
    {
        var graph = ConnectedSquare();
        var visited = graph.DepthFirstSearch(1).ToHashSet();
        Assert.Equal(new HashSet<int> { 1, 2, 3, 4 }, visited);
    }

    [Fact]
    public void DepthFirstSearch_DisconnectedGraph_VisitsOnlyReachableComponent()
    {
        var graph = new Graph<int>();
        graph.AddEdge(1, 2);
        graph.AddVertex(3);
        var visited = graph.DepthFirstSearch(1).ToHashSet();
        Assert.DoesNotContain(3, visited);
    }

    [Fact]
    public void DepthFirstSearch_SingleVertex_ReturnsOnlyThatVertex()
    {
        var graph = new Graph<int>();
        graph.AddVertex(7);
        Assert.Equal([7], graph.DepthFirstSearch(7));
    }

    [Fact]
    public void DepthFirstSearch_UnknownStart_ReturnsEmpty()
    {
        Assert.Empty(new Graph<int>().DepthFirstSearch(99));
    }

    // ── HasPath ──────────────────────────────────────────────────────────────

    [Fact]
    public void HasPath_DirectPath_ReturnsTrue()
    {
        var graph = new Graph<int>();
        graph.AddEdge(1, 2);
        Assert.True(graph.HasPath(1, 2));
    }

    [Fact]
    public void HasPath_IndirectPath_ReturnsTrue()
    {
        var graph = new Graph<int>();
        graph.AddEdge(1, 2);
        graph.AddEdge(2, 3);
        Assert.True(graph.HasPath(1, 3));
    }

    [Fact]
    public void HasPath_NoPath_ReturnsFalse()
    {
        var graph = new Graph<int>();
        graph.AddEdge(1, 2);
        graph.AddVertex(3);
        Assert.False(graph.HasPath(1, 3));
    }

    [Fact]
    public void HasPath_SameVertex_ReturnsTrue()
    {
        var graph = new Graph<int>();
        graph.AddVertex(5);
        Assert.True(graph.HasPath(5, 5));
    }

    [Fact]
    public void HasPath_DirectedGraph_OnlyFollowsEdgeDirection()
    {
        var graph = new Graph<int>(directed: true);
        graph.AddEdge(1, 2);
        Assert.True(graph.HasPath(1, 2));
        Assert.False(graph.HasPath(2, 1));
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>Builds undirected graph: 1-2, 2-4, 1-3, 3-4.</summary>
    private static Graph<int> ConnectedSquare()
    {
        var graph = new Graph<int>();
        graph.AddEdge(1, 2);
        graph.AddEdge(1, 3);
        graph.AddEdge(2, 4);
        graph.AddEdge(3, 4);
        return graph;
    }
}
