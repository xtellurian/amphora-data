export function frontDoorArm(tags: {}) {
    return {
        $schema: "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
        contentVersion: "1.0.0.0",
        parameters: {
            demoHostname: {
                type: "String",
            },
            frontDoorId: {
                defaultValue: "/subscriptions/651f2ed5-6e2f-41d0-b533-0cd28801ef2a/resourceGroups/amphora-central/providers/Microsoft.Network/frontdoors/amphora",
                type: "String",
            },
            frontDoorName: {
                metadata: {
                    description: "The name of the frontdoor resource.",
                },
                type: "string",
            },
            zoneName: {
                defaultValue: "amphoradata.com",
                type: "String",
            },
        },
        variables: {
            demoDomain: "beta.amphoradata.com",
            frontdoorLocation: "global",
            rootDomain: "amphoradata.com",
            squarespaceBackend: "azalea-orca-x567.squarespace.com",
            wwwDomain: "www.amphoradata.com",
        },
        // tslint:disable-next-line: object-literal-sort-keys
        resources: [
            {
                apiVersion: "2019-04-01",
                location: "[variables('frontdoorLocation')]",
                name: "[parameters('frontDoorName')]",
                properties: {
                    backendPools: [
                        {
                            name: "squarespaceBackend",
                            properties: {
                                backends: [
                                    {
                                        address: "[variables('squarespaceBackend')]",
                                        backendHostHeader: "[variables('squarespaceBackend')]",
                                        enabledState: "Enabled",
                                        httpPort: 80,
                                        httpsPort: 443,
                                        priority: 1,
                                        weight: 50,
                                    },
                                ],
                                healthProbeSettings: {
                                    // tslint:disable-next-line: max-line-length
                                    id: "[resourceId('Microsoft.Network/frontDoors/healthProbeSettings', parameters('frontDoorName'), 'normal')]",
                                },
                                loadBalancingSettings: {
                                    // tslint:disable-next-line: max-line-length
                                    id: "[resourceId('Microsoft.Network/frontDoors/loadBalancingSettings', parameters('frontDoorName'), 'loadBalancingSettings1')]",
                                },
                            },
                        },
                        {
                            name: "demoBackend",
                            properties: {
                                backends: [
                                    {
                                        address: "[parameters('demoHostname')]",
                                        backendHostHeader: "[parameters('demoHostname')]",
                                        enabledState: "Enabled",
                                        httpPort: 80,
                                        httpsPort: 443,
                                        priority: 1,
                                        weight: 50,
                                    },
                                ],
                                healthProbeSettings: {
                                    // tslint:disable-next-line: max-line-length
                                    id: "[resourceId('Microsoft.Network/frontDoors/healthProbeSettings', parameters('frontDoorName'), 'normal')]",
                                },
                                loadBalancingSettings: {
                                    // tslint:disable-next-line: max-line-length
                                    id: "[resourceId('Microsoft.Network/frontDoors/loadBalancingSettings', parameters('frontDoorName'), 'loadBalancingSettings1')]",
                                },
                            },
                        },
                    ],
                    enabledState: "Enabled",
                    frontendEndpoints: [
                        {
                            name: "defaultFrontend",
                            properties: {
                                hostName: "[concat(parameters('frontDoorName'), '.azurefd.net')]",
                                sessionAffinityEnabledState: "Enabled",
                            },
                        },
                        {
                            name: "rootDomain",
                            properties: {
                                hostName: "[variables('rootDomain')]",
                                sessionAffinityEnabledState: "Enabled",
                            },
                        },
                        {
                            name: "wwwDomain",
                            properties: {
                                hostName: "[variables('wwwDomain')]",
                                sessionAffinityEnabledState: "Enabled",
                            },
                        },
                        {
                            name: "demoDomain",
                            properties: {
                                hostName: "[variables('demoDomain')]",
                                sessionAffinityEnabledState: "Enabled",
                            },
                        },
                    ],
                    healthProbeSettings: [
                        {
                            name: "normal",
                            properties: {
                                intervalInSeconds: 30,
                                path: "/",
                                protocol: "Https",
                            },
                        },
                        {
                            name: "healthz",
                            properties: {
                                intervalInSeconds: 30,
                                path: "/healthz",
                                protocol: "Https",
                            },
                        },
                    ],
                    loadBalancingSettings: [
                        {
                            name: "loadBalancingSettings1",
                            properties: {
                                sampleSize: 4,
                                successfulSamplesRequired: 2,
                            },
                        },
                    ],
                    routingRules: [
                        {
                            name: "routeToSquarespace",
                            properties: {
                                acceptedProtocols: [
                                    "Https",
                                ],
                                enabledState: "Enabled",
                                frontendEndpoints: [
                                    {
                                        // tslint:disable-next-line: max-line-length
                                        id: "[resourceId('Microsoft.Network/frontDoors/frontendEndpoints', parameters('frontDoorName'), 'defaultFrontend')]",
                                    },
                                    {
                                        // tslint:disable-next-line: max-line-length
                                        id: "[resourceId('Microsoft.Network/frontDoors/frontendEndpoints', parameters('frontDoorName'), 'rootDomain')]",
                                    },
                                    {
                                        // tslint:disable-next-line: max-line-length
                                        id: "[resourceId('Microsoft.Network/frontDoors/frontendEndpoints', parameters('frontDoorName'), 'wwwDomain')]",
                                    },
                                ],
                                patternsToMatch: [
                                    "/*",
                                ],
                                routeConfiguration: {
                                    "@odata.type": "#Microsoft.Azure.FrontDoor.Models.FrontdoorForwardingConfiguration",
                                    "backendPool": {
                                        // tslint:disable-next-line: max-line-length
                                        id: "[resourceId('Microsoft.Network/frontDoors/backendPools', parameters('frontDoorName'), 'squarespaceBackend')]",
                                    },
                                    "cacheConfiguration": {
                                        dynamicCompression: "Enabled",
                                        queryParameterStripDirective: "StripNone",
                                    },
                                    "forwardingProtocol": "HttpsOnly", // could be MatchRequest
                                },
                            },
                        },
                        {
                            name: "routeToDemoEnvironment",
                            properties: {
                                acceptedProtocols: [
                                    "Https",
                                ],
                                enabledState: "Enabled",
                                frontendEndpoints: [
                                    {
                                        id: "[resourceId('Microsoft.Network/frontDoors/frontendEndpoints', parameters('frontDoorName'), 'demoDomain')]",
                                    },
                                ],
                                patternsToMatch: [
                                    "/*",
                                ],
                                routeConfiguration: {
                                    "@odata.type": "#Microsoft.Azure.FrontDoor.Models.FrontdoorForwardingConfiguration",
                                    "backendPool": {
                                        // tslint:disable-next-line: max-line-length
                                        id: "[resourceId('Microsoft.Network/frontDoors/backendPools', parameters('frontDoorName'), 'demoBackend')]",
                                    },
                                    "forwardingProtocol": "HttpsOnly", // could be MatchRequest
                                },
                            },
                        },
                    ],
                },
                tags,
                type: "Microsoft.Network/frontDoors",
            },
            {
                apiVersion: "2018-05-01",
                dependsOn: [
                    "[resourceId('Microsoft.Network/frontDoors', parameters('frontDoorName'))]",
                ],
                name: "[concat(parameters('zoneName'), '/@')]",
                properties: {
                    TTL: 60,
                    targetResource: {
                        id: "[resourceId('Microsoft.Network/frontDoors', parameters('frontDoorName'))]",
                    },
                },
                type: "Microsoft.Network/dnszones/A",
            },
        ],
        outputs: {},
    };
}
