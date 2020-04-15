#!/bin/bash

# need to set the DNS records for develop and master
# so that the ACME challenges are resolved
# and the stacks can be deleted
# without the kubernetes finalizers blocking namespace deletion

set -e
while getopts i:l:s: option
do
case "${option}"
in
i) IP=${OPTARG};;
l) LOCATION=${OPTARG};;
s) STACK=${OPTARG};;
esac
done

if [ "$IP" != "" ]; then
    echo "IP" is $IP
else
    echo "IP is empty"
    exit 1
fi
if [ "$LOCATION" != "" ]; then
    echo "LOCATION" is $LOCATION
else
    echo "LOCATION is empty"
    exit 1
fi
if [ "$STACK" != "" ]; then
    echo "STACK" is $STACK
else
    echo "STACK is empty"
    exit 1
fi


GROUP='amphora-central'
ZONE='amphoradata.com'
APP_NAME="$STACK.$LOCATION.app"
IDENTITY_NAME="$STACK.$LOCATION.identity"

if [ "$STACK" = "prod" ]; then
    echo "Not setting DNS for prod. Use Amphora Central Pulumi Stack."
else
    echo "Setting DNS for stack $STACK."
    az network dns record-set a delete --yes -g $GROUP -z $ZONE -n $APP_NAME #-n develop.australiasoutheast.app
    az network dns record-set a add-record --yes -g $GROUP -z $ZONE -n $APP_NAME -a $IP

    az network dns record-set a delete --yes -g $GROUP -z $ZONE -n $IDENTITY_NAME
    az network dns record-set a add-record --yes -g $GROUP -z $ZONE -n $IDENTITY_NAME -a $IP
fi
