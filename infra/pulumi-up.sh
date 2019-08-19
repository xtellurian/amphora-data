#!/bin/bash

# this should run in CI

# exit if a command returns a non-zero exit code and also print the commands and their args as they are executed
set -e -x

# Add the pulumi CLI to the PATH
export PATH=$PATH:$HOME/.pulumi/bin

# should pop to ./infra
pushd infra

pulumi login

npm install
# npm run build

# only sets stack if on develop or master
set_special_stack () {
  if [ $1 == "refs/heads/develop" ]; then
    echo "Special Stack: Develop"
    pulumi stack select develop
  elif [ $1 == "refs/heads/master" ]; then
      echo "Special Stack: Master"
      pulumi stack select master
  fi
}

# the default stack
pulumi stack select ci

# if [ $BUILD_REASON == "PullRequest" ] ; then
#   set_special_stack $SYSTEM_PULLREQUEST_TARGETBRANCH
#   echo "Previewing special target stack!"
#   pulumi preview
#   pulumi stack select ci
# else 
#   set_special_stack $BUILD_SOURCEBRANCH
# fi

echo build reason is $BUILD_REASON

oldImageName = $(pulumi stack output imageName)
acrName = $(pulumi stack output acrName)

if [[ ! -z $oldImageName ]] ; then
  echo "Attempting pulling cache"
  az acr login -n $acrName
  docker pull $oldImageName
fi

pulumi up --yes

kvUri=$(pulumi stack output kvUri)
echo stack is $kvUri

# Save the stack output variables to job variables.
echo "##vso[task.setvariable variable=kvUri; isOutput=true]$kvUri"
echo "##vso[task.setvariable variable=pulumiStack; isOutput=true]ci" # see above where stack is selected

popd
