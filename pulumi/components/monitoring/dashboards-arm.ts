export function getDashboardTemplate() {
  return {
    $schema:
      "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
    contentVersion: "1.0.0.0",
    parameters: {
      storageAccountType: {
        type: "string",
        defaultValue: "Standard_LRS",
        allowedValues: ["Standard_LRS", "Standard_GRS", "Standard_ZRS"],
        metadata: {
          description: "Storage Account type"
        }
      }
    },
    variables: {
      location: "[resourceGroup().location]",
      storageAccountName:
        "[concat(uniquestring(resourceGroup().id), 'storage')]",
      publicIPAddressName:
        "[concat('myPublicIp', uniquestring(resourceGroup().id))]",
      publicIPAddressType: "Dynamic",
      apiVersion: "2015-06-15"
    },
    resources: [
      {
        type: "Microsoft.Storage/storageAccounts",
        name: "[variables('storageAccountName')]",
        apiVersion: "[variables('apiVersion')]",
        location: "[variables('location')]",
        properties: {
          accountType: "[parameters('storageAccountType')]"
        }
      },
      {
        type: "Microsoft.Network/publicIPAddresses",
        apiVersion: "[variables('apiVersion')]",
        name: "[variables('publicIPAddressName')]",
        location: "[variables('location')]",
        properties: {
          publicIPAllocationMethod: "[variables('publicIPAddressType')]",
          dnsSettings: {
            domainNameLabel: "[variables('dnsLabelPrefix')]"
          }
        }
      }
    ],
    outputs: {
      storageAccountName: {
        type: "string",
        value: "[variables('storageAccountName')]"
      }
    }
  };
}
