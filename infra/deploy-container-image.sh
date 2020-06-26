#1/bin/bash
# Add the pulumi CLI to the PATH
export PATH=$PATH:$HOME/.pulumi/bin

set -e -x

if [ -z "$ACR_NAME" ]; then
    echo "ACR_NAME is required"
    exit 1
fi
if [ -z "$GITHASH" ]; then
    GITHASH=$(git rev-parse HEAD)
fi
if [ -z "$BUILD" ]; then
    BUILD=local
fi
if [ -z "$CACHED_IMAGE" ]; then
    CACHED_IMAGE=$ACR_NAME.azurecr.io/builder:latest
fi

az acr login -n $ACR_NAME
docker pull $CACHED_IMAGE || true # don't fail when cached image doesn't exist.

# build
pushd apps
# don't skip here, this is where it gets built for prod.
./build-containers.sh -a $ACR_NAME -c $CACHED_IMAGE -g $GITHASH -t $BUILD

echo "Pushing to ACR: $ACR_NAMR"
# push
REPOSITORY=$ACR_NAME.azurecr.io

docker push $REPOSITORY/builder:latest
docker push $REPOSITORY/builder:$BUILD

docker push $REPOSITORY/webapp:latest
docker push $REPOSITORY/webapp:$BUILD

docker push $REPOSITORY/identity:latest
docker push $REPOSITORY/identity:$BUILD
