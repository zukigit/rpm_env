FROM  redhat/ubi9

COPY sleep10.sh /tmp
COPY json-c-0.9.tar.gz /tmp
COPY libtar-1.2.20-src-11.31.tar.gz /tmp
COPY cent9.repo /etc/yum.repos.d/

RUN dnf update -y && \
    dnf install https://dl.fedoraproject.org/pub/epel/9/Everything/x86_64/Packages/e/epel-release-9-7.el9.noarch.rpm -y && \
    dnf install libgomp gcc automake autoconf make -y && \
    yum install openssl-devel -y && \
    yum install libssh2 -y && \
    yum install libssh2-devel -y && \
    yum install mariadb-devel -y && \
    yum install postgresql-devel -y && \
    yum install rpmdevtools rpmlint -y && \
    yum clean all && \
    rm -rf /var/cache/yum

RUN tar -xvzf /tmp/json-c-0.9.tar.gz -C /tmp/ && \
    tar -xvzf /tmp/libtar-1.2.20-src-11.31.tar.gz -C /tmp/ && \
    rm -rf /tmp/*.tar.gz

RUN cd /tmp/json-c-0.9/ && \
    ./configure --with-pic &&  \
    make && make install

RUN cd /tmp/libtar-1.2.20/ && \
    ./configure --with-pic &&  \
    make && make install

CMD [ "./tmp/sleep10.sh" ]