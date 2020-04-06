#!/bin/bash

pulumi stack ls

JSON=$(pulumi stack output -j aks)
set -x
name=$( jq -r '.name' <<< "$(echo $JSON)" )
group=$( jq -r '.group' <<< "$(echo $JSON)" )

set -e
az aks show -n "$name" -g "$group"

pushd .aks-shell
docker-compose build
docker-compose run --rm -e "name=$name" -e "group=$group" kubecli
popd