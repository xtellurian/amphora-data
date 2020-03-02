#!/bin/bash
# Add the pulumi CLI to the PATH
export PATH=$PATH:$HOME/.pulumi/bin

set -e -x
cd $(dirname "$0")

if [ -z "$STACK_OUTPUT_PATH" ]; then
        echo "Unknown Stack Output Path"
        exit 1
    fi

    STACK_OUTPUT_PATH=$(Pipeline.Workspace)/artifacts/apps/$STACK.output.json
fi

fqdnName=$( echo jq -r  '.k8sPrimary.fqdnName' <<< "$(cat $STACK_OUTPUT_PATH)" ) 
ip=$( echo jq -r  '.k8sPrimary.ingressIp' <<< "$(cat $STACK_OUTPUT_PATH)" ) 
./set-fqdn.sh -i $ -n $fqdnName -i $ip

fqdnName=$( echo jq -r  '.k8sSecondary.fqdnName' <<< "$(cat $STACK_OUTPUT_PATH)" ) 
ip=$( echo jq -r  '.k8sSecondary.ingressIp' <<< "$(cat $STACK_OUTPUT_PATH)" ) 
./set-fqdn.sh -i $ -n $fqdnName -i $ip
