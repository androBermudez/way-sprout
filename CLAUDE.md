# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build

# Run all tests
dotnet test

# Run a single test class
dotnet test --filter "FullyQualifiedName~JobApplicationTests"

# Run a single test method
dotnet test --filter "FullyQualifiedName~JobApplicationTests.Create_SetsAllFields"
```

```bash
dotnet watch --project src/WaySprout.Web/ run   # API with hot reload on http://localhost:5022
dotnet run --project src/WaySprout.Web/         # API, one-off (no watching)
```

If `dotnet watch` doesn't pick up changes inside the devcontainer (bind-mounted volumes sometimes don't propagate file events), force polling:

```bash
DOTNET_USE_POLLING_FILE_WATCHER=1 dotnet watch --project src/WaySprout.Web/ run
```

When the frontend project exists:

```bash
cd frontend && pnpm dev                      # Vite on http://localhost:5173
cd frontend && pnpm lint
```

## Architecture

Hexagonal architecture (ports & adapters). The dependency rule flows inward:

```
Web → Application → Domain
Infrastructure → Application
```

| Project                    | Role                                                     |
| --------------------------- | -------------------------------------------------------- |
| `WaySprout.Domain`         | Entities, value objects, domain rules, `DomainException` |
| `WaySprout.Application`    | Use cases, port interfaces (repositories), DTOs          |
| `WaySprout.Infrastructure` | Repository implementations (in-memory → PostgreSQL)      |
| `WaySprout.Web`            | ASP.NET host, minimal API endpoints, DI wiring           |
| `frontend/`                | React 19, Vite, TanStack Query, Tailwind v4, shadcn/ui   |

**Exception to the diagram:** `Program.cs` (the composition root) also references `Infrastructure` directly, to register concrete adapters against `Application` ports. That's the only place this is allowed — see Web Conventions.

**API base URL:** `http://localhost:5022/api/v1`

## Domain Conventions

**Entities** use `private set` properties and a `private` constructor. Instantiation goes through a `static Create(...)` factory method that enforces invariants and throws `DomainException` on invalid input. Never use `ArgumentException` in domain code.

```csharp
public static JobApplication Create(Guid userId, string company, ...) { ... }
```

**Value objects** use `record` for structural equality. **Entities** use `class` with identity-based equality (by `Id`).

**Enums that model a domain concept** (`ApplicationStatus`, `Currency`) live in `WaySprout.Domain/Enums/`. Extension methods on them (e.g., `IsFinal()`) go in the same file as a `static` class. Enums that only shape a use case's input, not a domain concept (e.g. `DateRangePreset`, `SortDirection`), don't belong here — see Application Conventions.

## Application Conventions

**Use cases** live under `UseCases/<Feature>/`, one folder per use case:

| File               | Purpose                                                                                                          |
| ------------------- | ------------------------------------------------------------------------------------------------------------------ |
| `<Feature>Query.cs`  | Use case input, with unresolved concerns (e.g. a date-range preset). List use cases only.                        |
| `<Feature>Filter.cs` | Resolved version of the query — concrete values only (e.g. `DateOnly` bounds instead of a preset). What the repository actually receives. List use cases only. |
| `<Feature>Handler.cs` | Orchestrates: resolves the query into a filter, calls the repository, maps the result to a DTO.                 |
| `*Dto.cs`            | One per shape returned. Named after what it represents, not the use case that produces it — a DTO is a data shape, not an action (`JobApplicationDto`, `JobApplicationDetailsDto`, not `GetJobApplicationByIdDto`). |

`GetJobApplications` (list, with the Query/Filter split) and `GetJobApplicationById` (single lookup, no split needed) are the two reference shapes — model new use cases after whichever one matches.

**Enums that only shape a use case's input**, not a domain concept, live in `Application/Enums/` (e.g. `DateRangePreset`, `SortDirection`, `JobApplicationSortCriteria`).

**`Application/Services/`** holds logic reused across use cases (e.g. `DateRangePresetResolver`). Types used by only one use case stay colocated in that use case's folder under `UseCases/`.

**Testable "now":** inject `TimeProvider` (BCL, no package needed) wherever code needs the current date/time — never call `DateTime.Now`/`TimeProvider.System` directly outside of DI registration in `Program.cs`. Test with a minimal `TimeProvider` subclass overriding `GetUtcNow()`, not a mocking library.

**DI lifetimes:** stateless services with no per-request mutable state (`TimeProvider`, `DateRangePresetResolver`) → `Singleton`. Handlers, repositories, and anything that will eventually hold a `DbContext`/unit-of-work → `Scoped`. This matters beyond style: a future EF-backed repository registered as `Singleton` is a real concurrency bug, not a nitpick — get the lifetime right from the start so swapping the in-memory repository for a real one doesn't require an audit.

## Web Conventions

**`Program.cs` is the composition root** — the one file allowed to reference `WaySprout.Infrastructure` directly, to register concrete adapters against `Application` ports. Endpoints themselves only depend on `Application` types (handlers, queries, DTOs), never on `Infrastructure`.

**URL naming:** path segments and query parameter *names* use `kebab-case` (e.g. `applied-range`, `sort-by`) — the convention most REST APIs converge on for multi-word URL parts, kept independent from JSON body casing. This only affects the wire name; C# parameter/variable names stay idiomatic camelCase. When a param's kebab-case wire name differs from its C# identifier, bind it explicitly with `[FromQuery(Name = "...")]` (see `appliedRange`/`sortBy` in `Program.cs`). Query parameter *values* are unaffected by this rule — they keep whatever casing the type expects (enum values here are case-insensitive and may be `camelCase`).

**Query-string parsing:** enum-valued query params (`status`, `applied-range` → `appliedRange`, `sort-by` → `sortBy`, `direction`) go through the local `TryParseEnum<TEnum>` helper already defined in `Program.cs` — absent/empty parses to `null` successfully, an unrecognized value fails. Reuse it for new enum-valued params instead of hand-rolling `Enum.TryParse` per param.

**Error responses:** validation failures (a bad query param) return `Results.ValidationProblem(...)` — RFC 7807 `application/problem+json`, enabled via `builder.Services.AddProblemDetails()` in `Program.cs`. "Not found" returns a plain `Results.NotFound()` (empty body — there's nothing more to say than the status code). Don't return a bare string from `Results.BadRequest(string)`; it isn't structured and a client can't parse it programmatically.

## Test Conventions

xUnit. Test method naming: `Subject_Scenario_ExpectedResult`.

```csharp
[Fact]
public void Create_EmptyCompany_Throws() { ... }

[Theory]
[InlineData(ApplicationStatus.Rejected)]
public void IsFinal_FinalStatuses_ReturnsTrue(ApplicationStatus status) { ... }
```

- **`WaySprout.Domain.Tests`:** each test class defines a private `static Valid()` helper returning a valid default instance, to keep setup concise. Asserts `DomainException` (not `ArgumentException`) on domain rule violations.
- **`WaySprout.Application.Tests`:** handler tests use a hand-rolled fake repository (e.g. `FakeJobApplicationRepository`) with a spy property (e.g. `LastFilter`) to assert on what the handler passed downstream, plus a minimal `TimeProvider` subclass for a fixed "now". No mocking library.
- **`WaySprout.Infrastructure.Tests`:** repository tests run against the real seed data and assert on filter/sort behavior end-to-end.
- **Frontend:** no test tooling set up yet (no Vitest/RTL in `frontend/package.json`). Add it once component logic gets non-trivial enough to justify it — an open decision, not an oversight.

## Code Style

### C#

- **Braces:** always use `{}` for `if`/`else`/loops, even for single-statement bodies. Enforced at build time (`EnforceCodeStyleInBuild` in `Directory.Build.props` + `csharp_prefer_braces` in `.editorconfig`) — a missing brace is a build warning, not just an IDE suggestion.

  ```csharp
  if (application is null)
  {
    return NotFound();
  }
  ```

- **Indentation:** 2 spaces, formalized in `.editorconfig` (not the 4-space .NET default — this project has used 2 spaces consistently since the initial scaffold).
- **Namespaces:** file-scoped (`namespace WaySprout.Domain.Entities;`), not block-scoped.
- **Primary constructors** for simple dependency injection (e.g., `GetJobApplicationsHandler(IJobApplicationRepository repository)`).
- **`var`** when the type is obvious from the right-hand side.
- **`Async` suffix** on every method that does I/O or returns a `Task`/`Task<T>` (`HandleAsync`, `GetByIdAsync`, `GetAllAsync`) — no exceptions.
- **Return types — collections vs. a single optional value.** These are two different questions with two different answers; don't reach for the same tool for both.
  - **Collections:** return `IReadOnlyList<T>` (or `IReadOnlyCollection<T>` if only count/membership matters, not order) from any public method or port — never `IEnumerable<T>`, `List<T>`, or an array. `IEnumerable<T>` in a return type hides from the caller whether they're getting a materialized snapshot or a live, re-runnable query (see the LINQ notes below). `List<T>` over-promises mutability: the caller might mutate a list that's actually backing internal state. "No results" for a collection is an **empty list, never `null`**.
  - **A single value that may or may not exist:** use a nullable reference type (`T?`), not exceptions or sentinel values. Reserve exceptions (`DomainException`) for actual domain rule violations, not for "not found." We deliberately don't use an `Option`/`Maybe`-style wrapper type here: `T?` gets real compiler-enforced analysis under `#nullable enable` (warnings on unchecked dereference), composes fine with the pattern-matching already used in this codebase (`is { } x`, `??`, `?.`), and stays native to the wider .NET ecosystem (model binding, EF, serialization) instead of requiring adapters at every boundary. The trade-off: it doesn't chain through transformations as fluently as a wrapper type would — accepted for now; revisit only if a real need for "absent vs. failed-with-a-reason" shows up, which is a different problem (`Result`/`Either`-shaped, not `Option`-shaped).
- **Prefer functional style over imperative code whenever it makes the result more expressive and readable** — favor pure functions, immutable values, and composition (`Func<...>`/`Action<...>`, LINQ pipelines, switch expressions, pattern matching) over mutable state and imperative control flow. This is a readability call, not a purity mandate: reach for it when it clarifies intent, not as an exercise in avoiding loops.

  One recurring shape of this: when multiple branches of a condition differ only in _what_ value/function they use, extract that difference as a value once, then apply the shared operation once — instead of repeating the full operation inside every branch.

- **LINQ is the default tool for data pipelines**, preferred over hand-rolled loops when it reads at least as clearly — but its deferred execution has real gotchas (multiple enumeration, exceptions surfacing late, `yield return` deferring even argument validation). `CA1851` is enabled in `.editorconfig` to catch the common case at build time. Full writeup with examples: [`docs/linq-guidelines.md`](docs/linq-guidelines.md) — read it before writing a non-trivial LINQ chain, not just when something breaks.
- **Avoid XML doc comments (`///`) that just restate the signature** — not a blanket ban on comments. If a descriptive name (`Create`, `HandleAsync`, `GetByIdAsync`) plus its parameter/return types already tell the reader what it does, a `///` on top of that is redundant. Write one when there's a non-obvious constraint, invariant, or side effect that the signature can't express on its own.

### Frontend (TypeScript/TSX)

- **Formatting:** Prettier on save (`editor.formatOnSave` + `esbenp.prettier-vscode`, configured in `.devcontainer/devcontainer.json`). Config lives in `frontend/.prettierrc`, scoped to `frontend/` only — it doesn't affect `.NET` or root-level files. `"semi": false`: semicolons are omitted where safe, kept only where required to avoid ambiguous parses. `"printWidth": 100`. Run `pnpm format` to reformat the whole `frontend/` tree on demand, outside of format-on-save.
- **Components:** function declarations (`export function ApplicationsPage() { ... }`), not arrow-function consts.
- **Functions inside a component body** (event handlers, small helpers scoped to that component): arrow-function consts (`const handleSort = (column: SortCriteriaValue) => { ... }`), not `function` declarations. This is the inverse of the rule above — it only applies to functions nested inside a component, not to the component itself or to standalone top-level functions/components in the same file.
- **Exports:** named exports everywhere, except `src/App.tsx` (default export, matching the Vite template's entry-point convention). Enforced via oxlint's `import/no-default-export` rule, with an override allowing it in `App.tsx` (`frontend/.oxlintrc.json`).
- **Import order:** third-party packages first, then a blank line, then internal `@/` imports.

  ```ts
  import { useQuery } from "@tanstack/react-query"

  import { getJobApplications } from "@/api/jobApplication"
  ```

  Auto-enforced on save by Prettier via the `@ianvs/prettier-plugin-sort-imports` plugin (config in `frontend/.prettierrc`) — oxlint's `import` plugin doesn't have an `import/order`-equivalent rule, so this lives in Prettier instead. Also alphabetizes named specifiers within a single import statement.

## Tech Notes

- Target: `net10.0`. `ImplicitUsings` and `Nullable` are enabled on all projects.
- Solution file: `WaySprout.slnx` (newer XML format — use `dotnet` CLI, not older tooling that may not support it).
- Tailwind v4 uses `@import "tailwindcss"` — no `tailwind.config.ts`.
- Linting uses `oxlint` (the current Vite React-TS template default) — not ESLint. Run via `pnpm lint`; no config file needed.
- Vite proxy points to `localhost:5022` (not 5000).
- Frontend package manager is `pnpm` (via Corepack), not `npm` — use `pnpm`/`pnpm dlx` in place of `npm`/`npx` for anything under `frontend/`.

## Feature Workflow

Every new feature starts on its own branch — never commit directly to `master`.

**Branch naming:** `<type>/kebab-case-description`, aligned with Conventional Commits types:

| Prefix      | Use                                                     |
| ----------- | ------------------------------------------------------- |
| `feature/`  | New functionality                                       |
| `fix/`      | Bug fixes                                               |
| `chore/`    | Maintenance — deps, config, tooling; no behavior change |
| `refactor/` | Code restructuring without changing behavior            |
| `docs/`     | Documentation only (README, CLAUDE.md)                  |
| `test/`     | Adding or fixing tests only                             |

Examples: `feature/add-status-filter`, `fix/salary-expectation-negative-amount`, `chore/bump-microsoft-openapi`.

Before merging, verify:

- [ ] `dotnet build` — 0 warnings, 0 errors.
- [ ] `dotnet test` — all green.
- [ ] `pnpm build` — if the feature touches `frontend/`, type-checks and builds clean.
- [ ] `pnpm lint` — if the feature touches `frontend/`, oxlint clean.
- [ ] `pnpm format` — if the feature touches `frontend/`, Prettier applied (no unformatted diffs left).
- [ ] `dotnet list package --vulnerable --include-transitive` — no new vulnerable packages introduced.
- [ ] Domain Conventions followed (`DomainException`, not `ArgumentException`; `private set` + `static Create()`; `record` vs `class`).
- [ ] Dependency direction respected: `Web → Application → Domain`, `Infrastructure → Application` — no new reference violates it.
- [ ] `README.md` updated, including the `## API` table if the feature adds or changes an endpoint.
- [ ] `CLAUDE.md` updated if the feature introduces a new convention or tech decision.
