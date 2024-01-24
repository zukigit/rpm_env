#!/bin/bash

docker exec -it --user root rhel6_rpm_env chown -R moon.moon /home/moon/rpmbuild/
docker exec -it --user moon rhel6_rpm_env ./tmp/low_build_6.sh
