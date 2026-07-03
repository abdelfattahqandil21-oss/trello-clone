# Project context

## Last updated
July 2026

## What this project does
Trello clone monorepo with Angular 22 front-end and .NET 10 back-end.

## Current focus
Implementing backend models/APIs based on ERD schema (User, WorkSpace, Board, List, Card, Label, CheckList, Comment).

## Decisions made — do not re-discuss
- pnpm workspace root + front-end
- SQLite for local dev, EF Core + Identity
- Tailwind v4 + PostCSS (no config file needed)
- Port 5000 for backend, 4200 for front-end
- Angular standalone, zoneless, signals, signal forms

## Tech debt — known issues
- No authentication UI yet (Identity scaffolded in backend)
- Areas folders are empty (Admin, Chat, Customer, Identity)
- Sample WeatherForecast endpoint still in backend (to be removed)

## Non-obvious file map
- `back-end/TrelloClone/` — .NET Web API project (not nested)
- `.opencode/skills/angular/angularCdk.md` — CDK drag-drop & overlay skill
- `Trello_Clone_FIXED.erdplus.txt` — ERD source for the schema
