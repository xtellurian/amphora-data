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
if [ -z "$BUILD" ]; then
    BUILD=local
fi
if [ -z "$WEBAPPID" ]; then
    WEBAPPID=$(pulumi stack output webAppResourceId)
fi
if [ -z "$STAGING" ]; then
    STAGING=false
fi

if $STAGING = true ; then
    echo "Deploying to staging slot"
    az webapp config container set --docker-custom-image-name $IMAGE:$BUILD --ids $WEBAPPID --slot staging
    #explicit set zero so I can route to it
    az webapp traffic-routing set --distribution staging=0 --ids $WEBAPPID
    az webapp config appsettings set --ids $WEBAPPID -s staging --slot-settings STACK="$STACK(staging)"
else
    echo "Deploying to production Slot"
    az webapp config container set --docker-custom-image-name $IMAGE:$BUILD --ids $WEBAPPID
fi
