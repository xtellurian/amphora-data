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
appUrl=$(pulumi stack output appUrl)
acrName=$(pulumi stack output acrName)
webAppResourceId=$(pulumi stack output webAppResourceId)

# Save the stack output variables to job variables.
echo "##vso[task.setvariable variable=kvUri;isOutput=true]$kvUri"
echo "##vso[task.setvariable variable=kvName;isOutput=true]$kvName"
echo "##vso[task.setvariable variable=appUrl;isOutput=true]$appUrl" 
echo "##vso[task.setvariable variable=acrName;isOutput=true]$acrName" 
echo "##vso[task.setvariable variable=stack;isOutput=true]$STACK" 
echo "##vso[task.setvariable variable=webAppResourceId;isOutput=true]$webAppResourceId" 

popd
