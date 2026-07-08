# LINQ Guidelines

LINQ is the default tool for data pipelines (filtering, projecting, grouping, sorting) in this codebase — prefer it over hand-rolled loops when it reads at least as clearly. But `IEnumerable<T>` is not a lazy sequence with language-level purity guarantees: its laziness is an implementation detail with sharp edges, not something to lean on. Know these before chaining.

## Deferred execution runs the pipeline on every enumeration, not once

A query built with `Where`/`Select`/etc. doesn't run until something enumerates it (`foreach`, `ToList()`, `Count()`, ...). If you enumerate the same unmaterialized query twice, the whole pipeline — including any side effects — runs twice.

```csharp
// Bug: Where runs once for the Count() and again for the foreach — twice, against
// whatever the source looks like at each point in time.
var pending = applications.Where(a => a.Status == ApplicationStatus.Applied);
var count = pending.Count();
foreach (var a in pending) { /* ... */ }
```

Materialize once with `.ToList()`/`.ToArray()` when a result will be reused, counted, or iterated more than once.

## A materialized result is a snapshot; an unmaterialized query is not

`Repository.GetAllAsync()` already returns `IReadOnlyList<T>` (materialized), so this is less of a trap here than it would be over an `IQueryable<T>` or an in-place-mutated collection — but if a LINQ pipeline is ever built directly over a mutable field or an EF `IQueryable`, re-enumerating it can observe different data than the first pass, since nothing was actually computed yet.

## Exceptions surface at enumeration time, not at the point the query is written

A `Select(a => 1 / a.SomeCount)` throws when a `foreach`/`ToList()` pulls the failing element, not on the line where `.Select` was called — makes stack traces and step-through debugging less intuitive than a plain loop.

## Ordering isn't implied by anything except `OrderBy`/`OrderByDescending`

These two operators *are* guaranteed stable in .NET. `Where`/`Select`/`GroupBy` preserve input order but don't impose one — don't assume a `GroupBy` result is sorted.

## A custom `yield return` iterator method defers *everything* in its body, including argument validation

An `ArgumentNullException.ThrowIfNull(x)` at the top of a method that also `yield return`s doesn't run when the method is called — it runs on the first `MoveNext()`, i.e. whenever a caller starts enumerating. Surprising even for experienced C# devs; if a method needs to validate eagerly, split validation into a non-iterator wrapper that calls a private iterator method.

```csharp
// Bug: ThrowIfNull doesn't run until the caller enumerates, not when GetActive is called.
public IEnumerable<JobApplication> GetActive(IReadOnlyList<JobApplication> apps)
{
  ArgumentNullException.ThrowIfNull(apps);
  foreach (var a in apps)
  {
    if (a.Status == ApplicationStatus.Applied)
    {
      yield return a;
    }
  }
}

// Fix: validate eagerly in a non-iterator wrapper; keep the deferred part private.
public IEnumerable<JobApplication> GetActive(IReadOnlyList<JobApplication> apps)
{
  ArgumentNullException.ThrowIfNull(apps);
  return GetActiveCore(apps);
}

private static IEnumerable<JobApplication> GetActiveCore(IReadOnlyList<JobApplication> apps)
{
  foreach (var a in apps)
  {
    if (a.Status == ApplicationStatus.Applied)
    {
      yield return a;
    }
  }
}
```

## This matters more once real persistence (EF/PostgreSQL) lands

Re-enumerating an unmaterialized `IQueryable<T>` doesn't just repeat in-memory work — it re-executes the query against the database, or triggers N+1 query patterns. The "return `IReadOnlyList<T>`, not `IEnumerable<T>`, from ports" rule (see `CLAUDE.md`) exists partly to keep that risk out of `Infrastructure` entirely: repositories materialize once, before returning.

## `CA1851` catches the common case at build time

The Roslyn analyzer `CA1851` (Performance) flags enumerating the same unmaterialized `IEnumerable<T>` more than once in a method. Enabled as a warning in `.editorconfig`. It's data-flow heuristics, not a type-level guarantee — it won't catch every path (e.g. a value stashed in a field and reused later), so the conventions above still matter; treat it as a safety net, not a substitute for them.
