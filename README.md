# Payments Integration Service

## Local development

Prerequisites: .NET 10 SDK, Docker Engine or Docker Desktop, and the Docker Compose v2 plugin. On Linux, the current user must be able to access the Docker daemon.

From the repository root, run:

```bash
scripts/local-up.sh
```

The script trusts and exports the local ASP.NET HTTPS development certificate, builds and starts SQL Server Developer and Redis, builds and deploys the database DACPAC, and runs the API on **https://localhost:8081**. Press `Ctrl+C` to stop the API; the SQL Server and Redis containers remain available.

With the local dependencies and schema already running, the API can also be started directly:

```bash
dotnet run --project src/AcmeIntegreatedAssessement/AcmeIntegreatedAssessement.Api
```

To run the API itself in Docker instead of using `dotnet run`:

```bash
scripts/local-up.sh --compose
```

The app, SQL Server, and Redis ports bind to localhost. The default local SQL `sa` password is `YourStrong!Passw0rd`; set `SA_PASSWORD` in the shell before running the setup script to override it. Set `ACME_PAY_BASE_URL` to a reachable Acme Pay stub or sandbox URL before submitting payments. AWS Secrets Manager is disabled for local development.

Stop the containers with `docker compose down`. Remove local SQL and Redis data as well with `docker compose down --volumes`.
