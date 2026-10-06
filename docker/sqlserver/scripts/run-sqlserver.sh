#!/bin/bash
set -euo pipefail

if [ -n "${SQL_SSH_PASSWORD:-}" ]; then
  echo "root:${SQL_SSH_PASSWORD}" | chpasswd
fi

# Start sshd so DB admins can tunnel through this container.
/usr/sbin/sshd

exec /opt/mssql/bin/sqlservr
