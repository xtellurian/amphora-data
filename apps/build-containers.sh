#!/bin/bash

while getopts a:c:g:t: option
do
case "${option}"
in
a) ACR_NAME=${OPTARG};; # Azure Container Registry Name
c) CACHED_IMAGE=${OPTARG};; # Image to cache from
g) GITHASH=${OPTARG};; # The git hash
t) TAG=${OPTARG};; # The git hash
esac
done

if [ "$ACR_NAME" != "" ]; then
    echo "ACR_NAME is $ACR_NAME"
else
    echo "ACR_NAME is empty, building locals"
    docker build -t builder .
    echo "BUILDER DONE"
    docker build -t identity -f identity/Dockerfile --build-arg "BASE=builder" .
    echo "IDENTITY SERVER DONE"
    docker build -t webapp -f api/Dockerfile --build-arg "BASE=builder" .
    echo "WEBAPP SERVER DONE"
    exit 1
fi
if [ "$CACHED_IMAGE" != "" ]; then
    echo "CACHED_IMAGE is $CACHED_IMAGE"
else
    echo "CACHED_IMAGE is empty"
    exit 1
fi
if [ "$GITHASH" != "" ]; then
    echo "GITHASH is $GITHASH"
else
    echo "GITHASH is empty. Setting..."
    GITHASH=$(git rev-parse HEAD)
    echo "GITHASH is now $GITHASH"
fi
if [ "$TAG" != "" ]; then
    echo "TAG is $TAG"
else
    echo "TAG is empty"
    exit 1
fi

REGISTRY=$ACR_NAME.azurecr.io
set -e
echo "<< Building Builder Container >>"
docker build -t builder -t $REGISTRY/builder:$TAG -t $REGISTRY/builder:$GITHASH -t $REGISTRY/builder:latest --cache-from $CACHED_IMAGE .

echo "<< Building API Container >>"
docker build -f api/Dockerfile -t api -t $REGISTRY/webapp:$TAG -t $REGISTRY/webapp:$GITHASH -t $REGISTRY/webapp:latest --build-arg "BASE=builder" .

echo "<< Building Identity Container >>"
docker build -f identity/Dockerfile -t identity -t $REGISTRY/identity:$TAG -t $REGISTRY/webapp:$GITHASH -t $REGISTRY/identity:latest --build-arg gitHash=$GITHASH --build-arg "BASE=builder" .

echo "Finished Building Containers"
