# Task Manager API

A small REST API for keeping track of to-do items. You can create a task, list every
task, fetch a single task by id, and mark a task as completed. Everything is stored
in PostgreSQL.

## Stack

- **.NET 10 / ASP.NET Core Web API** — the HTTP server
- **Dapper + Npgsql** — talking to the database (plain SQL, no ORM)
- **PostgreSQL 17** — the database, run in Docker via `docker-compose.yml`
- **DbUp** — applies the SQL scripts that create/update the database schema
- **Swashbuckle** — serves the Swagger UI so you can try the endpoints in a browser

## Project layout

There are two projects in this repo grouped together under `TaskManager.slnx`
(a solution file). You can still run each project from its own folder, or
from the root using `dotnet run --project ...` if you prefer.

- **`TaskManager/`** — the API itself.
  - `Controllers/` receives the HTTP requests
  - `Services/` holds the logic and turns errors into status codes
  - `Repository/` runs the SQL queries
  - `Models/` are the request/response shapes
  - `Database/Scripts/` the `.sql` files that define the table
- **`TaskManagerMigrator/`** — a small console app you run once to apply those
  `.sql` files to the database.

Requests flow: `Controller` → `Service` → `Repository` → PostgreSQL.

## Getting started

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download) and
[Docker](https://docs.docker.com/get-docker/). Run the commands below from the
folder they are shown in.

**1. Start the database** (from `TaskManager/`):

```bash
docker compose up -d
```

This starts a Postgres container on `localhost:5432` and keeps its data in a
Docker volume. Stop it later with `docker compose down`.

**2. Tell the app how to reach the database.**

The connection string is not stored in `appsettings.json` — it lives in .NET user
secrets, so it stays off the repo. Both projects share the same secrets id, so one
command from either project folder covers both:

```bash
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=task_manager;Username=postgres;Password=postgres"
```

Instead of that, you can set the environment variable `ConnectionStrings__Default`
to the same value for a single run. User secrets are only loaded in the
`Development` environment, which is what the default launch profile uses.

**3. Create the table** (from `TaskManagerMigrator/`):

```bash
dotnet run
```

It creates the `task_manager` database if needed and applies
`TaskManager/Database/Scripts/*.sql`. Safe to re-run — DbUp skips scripts that
have already been applied.

**4. Start the API** (from `TaskManager/`):

```bash
dotnet run
```

The server listens on <http://localhost:5105> and Swagger UI is at
<http://localhost:5105/swagger> (Development only).

## Example requests

Create a task:

```bash
curl -X POST http://localhost:5105/api/v1/tasks \
  -H "Content-Type: application/json" \
  -d '{"name":"Buy milk"}'
```

```json
{
  "success": "success",
  "data": {
    "id": "3f2b1c74-8a5e-4d19-9c77-2b0e5a1d6f30",
    "name": "buy milk",
    "completed": false,
    "createdAt": "2026-09-28T10:00:00Z",
    "updatedAt": null
  }
}
```

New task names are trimmed and lowercased before being stored, which is why
`"Buy milk"` comes back as `"buy milk"`.

List all tasks, oldest first:

```bash
curl http://localhost:5105/api/v1/tasks
```

Fetch one task:

```bash
curl http://localhost:5105/api/v1/tasks/3f2b1c74-8a5e-4d19-9c77-2b0e5a1d6f30
```

Mark a task as completed:

```bash
curl -X PATCH http://localhost:5105/api/v1/tasks/3f2b1c74-8a5e-4d19-9c77-2b0e5a1d6f30
```

Marking one as complete sets `completed` to `true` and stamps `updatedAt`.
