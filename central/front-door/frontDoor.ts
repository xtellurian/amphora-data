import * as azure from "@pulumi/azure";

import { IFrontendHosts } from "../dns/front-door-dns";
import { getBackendPools, IBackendEnvironments } from "./backendPools";
import { getFrontendEndpoints } from "./frontends";

const backendEnvironments: IBackendEnvironments = {
    develop: {
        app: "devAppBackend",
        identity: "devIdBackend",
    },
    master : {
        app: "masterAppBackend",
        identity: "masterIdBackend",
    },
    prod: {
        app: "prodBackend",
        identity: "identityBackend",
    },
};

export function createFrontDoor({ rg, kv, frontendHosts }
        : { rg: azure.core.ResourceGroup; kv: azure.keyvault.KeyVault; frontendHosts: IFrontendHosts; }) {

    const backendPools = getBackendPools({ backendEnvironments, frontendHosts } );
    const frontendEndpoints = getFrontendEndpoints(kv, frontendHosts);

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
        frontendEndpoints,
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
                    backendPoolName: backendEnvironments.prod.app,
                    cacheEnabled: false,
                    cacheQueryParameterStripDirective: "StripNone",
                    forwardingProtocol: "HttpsOnly",
                },
                frontendEndpoints: ["betaDomain", frontendHosts.prod.app.frontendName],
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
                    frontendHosts.prod.app.frontendName,
                    frontendHosts.prod.identity.frontendName,
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
                    backendPoolName: backendEnvironments.prod.identity,
                    cacheEnabled: false,
                    cacheQueryParameterStripDirective: "StripNone",
                    forwardingProtocol: "HttpsOnly",
                },
                frontendEndpoints: [frontendHosts.prod.identity.frontendName],
                name: "routeToIdentityProdEnvironment",
                patternsToMatches: ["/*"],
            },
        ],
    });
}
