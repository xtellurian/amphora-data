import * as azure from "@pulumi/azure";
import { Input } from "@pulumi/pulumi";
import { IFrontendHosts } from "../dns/front-door-dns";
import { IBackendEnvironments } from "./backendPools";

export function getRoutingRules(backendEnvironments: IBackendEnvironments, frontendHosts: IFrontendHosts)
    : Input<Array<Input<azure.types.input.frontdoor.FrontdoorRoutingRule>>> {
    return [
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
    ];
}
