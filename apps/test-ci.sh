#!/bin/bash

if [ -z "$CoverletOutput" ]
then
    CoverletOutput="$(pwd)"
fi

# echo Running Yarn scripts...
# yarn --cwd api/ClientApp install
# yarn --cwd api/ClientApp lint
# yarn --cwd api/ClientApp test

echo Coverlet Output Directory is $CoverletOutput
commonArgs="-l:trx;LogFileName=$CoverletOutput/TestOutput.xml /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=$CoverletOutput/"
set -x
# run the first phase tests
dotnet test --filter Phase=One $commonArgs

# run all the tests
dotnet test --filter DisplayName~Amphora.Tests.Identity.Integration $commonArgs
