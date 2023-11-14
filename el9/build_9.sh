#!/bin/bash

docker exec -it --user root rpm_env_9 chown -R moon.moon /home/moon/rpmbuild/
docker exec -it --user moon rpm_env_9 rpmbuild -ba /home/moon/rpmbuild/SPECS/jobarranger9.spec