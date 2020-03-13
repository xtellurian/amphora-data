#!/bin/bash

while getopts p: option
do
case "${option}"
in
p) PROJECT=${OPTARG};; # Azure Container Registry Name
esac
done

if [ "$PROJECT" == "test" ]; then
    echo PersistentStores: ${PersistentStores}
    echo kvUri: ${kvUri}
    echo disableKv: ${disableKv}
    echo BUILD_REASON: $BUILD_REASON
    echo "Running tester"
    docker-compose run tester
else if [ "$PROJECT" == "identity" ]; then
    echo "Running Identity Project"
    docker-compose run --service-ports identity
else
    echo "Running identity in the background"
    docker-compose run --service-ports -d identity
    sleep 5
    echo "Starting api"
    dotnet run --project api
fi

echo "Docker Compose Down"
docker-compose down
