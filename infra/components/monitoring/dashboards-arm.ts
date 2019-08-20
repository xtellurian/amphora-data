export function getDashboardTemplate() {
  return {
    $schema:
      "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
    contentVersion: "1.0.0.0",
    parameters: {
      storageAccountType: {
        allowedValues: ["Standard_LRS", "Standard_GRS", "Standard_ZRS"],
        defaultValue: "Standard_LRS",
        metadata: {
          description: "Storage Account type",
        },
        type: "string",
      },
    },
    variables: {
      apiVersion: "2015-06-15",
      location: "[resourceGroup().location]",
      publicIPAddressName:
        "[concat('myPublicIp', uniquestring(resourceGroup().id))]",
      publicIPAddressType: "Dynamic",
      storageAccountName:
        "[concat(uniquestring(resourceGroup().id), 'storage')]",
    },
    // tslint:disable-next-line: object-literal-sort-keys
    resources: [
      {
        apiVersion: "[variables('apiVersion')]",
        location: "[variables('location')]",
        name: "[variables('storageAccountName')]",
        properties: {
          accountType: "[parameters('storageAccountType')]",
        },
        type: "Microsoft.Storage/storageAccounts",
      },
      {
        apiVersion: "[variables('apiVersion')]",
        location: "[variables('location')]",
        name: "[variables('publicIPAddressName')]",
        properties: {
          dnsSettings: {
            domainNameLabel: "[variables('dnsLabelPrefix')]",
          },
          publicIPAllocationMethod: "[variables('publicIPAddressType')]",
        },
        type: "Microsoft.Network/publicIPAddresses",
      },
    ],
    outputs: {
      storageAccountName: {
        type: "string",
        value: "[variables('storageAccountName')]",
      },
    },
  };
}
