#!/bin/bash

if [ "$#" -ne 1 ]; then
    echo "Usage: $0 version_number"
    exit 1
fi

src_name="jobarranger-$1"

folder="./src"
for file_path in "$folder"/*; do
    src_folder=$(basename "$file_path")    
done

mv ./src/$src_folder ./src/$src_name
cd ./src
tar cvzf "$src_name.tar.gz" $src_name/

cp -f "$src_name.tar.gz" ../el7/rpmbuild/SOURCES/
cp -f "$src_name.tar.gz" ../el8/rpmbuild/SOURCES/
cp -f "$src_name.tar.gz" ../el9/rpmbuild/SOURCES/

if [ $? -eq 0 ]; then
    ./el7/build_7.sh
else
    echo "file copy failed. Return code: $?"
fi

if [ $? -eq 0 ]; then
    ./el8/build_8.sh
else
    echo "el7 failed. Return code: $?"
fi

if [ $? -eq 0 ]; then
    ./el9/build_8.sh
else
    echo "el8 failed. Return code: $?"
fi

if [ $? -eq 0 ]; then
    rm -rf ./exports/el7/*
    rm -rf ./exports/el8/*
    rm -rf ./exports/el9/*

    cp -r el7/rpmbuild/RPMS/x86_64/ ./exports/el7/
    cp -r el8/rpmbuild/RPMS/x86_64/ ./exports/el8/
    cp -r el9/rpmbuild/RPMS/x86_64/ ./exports/el9/
else
    echo "el9 failed. Return code: $?"
fi
