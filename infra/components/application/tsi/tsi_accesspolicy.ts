export function accessPolicyTemplate() {
    return {
        $schema:
            "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
        contentVersion: "1.0.0.0",
        parameters: {
            accessPolicyReaderObjectId: {
                defaultValue: "",
                type: "String",
            },
            environmentName: {
                type: "String",
            },
            name: {
                type: "String",
            },
        },
        variables: {},
        // tslint:disable-next-line: object-literal-sort-keys
        resources: [
            {
                apiVersion: "2018-08-15-preview",
                condition: "[not(empty(parameters('accessPolicyReaderObjectId')))]",
                name:
                    "[concat(parameters('environmentName'), '/', parameters('name'))]",
                properties: {
                    principalObjectId: "[parameters('accessPolicyReaderObjectId')]",
                    roles: ["Reader", "Contributor"],
                },
                type: "Microsoft.TimeSeriesInsights/environments/accesspolicies",
            },
        ],
        outputs: {},
    };
}
