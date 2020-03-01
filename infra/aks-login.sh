#!/bin/bash
set -x
name=$(pulumi stack output k8sName)
group=$(pulumi stack output k8sGroup)
az aks get-credentials -n "$name" -g "$group"
