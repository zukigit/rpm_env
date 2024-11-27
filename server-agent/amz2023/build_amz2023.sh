#!/bin/bash

target_folder="../../src/"
src_folder_name="jobarranger-"

if [ "$#" -ne 1 ]; then
    echo "[ERROR] Usage: $0 version_number"
    exit 1
fi
src_folder_name=$src_folder_name$1

files_count=$(ls $target_folder | wc -l)
if [ $files_count -eq 0 ]; then
    echo "[ERROR] Please put source code folder to ../src. "
    exit 1
fi
if [ $files_count -ne 1 ]; then
    echo "[ERROR] ../src only accept one folder. Please make ../src clean except source code folder. "
    exit 1
fi

docker compose up -d
if [ $? -ne 0 ]; then
    echo "[ERROR] Docker container failed to start. try again!"
    exit 1
fi

for file_name in "$target_folder"*; do
    target_folder=$target_folder$(basename "$file_name")    
done

rm -rf ./src/*
rm -rf ./rpmbuild/SOURCES/*
cp -rf $target_folder ./src/$src_folder_name
cd ./src/
tar -cvzf ../rpmbuild/SOURCES/$src_folder_name.tar.gz $src_folder_name/

docker exec -it --user root rpm_env_amzn2023 chown -R dat.dat /home/dat/rpmbuild/
docker exec -it --user dat rpm_env_amzn2023 rpmbuild -ba /home/dat/rpmbuild/SPECS/jobarranger.spec
docker kill rpm_env_amzn2023
