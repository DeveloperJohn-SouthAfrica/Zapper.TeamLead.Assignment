FROM redis:7.0-alpine

RUN apk add --no-cache openssh \
    && mkdir -p /var/run/sshd \
    && ssh-keygen -A

COPY scripts/run-redis.sh /usr/local/bin/run-redis.sh
RUN chmod +x /usr/local/bin/run-redis.sh

ENV REDIS_SSH_PASSWORD=ZapperSSH!

EXPOSE 6379
EXPOSE 2224

ENTRYPOINT ["/usr/local/bin/run-redis.sh"]
