import * as azure from "@pulumi/azure";

import { IFrontendHosts } from "../dns/front-door-dns";
import { getBackendPools, IBackendEnvironments } from "./backendPools";
import { getFrontendEndpoints } from "./frontends";
import { getRoutingRules } from "./routingRules";

const backendEnvironments: IBackendEnvironments = {
    develop: {
        app: "devAppBackend",
        identity: "devIdBackend",
    },
    master: {
        app: "masterAppBackend",
        identity: "masterIdBackend",
    },
    prod: {
        app: "prodBackend",
        identity: "identityBackend",
    },
};

export function createFrontDoor({ rg, kv, frontendHosts }
    : { rg: azure.core.ResourceGroup; kv: azure.keyvault.KeyVault; frontendHosts: IFrontendHosts; })
    : IBackendEnvironments {

    const backendPools = getBackendPools({ backendEnvironments, frontendHosts });
    const frontendEndpoints = getFrontendEndpoints(kv, frontendHosts);
    const routingRules = getRoutingRules(backendEnvironments, frontendHosts);

    const frontDoor = new azure.frontdoor.Frontdoor("fd", {
        backendPoolHealthProbes: [{
            intervalInSeconds: 30,
            name: "normal",
            path: "/",
            protocol: "Http",
        },
        {
            intervalInSeconds: 30,
            name: "healthz",
            path: "/healthz",
            protocol: "Http",
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
        routingRules,
    });

    return backendEnvironments;
}
