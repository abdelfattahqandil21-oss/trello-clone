# Project rules

## Stack
Angular 22 + Tailwind CSS v4 + TypeScript 6 + PrimeIcons v7

## Package manager
pnpm only. Never npm or yarn.

## Folder structure
src/
├── app/         — components, pages
├── environments/— dev/prod config
└── styles.css   — global styles (Tailwind + PrimeIcons)

## Hard rules
- No `any` — use `unknown` and narrow
- No barrel `index.ts` files
- No class-based state — signals only
- No inline styles — Tailwind classes only
- Commit message format: `type(scope): message` — types: feat/fix/refactor/chore/docs
- Always `inject()` — never constructor DI
- Always `input()/output()` — never `@Input/@Output`

## Do not re-discuss
- Standalone components (no NgModules)
- Zoneless Angular (default in v22)
- Signal forms (@angular/forms/signals)
- CSS only (no SCSS)
- No @angular/animations — use built-in animate
- One .gitignore at repo root (not per-project)
- Branch strategy: main only (no feature branches)
