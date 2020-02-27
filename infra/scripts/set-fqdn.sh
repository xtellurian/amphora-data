#!/bin/bash
# ./set-fqdn.sh -i $(pulumi stack output k8sIngressIp) -n $(pulumi stack output k8sFqdnName)
set -e
while getopts i:n: option
do
case "${option}"
in
i) IP=${OPTARG};; # Public IP address of your ingress controller
n) DNSNAME=${OPTARG};; # Name to associate with public IP address
esac
done

if [ "$IP" != "" ]; then
    echo "IP" is $IP
else
    echo "IP is empty"
    exit 1
fi
if [ "$DNSNAME" != "" ]; then
    echo "DNSNAME" is $DNSNAME
else
    echo "DNSNAME is empty"
    exit 1
fi

# Get the resource-id of the public ip
PUBLICIPID=$(az network public-ip list --query "[?ipAddress!=null]|[?contains(ipAddress, '$IP')].[id]" --output tsv)

# Update public ip address with DNS name
az network public-ip update --ids $PUBLICIPID --dns-name $DNSNAME