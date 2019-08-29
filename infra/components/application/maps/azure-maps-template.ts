export function azureMapsTemplate() {
    return {
        $schema: "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
        contentVersion: "1.0.0.0",
        parameters: {
            account_name: {
                type: "String",
            },
        },
        variables: {},
        // tslint:disable-next-line: object-literal-sort-keys
        resources: [
            {
                apiVersion: "2018-05-01",
                location: "global",
                name: "[parameters('account_name')]",
                sku: {
                    name: "s0",
                    tier: "Standard",
                },
                type: "Microsoft.Maps/accounts",
            },
        ],
        outputs: {
            clientId: {
                type: "string",
                // tslint:disable-next-line: max-line-length
                value: "[reference(concat('Microsoft.Maps/accounts/', parameters('account_name')))['x-ms-client-id']]",
            },
            resourceId: {
                type: "string",
                // tslint:disable-next-line: max-line-length
                value: "[resourceId('Microsoft.Maps/accounts/', parameters('account_name'))]",
            },
        },
    };
}
