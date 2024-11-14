FROM centos:7

COPY infinite.sh /tmp/
RUN yum groupinstall "Development Tools" -y\
    yum install openssl-devel -y \
    yum install libssh2 -y \
    yum install libssh2-devel -y \
    yum install mariadb-devel -y \
    yum install postgresql-devel -y \
    yum install rpmdevtools rpmlint -y
CMD [ "./tmp/infinite.sh" ]