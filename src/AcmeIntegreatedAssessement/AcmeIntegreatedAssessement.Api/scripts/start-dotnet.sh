#!/bin/sh
set -eu

if [ -n "${SSH_ROOT_PASSWORD:-}" ]; then
  echo "root:${SSH_ROOT_PASSWORD}" | chpasswd
fi

# Start sshd in the background so tools like DataGrip can tunnel through this container.
/usr/sbin/sshd

echo "Starting AcmeIntegreatedAssessement.Api..."
exec dotnet AcmeIntegreatedAssessement.Api.dll
