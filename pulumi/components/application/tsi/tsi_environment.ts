export function environmentTemplate() {
    return {
        $schema:
            "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
        contentVersion: "1.0.0.0",
        parameters: {
            location: {
                type: "String"
            },
            environmentName: {
                type: "String"
            },
            storageAccountName: {
                type: "String"
            },
            storageAccountResourceId: {
                type: "String"
            },
        },
        variables: {},
        resources: [
            {
                type: "Microsoft.TimeSeriesInsights/environments",
                apiVersion: "2018-08-15-preview",
                name: "[parameters('environmentName')]",
                location: "[parameters('location')]",
                sku: {
                    name: "L1",
                    capacity: 1
                },
                kind: "LongTerm",
                properties: {
                    storageConfiguration: {
                        accountName: "[parameters('storageAccountName')]",
                        managementKey:
                            "[listKeys(parameters('storageAccountResourceId'), '2018-02-01').keys[0].value]"
                    },
                    timeSeriesIdProperties: [
                        {
                            name: "tempora",
                            type: "string"
                        }
                    ]
                }
            }
        ],
        outputs: {
            dataAccessFqdn: {
                type: "string",
                value: "[reference(concat('Microsoft.TimeSeriesInsights/environments/', parameters('environmentName'))).dataAccessFqdn]"
            }
        }
    };
}

