# CS Algorithms and Data Structures

A C# library of classic data structures with their algorithms implemented as methods on each structure.
Each topic is a separate .NET 9 class library with a matching xUnit test project.

**Design philosophy:** algorithms belong to data structures. Sorting is a method on a list. Traversal
is a method on a tree. BFS and DFS are methods on a graph. This keeps the relationship between data
and algorithm explicit, and keeps code at the right level of abstraction.

## Data Structures

### Lists — `Algorithms.Lists`

**`SinglyLinkedList<T>`** — a linked list where each node points forward to the next.

| Operation | Description | Time |
|-----------|-------------|------|
| `AddFirst(value)` | Prepend a value | O(1) |
| `AddLast(value)` | Append a value | O(n) |
| `Remove(value)` | Remove the first occurrence | O(n) |
| `Contains(value)` | Check whether a value exists | O(n) |
| `Reverse()` | Reverse the list in place | O(n) |
| `Sort()` | Sort ascending using insertion sort | O(n²) |

---

### Trees — `Algorithms.Trees`

**`BinarySearchTree<T>`** — a BST where every left child is smaller and every right child is larger
than its parent. Inserting in random order gives O(log n) average height; degenerate (sorted) input
degrades to O(n).

| Operation | Description | Time (avg) |
|-----------|-------------|------------|
| `Insert(value)` | Add a value; duplicates are ignored | O(log n) |
| `Contains(value)` | Check whether a value exists | O(log n) |
| `Remove(value)` | Delete a value; uses in-order successor for two-child nodes | O(log n) |
| `InOrder()` | Visit every element in ascending order | O(n) |
| `PreOrder()` | Visit root, then left subtree, then right subtree | O(n) |
| `PostOrder()` | Visit left subtree, then right subtree, then root | O(n) |
| `LevelOrder()` | Visit every element level by level (breadth-first) | O(n) |

---

### Graphs — `Algorithms.Graphs`

**`Graph<T>`** — an unweighted graph backed by an adjacency list. Works as either undirected
(default) or directed (pass `directed: true` to the constructor).

| Operation | Description | Time |
|-----------|-------------|------|
| `AddVertex(v)` | Add a vertex; no-op if it already exists | O(1) |
| `AddEdge(from, to)` | Add an edge; creates missing vertices automatically | O(1) |
| `HasVertex(v)` | Check whether a vertex exists | O(1) |
| `HasEdge(from, to)` | Check whether a directed edge exists | O(1) |
| `Neighbors(v)` | Return the set of directly reachable vertices | O(1) |
| `DepthFirstSearch(start)` | Visit reachable vertices depth-first (uses an explicit stack) | O(V+E) |
| `BreadthFirstSearch(start)` | Visit reachable vertices level by level | O(V+E) |
| `HasPath(from, to)` | Check whether any path exists between two vertices | O(V+E) |

---

## Build & Test

```bash
dotnet build                                              # Build everything
dotnet test                                               # Run all tests
dotnet test tests/<Topic>.Tests                           # Run one topic's tests
dotnet test --filter "FullyQualifiedName~BinarySearchTree" # Run tests for a specific class
```

## Contributing

This project uses the [Gitflow](https://nvie.com/posts/a-successful-git-branching-model/) branching strategy.

### Branches

| Branch | Purpose | Created from | Merges into |
|--------|---------|--------------|-------------|
| `main` | Production-ready, tagged releases | — | — |
| `develop` | Integration branch for completed work | `main` | — |
| `feature/<topic>` | New data structure or algorithm | `develop` | `develop` |
| `release/<version>` | Release preparation (version bumps, changelog) | `develop` | `main` + `develop` |
| `hotfix/<description>` | Urgent production fixes | `main` | `main` + `develop` |

### Workflow

```bash
# Start a feature
git checkout develop && git pull
git checkout -b feature/hash-tables

# Finish a feature — open a PR: feature/* → develop

# Cut a release
git checkout develop && git pull
git checkout -b release/1.2.0
# ... bump versions, update changelog ...
# Open PRs: release/* → main  and  release/* → develop

# Emergency hotfix
git checkout main && git pull
git checkout -b hotfix/fix-bst-remove
# ... fix ...
# Open PRs: hotfix/* → main  and  hotfix/* → develop
```

> **Enforced on GitHub:** `main` and `develop` require a pull request before merging.
> Direct pushes, force-pushes, and branch deletions are disabled on both branches.

## Development

See [CLAUDE.md](CLAUDE.md) for conventions, available agents, and how to add a new data structure topic.
