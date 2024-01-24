#!/bin/bash

source scl_source enable devtoolset-8  
rpmbuild -ba /home/moon/rpmbuild/SPECS/jobarranger8.spec
