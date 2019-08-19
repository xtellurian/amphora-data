#!/bin/bash

# this should run in CI

# exit if a command returns a non-zero exit code and also print the commands and their args as they are executed
set -e -x

# Add the pulumi CLI to the PATH
export PATH=$PATH:$HOME/.pulumi/bin

pushd infra/

pulumi login

npm install
# npm run build

pulumi stack select ci

# https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables?view=vsts
case $BUILD_REASON in
  PullRequest)
      pulumi preview
    ;;
  BuildCompletion|BatchedCI)
      pulumi up --yes
    ;;
  *)
esac

echo build reason is $BUILD_REASON

# Save the stack output variables to job variables.
echo "##vso[task.setvariable variable=kvUri;isOutput=true]$(pulumi stack output kvUri)"

popd