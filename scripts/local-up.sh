#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.." && pwd)"
API_PROJECT="$ROOT_DIR/src/AcmeIntegreatedAssessement/AcmeIntegreatedAssessement.Api/AcmeIntegreatedAssessement.Api.csproj"
DATABASE_PROJECT="$ROOT_DIR/src/AcmeIntegreatedAssessement/AcmeIntegreatedAssessement.Database/AcmeIntegreatedAssessement.Database.sqlproj"
COMPOSE_FILE="$ROOT_DIR/docker-compose.yml"
RUN_IN_COMPOSE=false

if [[ "${1:-}" == "--compose" ]]; then
    RUN_IN_COMPOSE=true
elif [[ $# -gt 0 ]]; then
    echo "Usage: $0 [--compose]" >&2
    exit 2
fi

for command in docker dotnet curl; do
    if ! command -v "$command" >/dev/null 2>&1; then
        echo "Required command not found: $command" >&2
        exit 1
    fi
done

if ! docker compose version >/dev/null 2>&1; then
    echo "Docker Compose v2 is required. Install the Docker Compose plugin and ensure this user can access the Docker daemon." >&2
    exit 1
fi

export SA_PASSWORD="${SA_PASSWORD:-YourStrong!Passw0rd}"
export DEV_CERT_PASSWORD="${DEV_CERT_PASSWORD:-LocalDevOnly!ChangeMe}"
export SQL_PORT="${SQL_PORT:-1433}"
export REDIS_PORT="${REDIS_PORT:-6379}"
export TARGET_DATABASE="${TARGET_DATABASE:-Payments}"
export ACME_STUB_PORT="${ACME_STUB_PORT:-8082}"
export ACME_PAY_MERCHANT_ID="${ACME_PAY_MERCHANT_ID:-8B67BF88-BB8D-4EB5-ADFC-0F5C97C7ED67}"
export ACME_PAY_API_KEY="${ACME_PAY_API_KEY:-local-acme-pay-key}"
export ACME_ADVICE_USERNAME="${ACME_ADVICE_USERNAME:-local-advice-user}"
export ACME_ADVICE_PASSWORD="${ACME_ADVICE_PASSWORD:-local-advice-password}"
export ACME_WEBHOOK_SECRET="${ACME_WEBHOOK_SECRET:-local-webhook-secret-change-me}"

if [[ "$RUN_IN_COMPOSE" != true ]]; then
    export ACME_ADVICE_CALLBACK_URL="${ACME_ADVICE_CALLBACK_URL:-http://host.docker.internal:8080/api/payments/acme/notifications/approved}"
    export ACME_ADJUSTMENT_CALLBACK_URL="${ACME_ADJUSTMENT_CALLBACK_URL:-http://host.docker.internal:8080/api/payments/acme/notifications/reversal}"
fi

CERT_DIR="$ROOT_DIR/.local/https"
CERT_PATH="$CERT_DIR/aspnetapp.pfx"
mkdir -p "$CERT_DIR"
if [[ "$(uname -s)" == "Linux" ]]; then
    export SSL_CERT_DIR="$HOME/.aspnet/dev-certs/trust:/usr/lib/ssl/certs"
fi
dotnet dev-certs https --trust
dotnet dev-certs https -ep "$CERT_PATH" -p "$DEV_CERT_PASSWORD"

echo "Building the database project..."
dotnet build "$DATABASE_PROJECT" -c Release

echo "Starting SQL Server Developer and Redis..."
docker compose -f "$COMPOSE_FILE" up -d --build --wait --wait-timeout 180 sqlserver redis acme-pay-stub

DACPAC_PATH="$(find "$(dirname "$DATABASE_PROJECT")/bin" -name 'AcmeIntegreatedAssessement.Database.dacpac' -type f -print | sort | tail -n 1)"
if [[ -z "$DACPAC_PATH" ]]; then
    echo "The database build did not produce AcmeIntegreatedAssessement.Database.dacpac." >&2
    exit 1
fi
RELATIVE_DACPAC_PATH="${DACPAC_PATH#"$(dirname "$DATABASE_PROJECT")/bin/"}"

echo "Deploying the database schema..."
docker compose -f "$COMPOSE_FILE" run --build --rm \
    -e "SA_PASSWORD=$SA_PASSWORD" \
    -e "TARGET_DATABASE=$TARGET_DATABASE" \
    -e "DACPAC_RELATIVE_PATH=$RELATIVE_DACPAC_PATH" \
    dacpac

if [[ "$RUN_IN_COMPOSE" == true ]]; then
    echo "Starting the application container at https://localhost:8081..."
    docker compose -f "$COMPOSE_FILE" up -d --build acme-web
    HEALTHY=false
    for attempt in {1..45}; do
        if curl --fail --silent --insecure --max-time 5 https://localhost:8081/hc >/dev/null; then
            HEALTHY=true
            break
        fi
        sleep 2
    done
    if [[ "$HEALTHY" != true ]]; then
        docker compose -f "$COMPOSE_FILE" logs --tail 100 acme-web
        echo "The application did not become healthy at https://localhost:8081." >&2
        exit 1
    fi
    echo "Application is running at https://localhost:8081"
    exit 0
fi

if docker compose -f "$COMPOSE_FILE" ps --services --status running | grep -Fxq acme-web; then
    docker compose -f "$COMPOSE_FILE" stop acme-web
fi

echo "Starting the application at https://localhost:8081 (Ctrl+C stops the app; SQL Server, Redis, and the Acme stub remain running)..."
ACME_PAY_BASE_URL="${ACME_PAY_BASE_URL:-http://localhost:$ACME_STUB_PORT/}"
exec dotnet run \
    --project "$API_PROJECT" \
    --launch-profile AcmeIntegreatedAssessement.Api \
    -- \
    --ConnectionStrings:PaymentDatabase="Server=localhost,$SQL_PORT;Database=$TARGET_DATABASE;User Id=sa;Password=$SA_PASSWORD;Encrypt=True;TrustServerCertificate=True" \
    --ConnectionStrings:RedisOmConnectionString="localhost:$REDIS_PORT,abortConnect=false" \
    --AcmePay:BaseUrl="$ACME_PAY_BASE_URL"
