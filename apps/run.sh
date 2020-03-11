#!/bin/bash

docker-compose run -d identity
sleep 5
dotnet run --project api
docker-compose down