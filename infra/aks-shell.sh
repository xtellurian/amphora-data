#!/bin/bash

while getopts l: option
do
case "${option}"
in
l) LOCATION=${OPTARG};; # Public IP address of your ingress controller
esac
done

# location could be k8sPrimary or k8sSecondary

if [ -z "$LOCATION" ]; then
    echo "Missing location -l"
    LOCATION="k8sPrimary"
    echo "Using Primary Location"
fi

JSON=$(pulumi stack output -j $LOCATION)
name=$( jq -r  '.name' <<< "$(echo $JSON)" ) 
group=$( jq -r  '.group' <<< "$(echo $JSON)" ) 

echo "Name: $name, Group: $group"
# az aks get-credentials -n "$name" -g "$group"

# docker run -it -e "name=$name" -e "group=$group" mcr.microsoft.com/azure-cli

pushd .aks-shell
docker-compose build
docker-compose run -e "name=$name" -e "group=$group" kubecli
popd