# WaySprout — Frontend

React SPA for [WaySprout](../README.md), a job application tracker. Displays the list of job applications fetched from the API — no auth, no persistence beyond the backend's in-memory store yet.

## Stack

- React 19 + TypeScript
- Vite 8
- Tailwind CSS v4 (`@tailwindcss/vite`)
- shadcn/ui (Base UI primitives)
- TanStack Query
- React Router

## Getting Started

Prerequisites: Node 22+, pnpm enabled via Corepack (`corepack enable`).

```bash
pnpm install
pnpm dev
```

The dev server proxies `/api/*` to `http://localhost:5022` (see `vite.config.ts`) — run the backend (`dotnet run --project ../src/WaySprout.Web/`) for the app to show data.

## Project Structure

```
src/
  api/            — fetch clients for the backend API
  pages/          — route-level components
  components/ui/  — shadcn/ui components
  lib/            — shared utilities (cn(), etc.)
```

## Commands

| Command      | Description                   |
| ------------ | ----------------------------- |
| `pnpm dev`   | Start Vite dev server         |
| `pnpm build` | Type-check + production build |
| `pnpm lint`  | Lint with oxlint              |
| `pnpm format` | Format with Prettier          |

See the [root README](../README.md) and [CLAUDE.md](../CLAUDE.md) for the full project (backend, architecture, conventions).

---

## About this template

This project was scaffolded with `create-vite`'s React + TypeScript template — minimal setup for React in Vite with HMR and some Oxlint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) uses [Oxc](https://oxc.rs)
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) uses [SWC](https://swc.rs/)

### React Compiler

The React Compiler is not enabled on this template because of its impact on dev & build performance. To add it, see [this documentation](https://react.dev/learn/react-compiler/installation).

### Expanding the Oxlint configuration

If you are developing a production application, we recommend enabling type-aware lint rules by installing `oxlint-tsgolint` and editing `.oxlintrc.json`:

```json
{
  "$schema": "./node_modules/oxlint/configuration_schema.json",
  "plugins": ["react", "typescript", "oxc"],
  "options": {
    "typeAware": true
  },
  "rules": {
    "react/rules-of-hooks": "error",
    "react/only-export-components": ["warn", { "allowConstantExport": true }]
  }
}
```

See the [Oxlint rules documentation](https://oxc.rs/docs/guide/usage/linter/rules) for the full list of rules and categories.
