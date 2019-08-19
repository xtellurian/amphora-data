#!/bin/bash

# this should run in CI

# exit if a command returns a non-zero exit code and also print the commands and their args as they are executed
set -e -x

# Add the pulumi CLI to the PATH
export PATH=$PATH:$HOME/.pulumi/bin

# should pop to ./infra
pwd

pulumi login

npm install
# npm run build

set_stack () {
  if [ $1 == "refs/heads/develop" ]; then
    pulumi stack select develop
  elif [ $1 == "refs/heads/master" ]; then
      pulumi stack select master
  else:
    pulumi stack select ci
  fi
}


# https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables?view=vsts
case $BUILD_REASON in
  PullRequest)
      set_stack $SYSTEM_PULLREQUEST_TARGETBRANCH
      pulumi preview
    ;;
  BuildCompletion|BatchedCI|IndividualCI)
      set_stack $BUILD_SOURCEBRANCH
      pulumi up --yes
    ;;
  *)
esac

echo build reason is $BUILD_REASON

# Save the stack output variables to job variables.
echo "##vso[task.setvariable variable=kvUri; isOutput=true]$(pulumi stack output kvUri)"
