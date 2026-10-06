FROM nginx:1.26-alpine

RUN apk add --no-cache openssl bash curl \
    && wget -q -O /usr/local/bin/confd https://github.com/kelseyhightower/confd/releases/download/v0.17.0/confd-0.17.0-linux-amd64 \
    && chmod +x /usr/local/bin/confd

RUN mkdir -p /etc/confd/conf.d /etc/confd/templates

COPY confd/conf.d/app.toml /etc/confd/conf.d/app.toml
COPY templates/app.conf.tmpl /etc/confd/templates/app.conf.tmpl
COPY scripts/start-nginx.sh /usr/local/bin/start-nginx.sh

RUN chmod +x /usr/local/bin/start-nginx.sh

EXPOSE 80 443

CMD ["/usr/local/bin/start-nginx.sh"]
