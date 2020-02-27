#!/bin/bash
# ./aks_acr.sh -a $(pulumi stack output acrId) -n $(pulumi stack output k8sName) -g $(pulumi stack output k8sGroup)

set -e
while getopts a:n:g: option
do
case "${option}"
in
a) ACR_ID=${OPTARG};;
n) AKS_NAME=${OPTARG};;
g) AKS_RG=${OPTARG};;
esac
done

if [ "$ACR_ID" != "" ]; then
    echo "ACR_ID" is $ACR_ID
else
    echo "ACR_ID is empty"
    exit 1
fi
if [ "$AKS_NAME" != "" ]; then
    echo "AKS_NAME is " $AKS_NAME
else
    echo "AKS_NAME is empty"
    exit 1
fi
if [ "$AKS_RG" != "" ]; then
    echo "AKS_RG is " $AKS_RG
else
    echo "AKS_RG is empty"
    exit 1
fi

az aks update -n $AKS_NAME -g $AKS_RG --attach-acr $ACR_ID
