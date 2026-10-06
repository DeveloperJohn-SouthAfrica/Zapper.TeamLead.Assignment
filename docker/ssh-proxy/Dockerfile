FROM alpine:3.18

RUN apk add --no-cache openssh bash \
    && adduser -D -u 1000 app \
    && mkdir -p /var/run/sshd \
    && ssh-keygen -A

COPY scripts/entrypoint.sh /usr/local/bin/entrypoint.sh
RUN chmod +x /usr/local/bin/entrypoint.sh

EXPOSE 22

CMD ["/usr/local/bin/entrypoint.sh"]
