# CS Algorithms and Data Structures

C# library of classic data structures with their algorithms implemented as methods on each structure.
Each data structure topic is a separate class library project with its own xUnit test project.

## Core philosophy

Algorithms belong to data structures. Sorting is a method on a list. Traversal is a method on a tree.
BFS and DFS are methods on a graph. This keeps code at the right level of abstraction and makes the
relationship between data and algorithm explicit.

## Project Structure

```
AlgorithmsAndDataStructures.sln
Directory.Build.props          # Shared: net9.0, nullable, ImplicitUsings, TreatWarningsAsErrors
src/
  <Topic>/                     # One classlib per data structure topic (Lists, Trees, Graphs, …)
    <Topic>.csproj
    <DataStructure>.cs
tests/
  <Topic>.Tests/               # One xUnit project per src project
    <Topic>.Tests.csproj
    <DataStructure>Tests.cs
```

## Adding a New Topic

```bash
# 1. Create the class library
dotnet new classlib -n <Topic> -o src/<Topic> -f net9.0
dotnet sln AlgorithmsAndDataStructures.sln add src/<Topic>/<Topic>.csproj

# 2. Create the test project
dotnet new xunit -n <Topic>.Tests -o tests/<Topic>.Tests -f net9.0
dotnet add tests/<Topic>.Tests/<Topic>.Tests.csproj reference src/<Topic>/<Topic>.csproj
dotnet sln AlgorithmsAndDataStructures.sln add tests/<Topic>.Tests/<Topic>.Tests.csproj

# 3. Delete boilerplate
rm src/<Topic>/Class1.cs
rm tests/<Topic>.Tests/UnitTest1.cs
```

Or use the **`project-scaffolder`** agent — it runs all of the above and verifies the build.

## Build & Test

```bash
dotnet build                                              # Build everything
dotnet test                                               # Run all tests
dotnet test tests/<Topic>.Tests                           # Run one topic
dotnet test --filter "FullyQualifiedName~BinarySearchTree" # Run specific tests
```

## Conventions

### Namespaces

`Algorithms.<Topic>` — e.g. `Algorithms.Lists`, `Algorithms.Trees`, `Algorithms.Graphs`

### Implementation Files

- One public class per file; file name matches class name.
- Algorithms are **methods on the data structure**, not separate classes.
- All public members have XML doc comments.
- Document time and space complexity in `<remarks>` on the class and on each primary method.
- Use `where T : IComparable<T>` when the structure requires ordering; `where T : notnull` when it only requires hashability.

```csharp
/// <summary>A binary search tree maintaining the BST invariant.</summary>
/// <typeparam name="T">Element type; must implement <see cref="IComparable{T}"/>.</typeparam>
/// <remarks>
/// Time: Insert/Contains/Remove O(log n) average, O(n) worst (degenerate tree).
/// Space: O(n).
/// </remarks>
public sealed class BinarySearchTree<T> where T : IComparable<T>
{
    /// <summary>Inserts <paramref name="value"/> into the tree. Duplicates are ignored.</summary>
    public void Insert(T value) { ... }

    /// <summary>Visits every element in ascending order (left → root → right).</summary>
    public IEnumerable<T> InOrder() { ... }
}
```

### Test Files

- One test class per implementation class; name: `<ClassName>Tests`
- Namespace: `Algorithms.<Topic>.Tests`
- Cover at minimum: empty input, single element, duplicates, pre-sorted, reverse-sorted, large input (1000+)
- Use `[Theory]` + `[InlineData]` or `[MemberData]` for parameterized cases
- Use a `private static <Type> TypeOf(params ...)` helper to keep test bodies concise
- No mocking; all tests are pure unit tests

## Branching Strategy (Gitflow)

This repository follows [Gitflow](https://nvie.com/posts/a-successful-git-branching-model/).

| Branch | Purpose | Created from | Merges into |
|--------|---------|--------------|-------------|
| `main` | Production-ready, tagged releases | — | — |
| `develop` | Integration branch for completed work | `main` | — |
| `feature/<topic>` | New data structure or algorithm | `develop` | `develop` via PR |
| `release/<version>` | Release preparation (version bumps, changelog) | `develop` | `main` + `develop` via PR |
| `hotfix/<description>` | Urgent production fixes | `main` | `main` + `develop` via PR |

**Rules enforced on GitHub:**
- `main` and `develop` are protected — no direct pushes, no force-pushes, no deletions.
- All changes must arrive via a pull request.

**Branch naming convention:**
- `feature/` — new topic or algorithm (e.g. `feature/hash-tables`, `feature/bst-balance`)
- `release/` — semantic version (e.g. `release/1.2.0`)
- `hotfix/` — short description of the fix (e.g. `hotfix/fix-bst-remove`)

## Agents

| Agent | When to use |
|-------|-------------|
| `csharp-developer` | Write a new data structure or add an algorithm to an existing one |
| `doc-writer` | Update the Topics table in README/CLAUDE, or improve XML doc comments |
| `project-scaffolder` | Scaffold a new data structure topic (creates csproj, test project, wires into solution) |
| `algorithm-reviewer` | Review an implementation for correctness, edge cases, and convention compliance |
| `test-generator` | Generate a comprehensive xUnit test suite for an implementation |
| `complexity-analyzer` | Analyze and document Big-O time/space complexity |

## Hooks

| Trigger | Action |
|---------|--------|
| Edit/Write any `*.cs` (non-test) | `dotnet build` — catches compile errors immediately |
| Edit/Write any `*Tests.cs` | `dotnet test <project>` — runs the affected test project |

## Topics

| Topic | Namespace | Data Structures | Key Operations | Status |
|-------|-----------|-----------------|----------------|--------|
| Lists | `Algorithms.Lists` | `SinglyLinkedList<T>` | AddFirst, AddLast, Remove, Contains, Reverse, Sort | ✓ |
| Trees | `Algorithms.Trees` | `BinarySearchTree<T>` | Insert, Contains, Remove, InOrder, PreOrder, PostOrder, LevelOrder | ✓ |
| Graphs | `Algorithms.Graphs` | `Graph<T>` | AddVertex, AddEdge, HasPath, DepthFirstSearch, BreadthFirstSearch | ✓ |
