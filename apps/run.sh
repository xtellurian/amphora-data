#!/bin/bash

while getopts k:p: option
do
case "${option}"
in
k) KEY_VAULT=${OPTARG};; # Azure Container Registry Name
p) PROJECT=${OPTARG};; # Azure Container Registry Name
esac
done


if [ "$KEY_VAULT" != "" ]; then
    PersistentStores=true
    kvUri=$KEY_VAULT
    echo "Using Key Vault"
fi

if [ "$PROJECT" == "test" ]; then
    echo PersistentStores: ${PersistentStores}
    echo kvUri: ${kvUri}
    echo disableKv: ${disableKv}
    echo BUILD_REASON: $BUILD_REASON
    echo "Running tester"
    docker-compose run tester
elif [ "$PROJECT" == "identity" ]; then
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
docker-compose rm
