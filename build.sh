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

if [ $? -eq 0 ]; then
    ./el7/build_7.sh
    ./el8/build_8.sh
else
    echo "File copy failed. Return code: $?"
fi