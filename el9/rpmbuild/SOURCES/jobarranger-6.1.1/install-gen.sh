#!/bin/bash
##configure.inのAC_INIT(jobarranger, x.x.x) Version　変更

aclocal -I m4
autoconf
autoheader
automake -a
automake