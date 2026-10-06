#!/bin/sh
set -euo pipefail

SSH_PASSWORD=${SSH_PASSWORD:-ZapperSSH!}

echo "app:${SSH_PASSWORD}" | chpasswd

exec /usr/sbin/sshd -D
