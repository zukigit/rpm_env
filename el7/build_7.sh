#!/bin/bash

docker exec -it --user root rhel7_rpm_env chown -R moon.moon /home/moon/rpmbuild/
docker exec -it --user moon rhel7_rpm_env ./tmp/build_7.sh
