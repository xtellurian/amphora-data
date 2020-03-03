#!/bin/bash
# Add the pulumi CLI to the PATH
export PATH=$PATH:$HOME/.pulumi/bin

set -e -x
cd $(dirname "$0")

if [ -z "$STACK_OUTPUT_PATH" ]; then
    echo "STACK_OUTPUT_PATH not set"
    exit 1
else
    echo "STACK_OUTPUT_PATH is $STACK_OUTPUT_PATH"
fi

fqdnName=$( echo jq -r  '.k8sPrimary.fqdnName' <<< "$(cat $STACK_OUTPUT_PATH)" ) 
ip=$( echo jq -r  '.k8sPrimary.ingressIp' <<< "$(cat $STACK_OUTPUT_PATH)" ) 
./set-fqdn.sh -i $ -n $fqdnName -i $ip

fqdnName=$( echo jq -r  '.k8sSecondary.fqdnName' <<< "$(cat $STACK_OUTPUT_PATH)" ) 
ip=$( echo jq -r  '.k8sSecondary.ingressIp' <<< "$(cat $STACK_OUTPUT_PATH)" ) 
./set-fqdn.sh -i $ -n $fqdnName -i $ip
