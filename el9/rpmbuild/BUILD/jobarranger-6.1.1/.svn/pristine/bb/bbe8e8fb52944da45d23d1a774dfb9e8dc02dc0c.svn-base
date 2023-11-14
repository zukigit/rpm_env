FROM centos:centos8

LABEL org.opencontainers.image.title="Job Arranger server (PostgreSQL)" \
      org.opencontainers.image.authors="Park Seungyup <seungyup.park@jobarranger.info>" \
      org.opencontainers.image.vendor="Daiwa Institute of Research Ltd." \
      org.opencontainers.image.url="https://www.jobarranger.info/" \
      org.opencontainers.image.description="Job Arranger server with PosgreSQL database support" \
      org.opencontainers.image.licenses="GPL v2.0"

STOPSIGNAL SIGTERM

ENV TINI_VERSION=v0.19.0

RUN set -eux && \
    groupadd -g 1995 --system zabbix && \
    adduser -r --shell /sbin/nologin \
            -g zabbix -G dialout -G root \
            -d /var/lib/zabbix/ -u 1997 \
        zabbix && \
    mkdir -p /var/lib/zabbix && \
    dnf -y install --setopt=tsflags=nodocs  \
         https://download-ib01.fedoraproject.org/pub/epel/8/Everything/x86_64/Packages/l/libssh2-1.9.0-5.el8.x86_64.rpm \
         https://repo.zabbix.com/zabbix/5.0/rhel/8/x86_64/zabbix-release-5.0-1.el8.noarch.rpm \
          postgresql && \
    curl -L "https://github.com/krallin/tini/releases/download/${TINI_VERSION}/tini" -o /sbin/tini && \
    curl -L "https://github.com/krallin/tini/releases/download/${TINI_VERSION}/tini.asc" -o /tmp/tini.asc && \
    chmod +x /sbin/tini && \
    dnf -y clean all && \
    rm -rf /var/cache/yum /var/lib/yum/yumdb/* && \
    rm -rf /usr/lib/udev/hwdb.d/* && \
    rm -rf /var/cache/dnf
    

ARG MAJOR_VERSION=5.0
ARG JAZ_VERSION=${MAJOR_VERSION}

LABEL org.opencontainers.image.documentation="https://www.jobarranger.info/document/doku.php?id=start" \
      org.opencontainers.image.version="${JAZ_VERSION}" 

COPY jobarranger-server-postgresql-5.0.1-1.el8.x86_64.rpm /tmp/

RUN set -eux && \
    sed -i 's/enabled=0/enabled=1/g' /etc/yum.repos.d/CentOS-PowerTools.repo && \
    dnf --quiet makecache && \
    dnf -y install --setopt=tsflags=nodocs \
            libpq.x86_64 \
            zabbix-sender.x86_64 \
            logrotate.x86_64 \
            https://download-ib01.fedoraproject.org/pub/epel/8/Everything/x86_64/Packages/l/libssh2-devel-1.9.0-5.el8.x86_64.rpm && \
     cd /tmp/ && \
     rpm -ivh jobarranger-server-postgresql-5.0.1-1.el8.x86_64.rpm && \
     chown --quiet -R zabbix.root /etc/jobarranger /var/lib/zabbix && \
     chgrp -R 0 /etc/jobarranger/ && \
     chmod -R g=u /etc/jobarranger/ && \
     chmod +x /etc/jobarranger/alert/jasender.sh && \
     chmod +x /etc/jobarranger/monitor/jasender_monitor.sh && \
     sed -i 's/enabled=1/enabled=0/g' /etc/yum.repos.d/CentOS-PowerTools.repo && \
     rm -rf /var/cache/yum /var/lib/yum/yumdb/* && \
     rm -rf /var/cache/dnf 

EXPOSE 10055/TCP
WORKDIR /var/lib/zabbix
COPY ["docker-entrypoint.sh", "/usr/bin/"]

ENTRYPOINT ["/sbin/tini", "--", "/usr/bin/docker-entrypoint.sh"]
RUN ["chmod", "+x", "/usr/bin/docker-entrypoint.sh"]
USER 1997
