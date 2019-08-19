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


# the default stack
pulumi stack select $STACK

if [ $STACK == "ci" ]; then
    echo destroying CI
    pulumi destroy -y
fi


popd
