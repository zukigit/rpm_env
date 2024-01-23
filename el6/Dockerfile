FROM  zukidocker/oss:rhel6

# RUN dnf update -y && \
#     dnf install https://archives.fedoraproject.org/pub/archive/epel/6/x86_64/Packages/e/epel-release-6-8.noarch.rpm -y && \
#     dnf install libgomp automake autoconf make -y && \
#     yum install openssl-devel -y && \
#     yum install libssh2 -y && \
#     yum install libssh2-devel -y && \
#     yum install mariadb-devel -y && \
#     yum install postgresql-devel -y && \
#     yum install rpmdevtools rpmlint -y && \
#     yum clean all && \
#     rm -rf /var/cache/yum
COPY cent6.repo /etc/yum.repos.d/
COPY maria.repo /etc/yum.repos.d/
COPY sleep10.sh /tmp
COPY json-c-0.9.tar.gz /tmp
COPY libtar-1.2.20-src-11.31.tar.gz /tmp

# RUN subscription-manager repos --enable rhel-6-server-optional-rpms \
#     --enable rhel-server-rhscl-6-rpms && \
    # yum install https://archives.fedoraproject.org/pub/archive/epel/6/x86_64/Packages/e/epel-release-6-8.noarch.rpm -y && \
    # yum update -y && \
    # yum install libgomp automake autoconf make -y && \
    # yum install openssl-devel -y && \
    # yum install libssh2 -y && \
    # yum install libssh2-devel -y && \
    ##
    # yum install MariaDB-devel -y && \
    ##
    # yum install postgresql-devel -y && \
RUN yum install rpmdevtools rpmlint -y && \
    yum install devtoolset-8 llvm-toolset-6.0 -y && \
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