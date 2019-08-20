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
npm run build

# only sets stack if on develop or master
set_special_stack () {
  if [ $1 == "refs/heads/develop" ]; then
    STACK="develop"
  elif [ $1 == "refs/heads/master" ]; then
    STACK="master"
  fi
  echo "Selected Stack: $STACK"
  pulumi stack select $STACK
}

echo build reason is $BUILD_REASON

if [ $BUILD_REASON == "PullRequest" ] ; then
  set_special_stack $SYSTEM_PULLREQUEST_TARGETBRANCH
  echo "Previewing special target stack!"
  pulumi preview
  echo "Source Branch (Pull Request) is $SYSTEM_PULLREQUEST_SOURCEBRANCH"
  set_special_stack $SYSTEM_PULLREQUEST_SOURCEBRANCH
else
  # spin up the source branch stack
  echo "Source Branch is $BUILD_SOURCEBRANCH"
  set_special_stack $BUILD_SOURCEBRANCH
fi

pulumi up --yes

kvUri=$(pulumi stack output kvUri)
appUrl=$(pulumi stack output appUrl)
acrName=$(pulumi stack output acrName)

# Save the stack output variables to job variables.
echo "##vso[task.setvariable variable=kvUri;isOutput=true]$kvUri"
echo "##vso[task.setvariable variable=appUrl;isOutput=true]$appUrl" 
echo "##vso[task.setvariable variable=acrName;isOutput=true]$acrName" 
echo "##vso[task.setvariable variable=pulumiStack;isOutput=true]$STACK" 

popd
