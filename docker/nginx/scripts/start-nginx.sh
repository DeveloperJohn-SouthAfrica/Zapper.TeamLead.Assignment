#!/bin/sh
set -euo pipefail

confd -onetime -backend env -config-file /etc/confd/conf.d/app.toml

exec nginx -g 'daemon off;'
