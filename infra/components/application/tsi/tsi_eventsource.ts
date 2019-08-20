export function eventSourceTemplate() {
    return {
        $schema:
            "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
        contentVersion: "1.0.0.0",
        parameters: {
            eh_consumer_group: {
                defaultValue: "$default",
                type: "String",
            },
            environmentName: {
                type: "String",
            },
            eventHubName: {
                type: "String",
            },
            eventHubNamespaceName: {
                type: "String",
            },
            eventHubResourceId: {
                type: "String",
            },
            keyName: {
                type: "String",
            },
            location: {
                type: "String",
            },
            sharedAccessKey: {
                type: "String",
            },
            timestampPropertyName: {
                defaultValue: "t",
                type: "String",
            },
        },
        variables: {},
        // tslint:disable-next-line: object-literal-sort-keys
        resources: [
            {
                apiVersion: "2018-08-15-preview",
                kind: "Microsoft.EventHub",
                location: "[parameters('location')]",
                name: "[concat(parameters('environmentName'), '/data')]",
                properties: {
                    consumerGroupName: "[parameters('eh_consumer_group')]",
                    eventHubName: "[parameters('eventHubName')]",
                    eventSourceResourceId: "[parameters('eventHubResourceId')]",
                    keyName: "[parameters('keyName')]",
                    serviceBusNamespace: "[parameters('eventHubNamespaceName')]",
                    sharedAccessKey: "[parameters('sharedAccessKey')]",
                    timestampPropertyName: "[parameters('timestampPropertyName')]",
                },
                type: "Microsoft.TimeSeriesInsights/environments/eventsources",
            },
        ],
        outputs: {},
    };
}
