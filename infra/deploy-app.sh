#1/bin/bash
# Add the pulumi CLI to the PATH
export PATH=$PATH:$HOME/.pulumi/bin

set -e -x
cd $(dirname "$0")

if [ -z "$ACR_NAME" ]; then
    ACR_NAME=$(pulumi stack output acrName)
fi
if [ -z "$IMAGE" ]; then
    IMAGE="$ACR_NAME.azurecr.io/webapp"
fi
if [ -z "$GITHASH" ]; then
    GITHASH=$(git rev-parse HEAD)
fi
if [ -z "$BUILD" ]; then
    BUILD=local
fi
if [ -z "$WEBAPPID" ]; then
    WEBAPPID=$(pulumi stack output webAppResourceId)
fi
if [ -z "$CONTEXT" ]; then
    CONTEXT="../apps"
fi
if [ -z "$STAGING" ]; then
    STAGING=false
fi
if [ -z "$CACHED_IMAGE" ]; then
    CACHED_IMAGE=$ACR_NAME.azurecr.io/builder:latest
fi

az acr login -n $ACR_NAME
docker pull $CACHED_IMAGE || true # don't fail when cached image doesn't exist.

# build
docker build --target builder --cache-from $CACHED_IMAGE -t $CACHED_IMAGE $CONTEXT # rebuild the cache
docker build --cache-from $CACHED_IMAGE -t $IMAGE:latest -t $IMAGE:$GITHASH -t $IMAGE:$BUILD --build-arg gitHash=$GITHASH $CONTEXT

# push
docker push $CACHED_IMAGE
docker push $IMAGE:latest
docker push $IMAGE:$GITHASH
docker push $IMAGE:$BUILD 

if $STAGING = true ; then
    echo "Deploying to staging slot"
    az webapp config container set --docker-custom-image-name $IMAGE:$GITHASH --ids $WEBAPPID --slot staging
    #explicit set zero so I can route to it
    az webapp traffic-routing set --distribution staging=0 --ids $WEBAPPID
    az webapp config appsettings set --ids $WEBAPPID -s staging --slot-settings STACK="$STACK(staging)"
else
    echo "Deploying to production Slot"
    az webapp config container set --docker-custom-image-name $IMAGE:$GITHASH --ids $WEBAPPID
fi
