namespace Algorithms.Graphs;

/// <summary>
/// An unweighted graph backed by an adjacency list. Supports both undirected and directed modes.
/// </summary>
/// <typeparam name="T">The vertex type; must be non-nullable and support equality comparison.</typeparam>
/// <remarks>
/// Time:  AddVertex O(1), AddEdge O(1), HasEdge O(1),
///        BreadthFirstSearch O(V+E), DepthFirstSearch O(V+E), HasPath O(V+E).
/// Space: O(V+E) for the adjacency list.
/// </remarks>
public sealed class Graph<T> where T : notnull
{
    private readonly Dictionary<T, HashSet<T>> _adjacency = new();
    private readonly bool _directed;

    /// <summary>Initializes a new graph.</summary>
    /// <param name="directed">
    /// When <c>true</c>, each edge goes one way only.
    /// When <c>false</c> (the default), adding an edge A→B also adds B→A.
    /// </param>
    public Graph(bool directed = false) => _directed = directed;

    /// <summary>Gets the number of vertices in the graph.</summary>
    public int VertexCount => _adjacency.Count;

    /// <summary>Gets the number of directed edges (or undirected edge pairs when not directed).</summary>
    public int EdgeCount { get; private set; }

    /// <summary>
    /// Adds <paramref name="vertex"/> to the graph.
    /// Has no effect if the vertex already exists.
    /// </summary>
    public void AddVertex(T vertex) => _adjacency.TryAdd(vertex, new HashSet<T>());

    /// <summary>
    /// Adds an edge from <paramref name="from"/> to <paramref name="to"/>,
    /// creating both vertices automatically if they do not yet exist.
    /// Adding a duplicate edge has no effect.
    /// </summary>
    public void AddEdge(T from, T to)
    {
        AddVertex(from);
        AddVertex(to);

        if (_adjacency[from].Add(to))
            EdgeCount++;

        if (!_directed)
            _adjacency[to].Add(from);
    }

    /// <summary>Returns <c>true</c> if the graph contains <paramref name="vertex"/>.</summary>
    public bool HasVertex(T vertex) => _adjacency.ContainsKey(vertex);

    /// <summary>
    /// Returns <c>true</c> if there is a directed edge from <paramref name="from"/> to <paramref name="to"/>.
    /// </summary>
    public bool HasEdge(T from, T to) =>
        _adjacency.TryGetValue(from, out var neighbors) && neighbors.Contains(to);

    /// <summary>Returns the set of vertices directly reachable from <paramref name="vertex"/>.</summary>
    /// <exception cref="KeyNotFoundException">Thrown when <paramref name="vertex"/> is not in the graph.</exception>
    public IReadOnlySet<T> Neighbors(T vertex) => _adjacency[vertex];

    /// <summary>
    /// Visits all vertices reachable from <paramref name="start"/> in depth-first order,
    /// using an explicit stack to avoid recursion.
    /// </summary>
    /// <remarks>Time: O(V+E). Space: O(V) for the visited set and stack.</remarks>
    public IEnumerable<T> DepthFirstSearch(T start)
    {
        if (!_adjacency.ContainsKey(start)) yield break;

        var visited = new HashSet<T>();
        var stack = new Stack<T>();
        stack.Push(start);

        while (stack.Count > 0)
        {
            var vertex = stack.Pop();
            if (!visited.Add(vertex)) continue;

            yield return vertex;

            foreach (var neighbor in _adjacency[vertex])
                if (!visited.Contains(neighbor))
                    stack.Push(neighbor);
        }
    }

    /// <summary>
    /// Visits all vertices reachable from <paramref name="start"/> level by level (breadth-first).
    /// </summary>
    /// <remarks>Time: O(V+E). Space: O(V) for the visited set and queue.</remarks>
    public IEnumerable<T> BreadthFirstSearch(T start)
    {
        if (!_adjacency.ContainsKey(start)) yield break;

        var visited = new HashSet<T> { start };
        var queue = new Queue<T>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var vertex = queue.Dequeue();
            yield return vertex;

            foreach (var neighbor in _adjacency[vertex])
                if (visited.Add(neighbor))
                    queue.Enqueue(neighbor);
        }
    }

    /// <summary>
    /// Returns <c>true</c> if any path exists from <paramref name="from"/> to <paramref name="to"/>.
    /// </summary>
    /// <remarks>Time: O(V+E). Space: O(V).</remarks>
    public bool HasPath(T from, T to) => BreadthFirstSearch(from).Contains(to);
}
