#!/bin/bash

# run the first phase tests
dotnet test --filter Phase=One -l:trx;LogFileName=/output/TestOutput.xml /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=/output/
# run all the tests
dotnet test -l:trx;LogFileName=/output/TestOutput.xml /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=/output/