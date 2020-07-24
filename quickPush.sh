#!/bin/bash
set -e
GITHASH=$(git rev-parse HEAD)
echo "GITHASH is $GITHASH"
echo "<< Getting acrName from Pulumi >>"
pushd infra
pulumi stack select develop
ACR_NAME=$(pulumi stack output acrName)
REPOSITORY=$ACR_NAME.azurecr.io
echo "ACR_NAME set to $ACR_NAME"
popd
echo "<< Logging into ACR >>"
az acr login -n $ACR_NAME

pushd apps
echo "<< Building Containers >>"
./build-containers.sh -a $ACR_NAME -c builder -t $GITHASH
popd

echo "<< Publishing Containers >>"
docker push $REPOSITORY/webapp:$GITHASH
docker push $REPOSITORY/identity:$GITHASH

pushd k8s
pulumi stack select develop-australiasoutheast
pulumi up -c frontend:image="$ACR_NAME.azurecr.io/webapp:$GITHASH" -c identity:image="$ACR_NAME.azurecr.io/identity:$GITHASH" -y
popd
echo "<< DONE >>"