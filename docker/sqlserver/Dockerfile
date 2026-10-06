FROM mcr.microsoft.com/mssql/server:2022-latest

USER root

RUN apt-get update \
    && apt-get install -y --no-install-recommends openssh-server \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir -p /var/run/sshd \
    && ssh-keygen -A

COPY scripts/run-sqlserver.sh /usr/local/bin/run-sqlserver.sh
RUN chmod +x /usr/local/bin/run-sqlserver.sh

ENV SQL_SSH_PASSWORD=ZapperSSH!

EXPOSE 1433
EXPOSE 2223

CMD ["/usr/local/bin/run-sqlserver.sh"]
