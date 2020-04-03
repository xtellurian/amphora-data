#!/bin/bash

while getopts l: option
do
case "${option}"
in
l) LOCATION=${OPTARG};; # Public IP address of your ingress controller
esac
done

# location could be primaru or secondary

if [ -z "$LOCATION" ]; then
    echo "Missing location -l"
    LOCATION="australiasoutheast"
fi

pulumi stack ls
echo "Using $LOCATION"

JSON=$(pulumi stack output -j k8s)
LOC_JSON=$( jq -r ".${LOCATION}" <<< "$(echo $JSON)" ) 
set -x
name=$( jq -r '.name' <<< "$(echo $LOC_JSON)" ) 
group=$( jq -r '.group' <<< "$(echo $LOC_JSON)" ) 

set -e
az aks show -n "$name" -g "$group"

pushd .aks-shell
docker-compose build
docker-compose run --rm -e "name=$name" -e "group=$group" kubecli
popd