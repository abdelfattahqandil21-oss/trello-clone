# Trello Clone

Monorepo containing **Angular 22** front-end and **.NET 8** back-end.

## Structure

```

trello-clone/
├── front-end/          # Angular 22 app (pnpm)
│   ├── src/            # App source code
│   ├── proxy.conf.json # Proxy /api to backend
│   └── package.json
├── back-end/           # .NET 8 Web API
│   ├── TrelloClone.Api/
│   │   ├── Program.cs
│   │   └── ...
│   └── TrelloClone.sln
├── package.json        # Root workspace scripts
└── pnpm-workspace.yaml
```

## Setup

### Prerequisites

- **Node.js** >= 22
- **pnpm** >= 10
- **.NET SDK** 8.0+

### Front-end

```bash
pnpm install
pnpm --filter front-end start
# → http://localhost:4200
```

### Back-end

```bash

dotnet restore back-end/TrelloClone.Api
dotnet run --project back-end/TrelloClone.Api
# → http://localhost:5000
```

### Or from root
```bash
pnpm front:dev    # starts Angular
pnpm back:dev     # starts .NET (requires dotnet CLI)
```

The Angular dev server proxies `/api/*` requests to `http://localhost:5000`.
