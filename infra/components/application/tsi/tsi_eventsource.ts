export function eventSourceTemplate() {
    return {
        $schema:
            "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
        contentVersion: "1.0.0.0",
        parameters: {
            location: {
                type: "String"
            },
            eventHubNamespaceName: {
                type: "String"
            },
            eventHubName: {
                type: "String"
            },
            eventHubResourceId: {
                type: "String"
            },
            eh_consumer_group: {
                defaultValue: "$default",
                type: "String"
            },
            environmentName: {
                type: "String"
            },
            keyName: {
                type: "String"
            },
            sharedAccessKey: {
                type: "String"
            },
            timestampPropertyName: {
                type: "String",
                defaultValue: "t"
            }
        },
        variables: {},
        resources: [
            {
                type: "Microsoft.TimeSeriesInsights/environments/eventsources",
                apiVersion: "2018-08-15-preview",
                name: "[concat(parameters('environmentName'), '/data')]",
                location: "[parameters('location')]",
                kind: "Microsoft.EventHub",
                properties: {
                    // eventSourceResourceId:
                    //   "[concat(resourceId('Microsoft.EventHub/namespaces', parameters('eventHubNamespaceName')), '/EventHub/' , parameters('eventHubName'))]",
                    eventSourceResourceId: "[parameters('eventHubResourceId')]",
                    consumerGroupName: "[parameters('eh_consumer_group')]",
                    keyName: "[parameters('keyName')]",
                    timestampPropertyName: "[parameters('timestampPropertyName')]",
                    eventHubName: "[parameters('eventHubName')]",
                    serviceBusNamespace: "[parameters('eventHubNamespaceName')]",
                    sharedAccessKey: "[parameters('sharedAccessKey')]",
                }
            }
        ],
        outputs: {}
    };
}

