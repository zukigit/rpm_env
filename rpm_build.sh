#!/bin/bash

docker exec -it --user root rpm_env chown -R moon.moon /home/moon/rpmbuild/
docker exec -it --user moon rpm_env rpmbuild -ba /home/moon/rpmbuild/SPECS/jobarranger8.spec