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

az acr login -n $ACR_NAME
docker build -t $IMAGE:latest -t $IMAGE:$GITHASH -t $IMAGE:$BUILD --build-arg gitHash=$GITHASH $CONTEXT
docker push $IMAGE:latest
docker push $IMAGE:$GITHASH
docker push $IMAGE:$BUILD 
echo "Setting CI for web app"
# WEBHOOKURI=$(az webapp deployment container config --enable-cd true --ids "$WEBAPPID" --output tsv --query CI_CD_URL)
# echo "Webhook is $WEBHOOKURI"
# az acr webhook create -n WebAppCD -r $ACR_NAME --scope webapp:latest --actions push delete --uri $WEBHOOKURI

# this can set a slot, and then we don't need the webhook
az webapp config container set --docker-custom-image-name $IMAGE:$GITHASH --ids $WEBAPPID
