# Payments Integration Service

## Quick Start

1. **Clone repository**  
   ```bash
   git clone https://github.com/DeveloperJohn-SouthAfrica/Zapper.TeamLead.Assignment.git
   cd Zapper.TeamLead.Assignment
   ```

2. **Start local infrastructure and API**  
   ```bash
   scripts/local-up.sh
   ```

3. **Access services**  
   - API: https://localhost:8081  
   - Blazor client: http://localhost:5000  

4. **Run tests**  
   ```bash
   dotnet test
   ```

For detailed local setup, see the **Local development** section below.

## Documentation

- [Solution overview](Solution.md)  
- [Analysis artifacts](docs/analysis/structured-systems-analysis.md)  
- [Design artifacts](docs/design/README.md)  
- [Specification](docs/specification.md)  
- [Setup instructions](docs/setup.md)  
- [Soul file](docs/soul.md)  
- [High-Level Architecture diagrams](docs/design/high-level-architecture.md)  
- [AI workflow artifacts](ai/)  

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
