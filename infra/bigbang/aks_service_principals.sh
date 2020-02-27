#!/bin/bash

#NOTE: this is run by a AD admin to give the deployment SP enough rights to create/ destroy other SPs

set -e
set -x

PROD_SUBSCRIPTION_ID="cfd587d5-8215-4c5c-83bd-f61df6ba51c3"
DEV_SUBSCRIPTION_ID="651f2ed5-6e2f-41d0-b533-0cd28801ef2a"

# uncomment below as needed 
# az ad sp create-for-rbac --skip-assignment --name aksDevelop --scopes /subscriptions/$DEV_SUBSCRIPTION_ID
# az ad sp create-for-rbac --skip-assignment --name aksMaster --scopes /subscriptions/$DEV_SUBSCRIPTION_ID
# az ad sp create-for-rbac --skip-assignment --name aksProd --scopes /subscriptions/$PROD_SUBSCRIPTION_ID

echo "Save the above into Pulumi"
echo "pulumi stack select THE_STACK"
echo "pulumi config set aks:spAppId <AppId>"
echo "pulumi config set --secret aksSpAppPassword <Password>"