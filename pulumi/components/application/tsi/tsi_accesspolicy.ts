export function accessPolicyTemplate() {
    return {
        $schema:
            "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
        contentVersion: "1.0.0.0",
        parameters: {
            environmentName: {
                type: "String"
            },
            accessPolicyReaderObjectId: {
                type: "String",
                defaultValue: ""
            },
        },
        variables: {},
        resources: [
            {
                condition: "[not(empty(parameters('accessPolicyReaderObjectId')))]",
                apiVersion: "2018-08-15-preview",
                name:
                    "[concat(parameters('environmentName'), '/', 'ownerAccessPolicy')]",
                type: "Microsoft.TimeSeriesInsights/environments/accesspolicies",
                properties: {
                    principalObjectId: "[parameters('accessPolicyReaderObjectId')]",
                    roles: ["Reader", "Contributor"]
                },
                dependsOn: [
                    "[concat('Microsoft.TimeSeriesInsights/environments/', parameters('environmentName'))]"
                ]
            },
        ],
        outputs: {}
    };
}
