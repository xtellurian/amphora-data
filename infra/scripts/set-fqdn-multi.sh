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
# main location
fqdnName=$(jq -r '.k8s.australiasoutheast.fqdnName' <<< ${OUTPUT} ) 
ip=$(jq -r '.k8s.australiasoutheast.ingressIp' <<< ${OUTPUT} )
location=$(jq -r '.k8s.australiasoutheast.location' <<< ${OUTPUT} )
./set-fqdn.sh -i $ -n $fqdnName -i $ip
./update-main-dns.sh -i $ip -l $location -s $STACK

# other location
fqdnName=$(jq -r  '.k8s.australiaeast.fqdnName' <<< ${OUTPUT} ) 
ip=$(jq -r  '.k8s.australiaeast.ingressIp' <<< ${OUTPUT} )
location=$(jq -r  '.k8s.australiaeast.location' <<< ${OUTPUT} )
./set-fqdn.sh -i $ -n $fqdnName -i $ip
./update-main-dns.sh -i $ip -l $location -s $STACK
