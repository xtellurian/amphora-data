#!/bin/bash

# is this required?
# dotnet test --filter Category=Versioning

function compare {
    echo "Comaring version $1"
    rm openapi.$1.diff.json
    docker run -v `pwd`/bin/Debug/netcoreapp3.1:/input xtellurian/openapi-diff-container \
    https://app.amphoradata.com/swagger/$1/swagger.json /input/$1.swagger.json \
    >> openapi.$1.diff.json
}  

compare v1
compare v2
