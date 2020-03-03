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
    exit 1
fi

JSON=$(pulumi stack output $LOCATION)
name=$( jq -r  '.name' <<< "$(echo $JSON)" ) 
group=$( jq -r  '.group' <<< "$(echo $JSON)" ) 

echo "Name: $name, Group: $group"
az aks get-credentials -n "$name" -g "$group"
