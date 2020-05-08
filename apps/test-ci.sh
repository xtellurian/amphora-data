#!/bin/bash

if [ -z "$CoverletOutput" ]
then
    CoverletOutput="$(pwd)"
fi

echo Coverlet Output Directory is $CoverletOutput
commonArgs="-l:trx;LogFileName=$CoverletOutput/TestOutput.xml /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=$CoverletOutput"
set -x
# run the first phase tests
dotnet test --filter Phase=One $commonArgs
# run all the tests
dotnet test $commonArgs
