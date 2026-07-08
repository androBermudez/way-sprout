# WaySprout

A job application tracker. Log every application you submit, track its progress through the hiring pipeline, and keep notes on interviews, contacts, and next steps.

## Tech Stack

- **Backend:** ASP.NET Core 10, C#, hexagonal architecture
- **Frontend:** React 19, TypeScript, Vite, React Router, TanStack Query, Tailwind CSS, shadcn/ui
- **Database:** In-memory (PostgreSQL planned)

## Getting Started

### Option 1: DevContainer (recommended)

Open the project in VS Code and select **"Reopen in Container"** when prompted. All dependencies install automatically.

### Option 2: Local setup

Prerequisites: [.NET 10 SDK](https://dotnet.microsoft.com/download), [Node.js 22+](https://nodejs.org). Enable pnpm via Corepack (bundled with Node): `corepack enable`.

```bash
# Install dependencies
dotnet restore
cd frontend && pnpm install && cd ..

# Run backend with hot reload (http://localhost:5022)
dotnet watch --project src/WaySprout.Web/ run

# Run frontend in a second terminal (http://localhost:5173)
cd frontend && pnpm dev
```

Open http://localhost:5173

## Commands

| Command                                         | Description                             |
| ----------------------------------------------- | ---------------------------------------- |
| `dotnet build`                                  | Build all projects                       |
| `dotnet test`                                   | Run all tests                            |
| `dotnet watch --project src/WaySprout.Web/ run` | Start API server with hot reload (dev)   |
| `dotnet run --project src/WaySprout.Web/`       | Start API server (one-off, no watching)  |
| `cd frontend && pnpm dev`                       | Start Vite dev server                    |
| `cd frontend && pnpm build`                     | Build React app for production           |
| `cd frontend && pnpm lint`                      | Lint TypeScript                          |

## Project Structure

```
src/
  WaySprout.Domain/        — Entities, value objects, domain errors
  WaySprout.Application/   — Use cases, port interfaces, DTOs
  WaySprout.Infrastructure/— Repository implementations
  WaySprout.Web/           — ASP.NET host, controllers, static files
frontend/                  — React app (Vite)
tests/
  WaySprout.Domain.Tests/
  WaySprout.Application.Tests/
  WaySprout.Infrastructure.Tests/
```

## API

Base URL: `http://localhost:5022/api/v1`

| Method | Path                 | Description                                                            |
| ------ | -------------------- | ----------------------------------------------------------------------- |
| GET    | `/applications`      | List applications — supports filtering and sorting via query params (see below) |
| GET    | `/applications/{id}` | Get one application                                                    |
| POST   | `/applications`      | Create new application                                                 |

### `GET /applications` query params

| Param          | Values                                                                                 | Description                                            |
| -------------- | --------------------------------------------------------------------------------------- | ------------------------------------------------------- |
| `q`            | any text                                                                                 | Matches against Company or Position (case-insensitive)  |
| `status`       | `Applied`, `Interviewing`, `Offer`, `Rejected`, `Withdrawn` (repeat the param for multiple) | Filters by status; omit for all statuses               |
| `appliedRange` | `Today`, `Last2Days`, `ThisWeek`, `Last7Days`, `ThisMonth`, `Last30Days`, `Last90Days`   | Filters by applied date, resolved server-side relative to "now" |
| `sortBy`       | `Company`, `Position`, `DateApplied`                                                     | Sort criterion; omit for unsorted (seed order)          |
| `direction`    | `Asc`, `Desc`                                                                            | Sort direction; defaults to `Asc` if `sortBy` is set     |

All enum-like values are case-insensitive. An unrecognized value for any param returns `400 Bad Request` as an RFC 7807 `application/problem+json` body, e.g. `{"errors":{"status":["Invalid status value: 'Bogus'."]}}`.

Example: `GET /api/v1/applications?q=engineer&status=Applied&status=Interviewing&appliedRange=Last30Days&sortBy=DateApplied&direction=Desc`

## OpenAPI / Swagger (not currently included)

`WaySprout.Web` does not generate an OpenAPI/Swagger document today. Nothing in
the project consumes one yet, so the dependency was removed to avoid an
unused package (it previously pulled in `Microsoft.OpenApi 2.0.0`, which has a
known high-severity vulnerability, [GHSA-v5pm-xwqc-g5wc](https://github.com/advisories/GHSA-v5pm-xwqc-g5wc)).

Add it back when you actually need it — for example, to serve Swagger UI, or
to generate TypeScript types for the frontend from the API shape. To do so:

1. Add the package (pin `Microsoft.OpenApi` directly too — `Microsoft.AspNetCore.OpenApi` keeps resolving it to the vulnerable `2.0.0` unless you override it):

   ```bash
   dotnet add src/WaySprout.Web/WaySprout.Web.csproj package Microsoft.AspNetCore.OpenApi
   dotnet add src/WaySprout.Web/WaySprout.Web.csproj package Microsoft.OpenApi --version 2.9.0
   ```

2. In `src/WaySprout.Web/Program.cs`, add:

   ```csharp
   builder.Services.AddOpenApi();
   ```

   right after `WebApplication.CreateBuilder(args)`, and:

   ```csharp
   if (app.Environment.IsDevelopment())
   {
       app.MapOpenApi();
   }
   ```

   right after `var app = builder.Build();`. This exposes the spec at `/openapi/v1.json` in development.

3. Run `dotnet list src/WaySprout.Web/WaySprout.Web.csproj package --vulnerable --include-transitive` afterward to confirm no vulnerable version was reintroduced.

## Contributing

Follow the conventions in `CLAUDE.md`.
