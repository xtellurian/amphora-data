#!/bin/bash

# this script updates the timeout of the Azure Front Door
# https://docs.microsoft.com/en-us/azure/frontdoor/front-door-troubleshoot-routing#503-response-from-front-door-after-a-few-seconds
az network front-door update --name amphora -g amphora-central --send-recv-timeout 60
