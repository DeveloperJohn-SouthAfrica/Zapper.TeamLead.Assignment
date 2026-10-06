#!/bin/sh
set -euo pipefail

if [ -n "${REDIS_SSH_PASSWORD:-}" ]; then
  echo "root:${REDIS_SSH_PASSWORD}" | chpasswd
fi

/usr/sbin/sshd

exec /usr/local/bin/docker-entrypoint.sh redis-server
