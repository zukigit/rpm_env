FROM  redhat/ubi9:latest

COPY json-c-0.9.tar.gz libtar-1.2.20-src-11.31.tar.gz /tmp/
COPY cent9.repo /etc/yum.repos.d/

RUN dnf install https://dl.fedoraproject.org/pub/epel/9/Everything/x86_64/Packages/e/epel-release-9-9.el9.noarch.rpm -y && \
    dnf update -y && \
    dnf install mariadb-devel postgresql-devel libssh2-devel openssl-devel -y && \
    dnf install gcc automake autoconf make -y && \
    dnf install rpmdevtools rpmlint -y && \
    dnf remove epel-release -y && \
    dnf clean all && \
    rm -rf /var/cache/yum && \
    tar -xvzf /tmp/json-c-0.9.tar.gz -C /tmp/ && \
    tar -xvzf /tmp/libtar-1.2.20-src-11.31.tar.gz -C /tmp/ && \
    cd /tmp/json-c-0.9/ && \
    ./configure --with-pic &&  \
    make && make install && \
    cd /tmp/libtar-1.2.20/ && \
    ./configure --with-pic &&  \
    make && make install && \
    rm -rf /tmp/libtar-1.2.20 /tmp/json-c-0.9 /tmp/*.tar.gz /etc/yum.repos.d/cent9.repo

CMD ["tail", "-f", "/dev/null"]
