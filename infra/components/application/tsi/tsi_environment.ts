export function environmentTemplate() {
    return {
        $schema:
            "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
        contentVersion: "1.0.0.0",
        parameters: {
            environmentName: {
                type: "String",
            },
            location: {
                type: "String",
            },
            storageAccountName: {
                type: "String",
            },
            storageAccountResourceId: {
                type: "String",
            },
        },
        variables: {},
        // tslint:disable-next-line: object-literal-sort-keys
        resources: [
            {
                apiVersion: "2018-08-15-preview",
                kind: "LongTerm",
                location: "[parameters('location')]",
                name: "[parameters('environmentName')]",
                properties: {
                    storageConfiguration: {
                        accountName: "[parameters('storageAccountName')]",
                        managementKey:
                            "[listKeys(parameters('storageAccountResourceId'), '2018-02-01').keys[0].value]",
                    },
                    timeSeriesIdProperties: [
                        {
                            name: "tempora",
                            type: "string",
                        },
                    ],
                },
                sku: {
                    capacity: 1,
                    name: "L1",
                },
                type: "Microsoft.TimeSeriesInsights/environments",
            },
        ],
        outputs: {
            dataAccessFqdn: {
                type: "string",
                // tslint:disable-next-line: max-line-length
                value: "[reference(concat('Microsoft.TimeSeriesInsights/environments/', parameters('environmentName'))).dataAccessFqdn]",
            },
        },
    };
}
