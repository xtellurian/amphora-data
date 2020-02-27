#!/bin/bash

# this should run in CI

# exit if a command returns a non-zero exit code and also print the commands and their args as they are executed
set -e

# Add the pulumi CLI to the PATH
export PATH=$PATH:$HOME/.pulumi/bin

# should pop to ./infra
pushd infra

pulumi login

echo "Running NPM install + lint + build"
npm install
npm run lint

# annoying - weird error where it's finding broken types in node_modules
# npm run build

echo build reason is $BUILD_REASON

. pulumi-stack.sh

pulumi up --yes

kvUri=$(pulumi stack output kvUri)
kvName=$(pulumi stack output kvName)
acrName=$(pulumi stack output acrName)
webAppResourceId=$(pulumi stack output webAppResourceId)
workflowTriggerId=$(pulumi stack output workflowTriggerId)
appEventGridTopicId=$(pulumi stack output appEventGridTopicId)

acrId=$(pulumi stack output acrId)
k8sName=$(pulumi stack output k8sName)
k8sGroup=$(pulumi stack output k8sGroup)



# Save the stack output variables to job variables.
echo "##vso[task.setvariable variable=kvUri;isOutput=true]$kvUri"
echo "##vso[task.setvariable variable=kvName;isOutput=true]$kvName"
echo "##vso[task.setvariable variable=acrName;isOutput=true]$acrName"
echo "##vso[task.setvariable variable=stack;isOutput=true]$STACK" 
echo "##vso[task.setvariable variable=webAppResourceId;isOutput=true]$webAppResourceId" 
echo "##vso[task.setvariable variable=workflowTriggerId;isOutput=true]$workflowTriggerId" 
echo "##vso[task.setvariable variable=appEventGridTopicId;isOutput=true]$appEventGridTopicId"

echo "##vso[task.setvariable variable=acrId;isOutput=true]$acrId"
echo "##vso[task.setvariable variable=k8sGroup;isOutput=true]$k8sGroup"
echo "##vso[task.setvariable variable=k8sName;isOutput=true]$k8sName"

echo "##vso[task.setvariable variable=k8sFqdnName;isOutput=true]$(pulumi stack output k8sFqdnName)"
echo "##vso[task.setvariable variable=k8sIngressIp;isOutput=true]$(pulumi stack output k8sIngressIp)"


popd
