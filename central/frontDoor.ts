import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";

const prodStack = new pulumi.StackReference(`xtellurian/amphora/prod`);
const prodHostnames = prodStack.getOutput("appHostnames"); // should be an array
const prodBackendCount = 2; // should match the array length from prod stack as above
const k8sPrimary = prodStack.getOutput("k8sPrimary");
const k8sSecondary = prodStack.getOutput("k8sSecondary");

export function createFrontDoor(rg: azure.core.ResourceGroup, kv: azure.keyvault.KeyVault) {

    const appHostName = "app.amphoradata.com";
    const identityHostName = "identity.amphoradata.com";

    const backendPools: pulumi.Input<Array<pulumi.Input<azure.types.input.frontdoor.FrontdoorBackendPool>>> = [];
    // add the squarespace backend
    backendPools.push(
        {
            backends: [{
                address: "azalea-orca-x567.squarespace.com",
                hostHeader: "azalea-orca-x567.squarespace.com",
                httpPort: 80,
                httpsPort: 443,
            }],
            healthProbeName: "normal",
            loadBalancingName: "loadBalancingSettings1",
            name: "squarespaceBackend",
        });

    // APP BACKENDS
    // add app service backends
    const prodBackends: Array<pulumi.Input<azure.types.input.frontdoor.FrontdoorBackendPoolBackend>> = [];
    for (let i = 0; i < prodBackendCount; i++) {
        prodBackends.push({
            address: prodHostnames.apply((h) => h[i]),
            hostHeader: appHostName,
            httpPort: 80,
            httpsPort: 443,
        });
    }

    // add k8s primary + secondary backend from aks

    prodBackends.push({
        address: k8sPrimary.apply((k) => k.fqdn),
        hostHeader: appHostName,
        httpPort: 80,
        httpsPort: 443,
    });
    prodBackends.push({
        address: k8sSecondary.apply((k) => k.fqdn),
        hostHeader: appHostName,
        httpPort: 80,
        httpsPort: 443,
    });

    const prodBackendPoolName = "prodBackend";
    const prodBackendPool: azure.types.input.frontdoor.FrontdoorBackendPool = {
        backends: prodBackends,
        healthProbeName: "normal",
        loadBalancingName: "loadBalancingSettings1",
        name: prodBackendPoolName,
    };

    backendPools.push(prodBackendPool);

    const appFrontend = {
        customHttpsConfiguration: {
            certificateSource: "FrontDoor",
        },
        customHttpsProvisioningEnabled: true,
        hostName: appHostName,
        name: "appDomain",
        sessionAffinityEnabled: true,
    };

    // IDENTITY BACKENDS
    const identityBackends: Array<pulumi.Input<azure.types.input.frontdoor.FrontdoorBackendPoolBackend>> = [];
    identityBackends.push({
        address: k8sPrimary.apply((k) => k.fqdn),
        hostHeader: identityHostName,
        httpPort: 80,
        httpsPort: 443,
    });
    identityBackends.push({
        address: k8sSecondary.apply((k) => k.fqdn),
        hostHeader: identityHostName,
        httpPort: 80,
        httpsPort: 443,
    });

    const identityBackendPoolName = "identityBackend";
    const identityBackendPool: azure.types.input.frontdoor.FrontdoorBackendPool = {
        backends: identityBackends,
        healthProbeName: "normal",
        loadBalancingName: "loadBalancingSettings1",
        name: identityBackendPoolName,
    };

    backendPools.push(identityBackendPool);

    const identityFrontend = {
        customHttpsConfiguration: {
            certificateSource: "FrontDoor",
        },
        customHttpsProvisioningEnabled: true,
        hostName: identityHostName,
        name: "identityFrontend",
        sessionAffinityEnabled: true,
    };

    const frontDoor = new azure.frontdoor.Frontdoor("fd", {
        backendPoolHealthProbes: [{
            intervalInSeconds: 30,
            name: "normal",
            path: "/",
            protocol: "Https",
        }],
        backendPoolLoadBalancings: [{
            name: "loadBalancingSettings1",
            sampleSize: 4,
            successfulSamplesRequired: 2,
        }],
        backendPools,
        enforceBackendPoolsCertificateNameCheck: true,
        frontendEndpoints: [{
            customHttpsProvisioningEnabled: false,
            hostName: "amphora.azurefd.net",
            name: "defaultFrontend",
            sessionAffinityEnabled: true,
        }, {
            customHttpsConfiguration: {
                azureKeyVaultCertificateSecretName: "static-site",
                azureKeyVaultCertificateSecretVersion: "2cb1416e06e640229d2e28bacb1eb9cd",
                azureKeyVaultCertificateVaultId: kv.id,
                certificateSource: "AzureKeyVault",
            },
            customHttpsProvisioningEnabled: true,
            hostName: "amphoradata.com",
            name: "rootDomain",
            sessionAffinityEnabled: true,
        }, {
            customHttpsConfiguration: {
                certificateSource: "FrontDoor",
            },
            customHttpsProvisioningEnabled: true,
            hostName: "www.amphoradata.com",
            name: "wwwDomain",
            sessionAffinityEnabled: true,
        }, {
            customHttpsConfiguration: {
                certificateSource: "FrontDoor",
            },
            customHttpsProvisioningEnabled: true,
            hostName: "beta.amphoradata.com",
            name: "betaDomain",
            sessionAffinityEnabled: true,
        },
            appFrontend,
            identityFrontend,
        ],
        location: "global",
        name: "amphora",
        resourceGroupName: rg.name,
        routingRules: [
            {
                // Squarespace Backend Route
                acceptedProtocols: [
                    "Https",
                ],
                forwardingConfiguration: {
                    backendPoolName: "squarespaceBackend",
                    cacheQueryParameterStripDirective: "StripNone",
                    forwardingProtocol: "HttpsOnly",
                },
                frontendEndpoints: ["defaultFrontend", "rootDomain", "wwwDomain"],
                name: "routeToSquarespace",
                patternsToMatches: ["/*"],
            },
            {
                // Prod App Backend Route
                acceptedProtocols: [
                    "Https",
                ],
                forwardingConfiguration: {
                    backendPoolName: prodBackendPoolName,
                    cacheEnabled: false,
                    cacheQueryParameterStripDirective: "StripNone",
                    forwardingProtocol: "HttpsOnly",
                },
                frontendEndpoints: ["betaDomain", appFrontend.name],
                name: "routeToProdEnvironment",
                patternsToMatches: ["/*"],
            },
            {
                // Https Redirects Route
                acceptedProtocols: [
                    "Http",
                ],
                frontendEndpoints: [
                    "defaultFrontend",
                    "rootDomain",
                    "wwwDomain",
                    "betaDomain",
                    appFrontend.name,
                    identityFrontend.name,
                ],
                name: "redirectToHttps",
                patternsToMatches: ["/*"],
                redirectConfiguration: {
                    redirectProtocol: "HttpsOnly",
                    redirectType: "Found",
                },
            },
            {
                // Prod Identity Backend Route
                acceptedProtocols: [
                    "Https",
                ],
                forwardingConfiguration: {
                    backendPoolName: identityBackendPoolName,
                    cacheEnabled: false,
                    cacheQueryParameterStripDirective: "StripNone",
                    forwardingProtocol: "HttpsOnly",
                },
                frontendEndpoints: [identityFrontend.name],
                name: "routeToIdentityProdEnvironment",
                patternsToMatches: ["/*"],
            },
        ],
    });
}
