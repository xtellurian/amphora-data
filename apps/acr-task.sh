#!/bin/bash
set -e -x

az acr task create -n $TASK_NAME -r $ACR_NAME --branch $BRANCH -f apps/acr-task.yaml \
    -c https://dev.azure.com/amphoradata/Amphora/_git/Amphora --git-access-token $ACCESS_TOKEN \
    --commit-trigger-enabled false 

echo "Githash is $GITHASH"

az acr task run -n $TASK_NAME -r $ACR_NAME --set GitHash=$GITHASH
