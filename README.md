# BTR - Billiard Tournament Register

## Objective
BTR is an MVP web application to manage billiard tournaments and player registrations.

The app allows users to:
- Register and log in
- Create and list tournaments
- Create, edit, list, and delete tournament registrations

## Architecture
This project follows a clean layered architecture with separated backend and frontend.

### Backend (.NET 8)
- `Btr.Api`: HTTP API (controllers, auth, Swagger, health checks)
- `Btr.Application`: use cases and business services
- `Btr.Domain`: core entities and domain rules
- `Btr.Infrastructure`: EF Core persistence, repositories, security infrastructure
- `Btr.UnitTests` and `Btr.IntegrationTests`: test projects

### Frontend (Angular 20)
- Standalone Angular application in `src/frontend/btr-web`
- Feature-based UI structure (auth, registrations, tournaments)
- JWT auth guard + interceptor
- Reactive forms and typed API service wrapper

## Technologies
- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core 8
- SQLite (local database)
- JWT Bearer Authentication
- Angular 20
- TypeScript + RxJS

## Run the project

### Prerequisites
- .NET SDK 8
- Node.js 20+ and npm

### 1) Run backend API
From `src/backend`:

```bash
dotnet restore Btr.slnx
dotnet run --project Btr.Api/Btr.Api.csproj --urls http://localhost:5000
```

Useful endpoints:
- Swagger: `http://localhost:5000/swagger`
- Health: `http://localhost:5000/health`

### 2) Run frontend
From `src/frontend/btr-web`:

```bash
npm install
npm start
```

Frontend URL:
- `http://localhost:4200`

## Build and test

### Backend
From `src/backend`:

```bash
dotnet build Btr.slnx
dotnet test Btr.slnx
```

### Frontend
From `src/frontend/btr-web`:

```bash
npm run build
```

## Notes
- The backend applies EF Core migrations automatically on startup (except in Testing environment).
- Default local database file is `btr.local.db`.

## AI Prompt (Task Management API)
See [docs/task-management-api.prompt.md](docs/task-management-api.prompt.md).

## Design Process
See [docs/design-process.md](docs/design-process.md).
