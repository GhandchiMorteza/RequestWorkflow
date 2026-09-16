# RequestWorkflow

ASP.NET Core Web API for a simple request approval workflow.

Employees create requests and the backend automatically routes each request to `Manager` or `Finance` based on its amount. Visibility and approval permissions are enforced on the server.

## Tech Stack

- .NET 10 / ASP.NET Core
- Entity Framework Core + PostgreSQL
- ASP.NET Core Identity + JWT
- Repository Pattern + Unit of Work
- xUnit
- Docker Compose

## Run

### Prerequisite

- Docker Desktop

From the repository root:

```bash
docker compose up --build
```

This command:

1. Starts PostgreSQL.
2. Applies EF Core migrations using a dedicated migration container.
3. Starts the API after migrations complete successfully.
4. Seeds the predefined roles and users in the Development environment.

API:

```text
http://localhost:8080
```

Scalar API reference:

```text
http://localhost:8080/scalar/v1
```

No .NET User Secrets or manual migration command is required when running with Docker.

To stop the project:

```bash
docker compose down
```

To also remove the database volume:

```bash
docker compose down -v
```

## Test Users

| Role | Email | Password |
| --- | --- | --- |
| Employee | `employee@example.com` | `Employee123` |
| Manager | `manager@example.com` | `Manager123` |
| Finance | `finance@example.com` | `Finance123` |

The three predefined users are the primary accounts for evaluating the workflow. Registration is also implemented; newly registered users are always assigned the `Employee` role.

## Request Routing

The approval threshold is configurable in `src/RequestWorkflow.Api/appsettings.json`:

```json
{
  "RequestRouting": {
    "ManagerApprovalMaxAmount": 1000000
  }
}
```

- `Amount <= 1,000,000` -> `Manager`
- `Amount > 1,000,000` -> `Finance`

## Main Endpoints

| Method | Endpoint | Access |
| --- | --- | --- |
| `POST` | `/api/auth/login` | Anonymous |
| `POST` | `/api/auth/register` | Anonymous |
| `POST` | `/api/requests` | Employee |
| `GET` | `/api/requests` | Authenticated |
| `POST` | `/api/requests/{id}/approve` | Assigned Manager / Finance |
| `POST` | `/api/requests/{id}/reject` | Assigned Manager / Finance |

## Important Implementation Decisions

- Request visibility and approve/reject authorization are enforced by the backend, not only by the UI.
- Extra fields from the dynamic JSON form, such as `urgency`, are stored in PostgreSQL as `jsonb` metadata so new form fields do not require a new database column.
- EF Core migrations run as a separate Docker Compose step instead of being executed from `Program.cs`, keeping database deployment concerns separate from application startup.
- The assignment-required Generic Repository and Unit of Work patterns are implemented. ASP.NET Core Identity is kept outside the generic repository abstraction.

## Tests

Routing behavior is covered for amounts below, equal to, and above the configured threshold.

If the .NET 10 SDK is installed locally:

```bash
dotnet test tests/RequestWorkflow.Application.Tests/RequestWorkflow.Application.Tests.csproj
```
