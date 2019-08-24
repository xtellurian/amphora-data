#!/bin/bash

SP_NAME=http://PulumiAzureDevOps


# Create SP for Pulumi & infra
az ad sp create-for-rbac --name $SP_NAME

# user access administrator
# https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#user-access-administrator
az role assignment create --assignee $SP_NAME --role 18d7d88d-d35e-4fb5-a5c3-7773c20a72d9
