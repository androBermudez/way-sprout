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

| Project | Role |
|---|---|
| `WaySprout.Domain` | Entities, value objects, domain rules, `DomainException` |
| `WaySprout.Application` | Use cases, port interfaces (repositories), DTOs |
| `WaySprout.Infrastructure` | Repository implementations (in-memory → PostgreSQL) |
| `WaySprout.Web` | ASP.NET host, minimal API endpoints, DI wiring |
| `frontend/` | React 19, Vite, TanStack Query, Tailwind v4, shadcn/ui |

**API base URL:** `http://localhost:5022/api/v1`

## Domain Conventions

**Entities** use `private set` properties and a `private` constructor. Instantiation goes through a `static Create(...)` factory method that enforces invariants and throws `DomainException` on invalid input. Never use `ArgumentException` in domain code.

```csharp
public static JobApplication Create(Guid userId, string company, ...) { ... }
```

**Value objects** use `record` for structural equality. **Entities** use `class` with identity-based equality (by `Id`).

**Enums** live in `WaySprout.Domain/Enums/`. Extension methods on enums (e.g., `IsFinal()`) go in the same file as a `static` class.

## Test Conventions

xUnit. Test method naming: `Subject_Scenario_ExpectedResult`.

```csharp
[Fact]
public void Create_EmptyCompany_Throws() { ... }

[Theory]
[InlineData(ApplicationStatus.Rejected)]
public void IsFinal_FinalStatuses_ReturnsTrue(ApplicationStatus status) { ... }
```

Each test class defines a private `static` helper that returns a valid default instance (`Valid()`) to keep test setup concise. Tests use `DomainException` (not `ArgumentException`) when asserting throws on domain rules.

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

| Prefix | Use |
|---|---|
| `feature/` | New functionality |
| `fix/` | Bug fixes |
| `chore/` | Maintenance — deps, config, tooling; no behavior change |
| `refactor/` | Code restructuring without changing behavior |
| `docs/` | Documentation only (README, CLAUDE.md) |
| `test/` | Adding or fixing tests only |

Examples: `feature/add-status-filter`, `fix/salary-expectation-negative-amount`, `chore/bump-microsoft-openapi`.

Before merging, verify:

- [ ] `dotnet build` — 0 warnings, 0 errors.
- [ ] `dotnet test` — all green.
- [ ] `pnpm build` — if the feature touches `frontend/`, type-checks and builds clean.
- [ ] `pnpm lint` — if the feature touches `frontend/`, oxlint clean.
- [ ] `dotnet list package --vulnerable --include-transitive` — no new vulnerable packages introduced.
- [ ] Domain Conventions followed (`DomainException`, not `ArgumentException`; `private set` + `static Create()`; `record` vs `class`).
- [ ] Dependency direction respected: `Web → Application → Domain`, `Infrastructure → Application` — no new reference violates it.
- [ ] `README.md` updated, including the `## API` table if the feature adds or changes an endpoint.
- [ ] `CLAUDE.md` updated if the feature introduces a new convention or tech decision.
