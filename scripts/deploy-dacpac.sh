#!/usr/bin/env bash
set -euo pipefail

COMPOSE_FILE="${1:-docker-compose.yml}"
PROJECT_DIR="src/AcmeIntegreatedAssessement/AcmeIntegreatedAssessement.Database"
SA_PASSWORD="${SA_PASSWORD:-YourStrong!Passw0rd}"
TARGET_DATABASE="${TARGET_DATABASE:-Payments}"

echo "Building database project so the DACPAC can be deployed..."
dotnet build "$PROJECT_DIR/AcmeIntegreatedAssessement.Database.sqlproj" -c Release

BIN_DIR="$PROJECT_DIR/bin"
DACPAC_FILE=$(find "$BIN_DIR" -name 'AcmeIntegreatedAssessement.Database.dacpac' | sort | tail -n1)

if [ -z "$DACPAC_FILE" ]; then
  echo "Unable to locate AcmeIntegreatedAssessement.Database.dacpac in $BIN_DIR"
  exit 1
fi

RELATIVE_PATH=${DACPAC_FILE#"$BIN_DIR/"}

echo "Deploying $RELATIVE_PATH to SQL Server via SQLPackage..."
docker compose -f "$COMPOSE_FILE" run --rm \
  -e SA_PASSWORD="$SA_PASSWORD" \
  -e TARGET_DATABASE="$TARGET_DATABASE" \
  -e DACPAC_RELATIVE_PATH="$RELATIVE_PATH" \
  dacpac
