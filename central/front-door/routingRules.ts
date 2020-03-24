import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { Input } from "@pulumi/pulumi";
import { IFrontendHosts, IUniqueUrl } from "../dns/front-door-dns";
import { IBackendEnvironments } from "./backendPools";

const config = new pulumi.Config();

export function getRoutingRules(backendEnvironments: IBackendEnvironments, frontendHosts: IFrontendHosts)
    : Input<Array<Input<azure.types.input.frontdoor.FrontdoorRoutingRule>>> {
    const rules: Input<Array<Input<azure.types.input.frontdoor.FrontdoorRoutingRule>>> = [
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
        getRule("routeToProdEnvironment", backendEnvironments.prod.app, frontendHosts.prod.app, ["betaDomain"]),
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
                frontendHosts.develop.app.frontendName,
                frontendHosts.develop.identity.frontendName,
            ],
            name: "redirectToHttps",
            patternsToMatches: ["/*"],
            redirectConfiguration: {
                redirectProtocol: "HttpsOnly",
                redirectType: "Found",
            },
        },
        getRule("routeToIdentityProdEnvironment", backendEnvironments.prod.identity, frontendHosts.prod.identity),
    ];

    if (config.requireBoolean("deployDevelop")) {
        rules.push(getRule("developIdRoute", backendEnvironments.develop.identity, frontendHosts.develop.identity));
        rules.push(getRule("developAppRoute", backendEnvironments.develop.app, frontendHosts.develop.app));
    }

    if (config.requireBoolean("deployMaster")) {
        rules.push(getRule("masterIdRoute", backendEnvironments.master.identity, frontendHosts.master.identity));
        rules.push(getRule("masterAppRoute", backendEnvironments.master.app, frontendHosts.master.app));
    }

    return rules;
}

function getRule(name: string, backendPoolName: string, frontend: IUniqueUrl, additionalFrontendNames?: string[])
    : Input<azure.types.input.frontdoor.FrontdoorRoutingRule> {
    const frontendEndpoints = [
        frontend.frontendName,
    ];
    if (additionalFrontendNames && additionalFrontendNames.length > 0) {
        frontendEndpoints.push(...additionalFrontendNames);
    }

    return {
        // Standard Route
        acceptedProtocols: [
            "Https",
        ],
        forwardingConfiguration: {
            backendPoolName,
            cacheEnabled: false,
            cacheQueryParameterStripDirective: "StripNone",
            forwardingProtocol: "HttpsOnly",
        },
        frontendEndpoints,
        name,
        patternsToMatches: ["/*"],
    };
}
