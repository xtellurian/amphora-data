import * as azure from "@pulumi/azure";
import * as pulumi from "@pulumi/pulumi";
import { IFrontendFqdns } from "../dns/front-door-dns";
import { IMainHostnames } from "./frontDoor";

const prodStack = new pulumi.StackReference(`xtellurian/amphora/prod`);
const prodHostnames = prodStack.getOutput("appHostnames"); // should be an array
const prodBackendCount = 2; // should match the array length from prod stack as above
const k8sPrimary = prodStack.getOutput("k8sPrimary");
const k8sSecondary = prodStack.getOutput("k8sSecondary");

interface IBackendPoolsParams {
    backendPoolNames: IBackendPoolNames;
    hostnames: IMainHostnames;
    frontendFqdns: IFrontendFqdns;
}

export interface IBackendPoolNames {
    prod: string;
    identity: string;
}

export function getBackendPools(params: IBackendPoolsParams)
    : pulumi.Input<Array<pulumi.Input<azure.types.input.frontdoor.FrontdoorBackendPool>>> {

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
            hostHeader: params.hostnames.app,
            httpPort: 80,
            httpsPort: 443,
        });
    }

    // add k8s primary + secondary backend from aks
    prodBackends.push({
        address: k8sPrimary.apply((k) => k.fqdn),
        hostHeader: params.hostnames.app,
        httpPort: 80,
        httpsPort: 443,
    });
    prodBackends.push({
        address: k8sSecondary.apply((k) => k.fqdn),
        hostHeader: params.hostnames.app,
        httpPort: 80,
        httpsPort: 443,
    });

    const prodBackendPool: azure.types.input.frontdoor.FrontdoorBackendPool = {
        backends: prodBackends,
        healthProbeName: "normal",
        loadBalancingName: "loadBalancingSettings1",
        name: params.backendPoolNames.prod,
    };

    backendPools.push(prodBackendPool);

    // IDENTITY BACKENDS
    const identityBackends: Array<pulumi.Input<azure.types.input.frontdoor.FrontdoorBackendPoolBackend>> = [];
    identityBackends.push({
        address: k8sPrimary.apply((k) => k.fqdn),
        hostHeader: params.hostnames.identity,
        httpPort: 80,
        httpsPort: 443,
    });
    identityBackends.push({
        address: k8sSecondary.apply((k) => k.fqdn),
        hostHeader: params.hostnames.identity,
        httpPort: 80,
        httpsPort: 443,
    });

    const identityBackendPool: azure.types.input.frontdoor.FrontdoorBackendPool = {
        backends: identityBackends,
        healthProbeName: "normal",
        loadBalancingName: "loadBalancingSettings1",
        name: params.backendPoolNames.identity,
    };

    backendPools.push(identityBackendPool);

    return backendPools;
}
