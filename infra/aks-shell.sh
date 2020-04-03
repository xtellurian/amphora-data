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
    LOCATION="primary"
    echo "Using Primary Location"
fi

pulumi stack ls

JSON=$(pulumi stack output -j $LOCATION)
name=$( jq -r  '.name' <<< "$(echo $JSON)" ) 
group=$( jq -r  '.group' <<< "$(echo $JSON)" ) 

az aks show -n "$name" -g "$group"

pushd .aks-shell
docker-compose build
docker-compose run --rm -e "name=$name" -e "group=$group" kubecli
popd