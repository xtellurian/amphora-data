import * as azure from "@pulumi/azure";

import { IFrontendHosts } from "../dns/front-door-dns";
import { getBackendPools, IBackendPoolNames } from "./backendPools";

const poolNames: IBackendPoolNames = {
    app: "prodBackend",
    identity: "identityBackend",
};

export function createFrontDoor({ rg, kv, frontendHosts }
        : { rg: azure.core.ResourceGroup; kv: azure.keyvault.KeyVault; frontendHosts: IFrontendHosts; }) {

    const backendPools = getBackendPools({ poolNames, frontendHosts } );
    const appFrontend = {
        customHttpsConfiguration: {
            certificateSource: "FrontDoor",
        },
        customHttpsProvisioningEnabled: true,
        hostName: frontendHosts.prod.app.globalHost,
        name: "appDomain",
        sessionAffinityEnabled: true,
    };

    const identityFrontend = {
        customHttpsConfiguration: {
            certificateSource: "FrontDoor",
        },
        customHttpsProvisioningEnabled: true,
        hostName: frontendHosts.prod.identity.globalHost,
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
                    backendPoolName: poolNames.app,
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
                    backendPoolName: poolNames.identity,
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
