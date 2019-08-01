export function getTsiTemplate() {
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
      storageAccountName: {
        type: "String"
      },
      accessPolicyReaderObjectId: {
        type: "String",
        defaultValue: ""
      }
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
              "[listKeys(resourceId('Microsoft.Storage/storageAccounts', parameters('storageAccountName')), '2018-02-01').keys[0].value]"
          },
          timeSeriesIdProperties: [
            {
              name: "s",
              type: "string"
            }
          ]
        }
      },
      {
        type: "Microsoft.TimeSeriesInsights/environments/eventsources",
        apiVersion: "2018-08-15-preview",
        name: "[concat(parameters('environmentName'), '/data')]",
        location: "[parameters('location')]",
        dependsOn: [
          "[resourceId('Microsoft.TimeSeriesInsights/environments', parameters('environmentName'))]"
        ],
        kind: "Microsoft.EventHub",
        properties: {
          // eventSourceResourceId:
          //   "[concat(resourceId('Microsoft.EventHub/namespaces', parameters('eventHubNamespaceName')), '/EventHub/' , parameters('eventHubName'))]",
          eventSourceResourceId: "[parameters('eventHubResourceId')]",
          consumerGroupName: "[parameters('eh_consumer_group')]",
          keyName: "[parameters('keyName')]",
          timestampPropertyName: "t",
          eventHubName: "[parameters('eventHubName')]",
          serviceBusNamespace: "[parameters('eventHubNamespaceName')]",
          sharedAccessKey: "[parameters('sharedAccessKey')]",
        }
      },
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
    ]
  };
}

// eventHubName, serviceBusNamespace, consumerGroupName, keyName, sharedAccessKey
