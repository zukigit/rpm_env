#!/bin/bash

rm -rf ./exports/el7/*
rm -rf ./exports/el8/*
rm -rf ./exports/el9/*

cp -r el7/rpmbuild/RPMS/x86_64/ ./exports/el7/
cp -r el8/rpmbuild/RPMS/x86_64/ ./exports/el8/
cp -r el9/rpmbuild/RPMS/x86_64/ ./exports/el9/