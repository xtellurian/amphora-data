#!/bin/bash
# Add the pulumi CLI to the PATH
export PATH=$PATH:$HOME/.pulumi/bin

set -e
cd $(dirname "$0")

if [ -z "$STACK_OUTPUT_PATH" ]; then
    echo "STACK_OUTPUT_PATH not set"
    exit 1
else
    echo "STACK_OUTPUT_PATH is $STACK_OUTPUT_PATH"
fi

OUTPUT=$(cat $STACK_OUTPUT_PATH)
echo $OUTPUT
fqdnName=$(jq -r  '.k8sPrimary.fqdnName' <<< ${OUTPUT} ) 
ip=$(jq -r  '.k8sPrimary.ingressIp' <<< ${OUTPUT} )
./set-fqdn.sh -i $ -n $fqdnName -i $ip

fqdnName=$(jq -r  '.k8sSecondary.fqdnName' <<< ${OUTPUT} ) 
ip=$(jq -r  '.k8sSecondary.ingressIp' <<< ${OUTPUT} )
./set-fqdn.sh -i $ -n $fqdnName -i $ip
