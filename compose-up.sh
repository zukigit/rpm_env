#!/bin/bash

docker compose -f ./el6/docker-compose.yml up -d
docker compose -f ./el7/docker-compose.yml up -d
docker compose -f ./el8/docker-compose.yml up -d
docker compose -f ./el9/docker-compose.yml up -d