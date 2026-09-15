# RequestWorkflow

ASP.NET Core backend for a simple request approval workflow.

## Features

* JWT authentication
* Employee request creation
* Manager / Finance routing based on amount
* Role-based request visibility
* Approve / Reject workflow
* PostgreSQL + EF Core
* Docker Compose

## Run

```bash
docker compose up -d
dotnet ef database update --project src/RequestWorkflow.Infrastructure --startup-project src/RequestWorkflow.Api
dotnet run --project src/RequestWorkflow.Api
```

Secrets are configured via .NET User Secrets.

API flows have been manually tested using `RequestWorkflow.Api.http`.
