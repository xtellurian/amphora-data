import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";

import { IFrontendHosts } from "../dns/front-door-dns";
import { getBackendPools, IBackendEnvironments } from "./backendPools";
import { getFrontendEndpoints } from "./frontends";
import { getRoutingRules } from "./routingRules";

const backendEnvironments: IBackendEnvironments = {
    develop: {
        api: "devApiBackend",
        app: "devAppBackend",
        identity: "devIdBackend",
    },
    master: {
        api: "masterApiBackend",
        app: "masterAppBackend",
        identity: "masterIdBackend",
    },
    prod: {
        api: "prodApiBackend",
        app: "prodBackend",
        identity: "identityBackend",
    },
};

export function createFrontDoor({
    rg,
    kv,
    frontendHosts,
    prodHostnames,
}: // tslint:disable-next-line: max-line-length
{
    rg: azure.core.ResourceGroup;
    kv: azure.keyvault.KeyVault;
    frontendHosts: IFrontendHosts;
    prodHostnames: pulumi.Output<any>;
}): IBackendEnvironments {
    const backendPools = getBackendPools({
        backendEnvironments,
        frontendHosts,
        prodHostnames,
    });
    const frontendEndpoints = getFrontendEndpoints(kv, frontendHosts);
    const routingRules = getRoutingRules(backendEnvironments, frontendHosts);

    const frontDoor = new azure.frontdoor.Frontdoor("fd", {
        backendPoolHealthProbes: [
            {
                intervalInSeconds: 30,
                name: "quickstart",
                path: "/quickstart",
                protocol: "Https",
            },
            {
                intervalInSeconds: 30,
                name: "normal",
                path: "/",
                protocol: "Https",
            },
        ],
        backendPoolLoadBalancings: [
            {
                name: "loadBalancingSettings1",
                sampleSize: 4,
                successfulSamplesRequired: 2,
            },
        ],
        backendPools,
        backendPoolsSendReceiveTimeoutSeconds: 90,
        enforceBackendPoolsCertificateNameCheck: true,
        frontendEndpoints,
        name: "amphora",
        resourceGroupName: rg.name,
        routingRules,
        tags: {
            component: "constant",
            project: pulumi.getProject(),
            source: "pulumi",
            stack: pulumi.getStack(),
        },
    });

    return backendEnvironments;
}
