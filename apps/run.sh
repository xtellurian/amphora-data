#!/bin/bash

docker-compose run --service-ports -d identity
sleep 5
dotnet run --project api
docker-compose down