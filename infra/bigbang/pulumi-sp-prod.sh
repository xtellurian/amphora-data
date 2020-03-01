#!/bin/bash

SUB="AI for Earth"
SP_NAME_PROD=http://PulumiAzureDevOpsProd
# https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#user-access-administrator
ROLE_ID="18d7d88d-d35e-4fb5-a5c3-7773c20a72d9" # User Access Administrator

# Create SP for Pulumi & infra
az ad sp create-for-rbac --name $SP_NAME_PROD --scopes /subscriptions/$SUB

# user access administrator
# https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#user-access-administrator
az role assignment create --assignee $SP_NAME_PROD --role $ROLE_ID --subscription "$SUB"
