# CS Algorithms and Data Structures

C# library collection implementing classic algorithms and data structures. Each topic is a separate class library project with its own xUnit test project.

## Project Structure

```
AlgorithmsAndDataStructures.sln
Directory.Build.props          # Shared: net9.0, nullable, TreatWarningsAsErrors
src/
  <Topic>/                     # One classlib per topic (e.g. Sorting, Trees, Graphs)
    <Topic>.csproj
    <Algorithm>.cs
tests/
  <Topic>.Tests/               # One xUnit project per src project
    <Topic>.Tests.csproj
    <Algorithm>Tests.cs
```

## Adding a New Topic

```bash
# 1. Create the library
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

## Build & Test

```bash
dotnet build                                              # Build everything
dotnet test                                               # Run all tests
dotnet test tests/<Topic>.Tests                           # Run one topic
dotnet test --filter "FullyQualifiedName~QuickSort"       # Run specific test
```

## Conventions

### Namespaces

`Algorithms.<Topic>` — e.g. `Algorithms.Sorting`, `Algorithms.Trees`, `Algorithms.Graphs`

### Implementation Files

- One public class (or static class) per file, named after the algorithm or data structure
- All public members must have XML doc comments
- Document time and space complexity in `<remarks>`:

```csharp
/// <summary>Sorts an array in ascending order using quicksort.</summary>
/// <typeparam name="T">Element type; must implement <see cref="IComparable{T}"/>.</typeparam>
/// <param name="array">The array to sort in place.</param>
/// <remarks>
/// Time: O(n log n) average, O(n²) worst.
/// Space: O(log n) stack depth.
/// </remarks>
public static void Sort<T>(T[] array) where T : IComparable<T> { ... }
```

### Test Files

- One test class per implementation class; name: `<ClassName>Tests`
- Namespace: `Algorithms.<Topic>.Tests`
- Cover at minimum: empty input, single element, duplicates, pre-sorted, reverse-sorted
- Use `[Theory]` + `[InlineData]` or `[MemberData]` for parameterized cases
- No mocking; all tests are pure unit tests

## Agents

These agents are available for common tasks:

| Agent | When to use |
|-------|-------------|
| `algorithm-reviewer` | Review a new implementation for correctness, edge cases, and convention compliance |
| `test-generator` | Generate a comprehensive xUnit test suite for an implementation |
| `complexity-analyzer` | Analyze and document Big-O time/space complexity |

## Topics

| Topic | Namespace | Status |
|-------|-----------|--------|
| *(none yet)* | | |
