#!/bin/bash
set -e
while getopts t:e: option
do
case "${option}"
in
t) TRIGGER_ID=${OPTARG};;
e) EVENT_GRID_TOPIC_ID=${OPTARG};;
esac
done

if [ "$TRIGGER_ID" != "" ]; then
    echo "TRIGGER_ID" is $TRIGGER_ID
else
    echo "TRIGGER_ID is empty"
    exit 1
fi
if [ "$EVENT_GRID_TOPIC_ID" != "" ]; then
    echo "EVENT_GRID_TOPIC_ID is " $EVENT_GRID_TOPIC_ID
else
    echo "EVENT_GRID_TOPIC_ID is empty"
    exit 1
fi

endpoint=$(az rest -m post -u "$TRIGGER_ID/listCallbackUrl?api-version=2016-06-01" --query value -o tsv)

az eventgrid event-subscription create --name test1 \
    --source-resource-id $EVENT_GRID_TOPIC_ID \
    --endpoint $endpoint